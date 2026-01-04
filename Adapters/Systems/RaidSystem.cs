using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaArcRaiders.Adapters.Players;
using TerrariaArcRaiders.Adapters.NPCs;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Adapters.Systems
{
    public class RaidSystem : ModSystem
    {
        private const string StashKey = "raidStashes";
        private const string PortalKey = "raidEntryPortal";
        private const string PortalXKey = "x";
        private const string PortalYKey = "y";

        internal static IRaidPersistence Persistence { get; set; } = new RaidPersistence();

        internal static Dictionary<string, Stash> Stashes { get; } = new();

        internal static bool HasPortal { get; private set; }

        internal static Point PortalTile { get; private set; }

        internal static Stash GetOrCreateStash(string playerId)
        {
            if (!Stashes.TryGetValue(playerId, out var stash))
            {
                stash = new Stash();
                Stashes[playerId] = stash;
            }

            return stash;
        }

        public override void OnWorldLoad()
        {
            Stashes.Clear();
            HasPortal = false;
            PortalTile = default;
            EnsurePortalInitialized();
            EnsureHubConsoleExists();
        }

        public override void OnWorldUnload()
        {
            Stashes.Clear();
            HasPortal = false;
            PortalTile = default;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            var stashRoot = new TagCompound();

            foreach (var pair in Stashes)
            {
                var dto = Persistence.Save(pair.Value, sessionSnapshot: null);
                stashRoot[pair.Key] = TagCompoundBridge.ToTagCompound(dto);
            }

            tag[StashKey] = stashRoot;

            // Do not persist portal metadata: the hub console and portal anchor are derived from spawn each load.
            // This keeps worlds robust if the feature is removed and avoids unnecessary world data writes.
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Stashes.Clear();
            HasPortal = false;
            PortalTile = default;

            if (!tag.ContainsKey(StashKey))
            {
                InitializePortalFromTag(tag);
                return;
            }

            var stashRoot = tag.Get<TagCompound>(StashKey);
            foreach (var pair in stashRoot)
            {
                try
                {
                    if (pair.Value is not TagCompound stashTag)
                    {
                        continue;
                    }

                    var dto = TagCompoundBridge.FromTagCompound(stashTag);
                    var stash = new Stash();
                    Persistence.Load(dto, stash, out _);
                    Stashes[pair.Key] = stash;
                }
                catch
                {
                    // Ignore corrupt stash entries to preserve world load safety.
                }
            }

            InitializePortalFromTag(tag);
        }

        internal static bool TryInteractPortal(Player player)
        {
            EnsurePortalInitialized();

            if (player == null)
            {
                return false;
            }

            var raidPlayer = player.GetModPlayer<RaidPlayer>();
            return raidPlayer.TryEnterRaid();
        }

        internal static bool TryInteractExit(Player player)
        {
            if (player == null)
            {
                return false;
            }

            var raidPlayer = player.GetModPlayer<RaidPlayer>();
            if (!raidPlayer.IsInRaid)
            {
                return false;
            }

            var extracted = raidPlayer.TryExtract();
            if (!extracted)
            {
                return false;
            }

            SendPlayerToHub(player);
            return true;
        }

        private static void EnsurePortalInitialized()
        {
            if (HasPortal)
            {
                return;
            }

            var spawnTile = GetDefaultPortalTile();
            PortalTile = spawnTile;
            HasPortal = true;
        }

        private static Point GetDefaultPortalTile()
        {
            // Anchor near spawn without mutating tiles; future portal tile can bind to this position.
            var x = Main.spawnTileX;
            var y = Main.spawnTileY - 1;
            return new Point(x, y);
        }

        private static void SendPlayerToHub(Player player)
        {
            // Teleport to spawn as a hub stand-in without modifying world tiles.
            var spawnPosition = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            player.Teleport(spawnPosition, TeleportationStyleID.RodOfDiscord);
        }

        private static void InitializePortalFromTag(TagCompound tag)
        {
            try
            {
                if (tag.ContainsKey(PortalKey))
                {
                    var portalTag = tag.Get<TagCompound>(PortalKey);
                    if (portalTag != null && portalTag.ContainsKey(PortalXKey) && portalTag.ContainsKey(PortalYKey))
                    {
                        var x = portalTag.GetInt(PortalXKey);
                        var y = portalTag.GetInt(PortalYKey);
                        PortalTile = new Point(x, y);
                        HasPortal = true;
                        return;
                    }
                }
            }
            catch
            {
                // Ignore corrupt portal metadata to preserve world load safety.
            }

            EnsurePortalInitialized();
        }

        private static void EnsureHubConsoleExists()
        {
            // Only the server/single-player host should spawn NPCs to avoid duplicates.
            if (Main.netMode != NetmodeID.Server && Main.netMode != NetmodeID.SinglePlayer)
            {
                return;
            }

            var consoleType = ModContent.NPCType<RaidHubConsoleNPC>();
            foreach (var npc in Main.npc)
            {
                if (npc != null && npc.active && npc.type == consoleType)
                {
                    return;
                }
            }

            var spawnWorldPosition = GetDefaultPortalTile();
            var spawnPixelPosition = new Vector2(spawnWorldPosition.X * 16, spawnWorldPosition.Y * 16);
            var source = new EntitySource_WorldEvent(nameof(RaidHubConsoleNPC));
            NPC.NewNPC(source, (int)spawnPixelPosition.X, (int)spawnPixelPosition.Y, consoleType);
        }
    }
}
