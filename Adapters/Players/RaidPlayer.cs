using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

#nullable enable

namespace TerrariaArcRaiders.Adapters.Players
{
    public class RaidPlayer : ModPlayer
    {
        private readonly RaidSessionService _sessionService = new();

        private RaidSession? _session;

        private bool _respawnToHub;

        public RaidSession? CurrentSession => _session;

        public bool IsInRaid => _session != null && _session.Status != RaidSessionStatus.Unknown && _session.Status != RaidSessionStatus.Extracted && _session.Status != RaidSessionStatus.Failed;

        private string PlayerId => Player.name;

        public bool TryEnterRaid()
        {
            if (IsInRaid)
            {
                return false;
            }

            _session = _sessionService.StartSession(PlayerId);
            _sessionService.ActivateSession(_session);
            Notify("Entering ARC raid zone.");
            return true;
        }

        public void AwardScrap(int amount)
        {
            if (!IsInRaid)
            {
                return;
            }

            _sessionService.AddScrap(_session!, amount);
        }

        public bool TryExtract()
        {
            if (_session == null)
            {
                return false;
            }

            var inventoryFull = IsInventoryFull();
            var stash = RaidSystem.GetOrCreateStash(PlayerId);
            _sessionService.Extract(_session, stash);
            _session = null;
            Notify("Extraction successful. Stash updated.");

            if (inventoryFull)
            {
                Notify("Player inventory is full; stash still updated.");
            }
            return true;
        }

        public void FailRun()
        {
            if (_session == null)
            {
                return;
            }

            _sessionService.Fail(_session);
            _session = null;
            _respawnToHub = true;
            Notify("Raid failed. Scrap lost.");
        }

        public override void OnEnterWorld()
        {
            _session = null;
            _ = RaidSystem.GetOrCreateStash(PlayerId);
            _respawnToHub = false;
        }

        public override void SaveData(TagCompound tag)
        {
            var stash = RaidSystem.GetOrCreateStash(PlayerId);
            var dto = RaidSystem.Persistence.Save(stash, sessionSnapshot: null);
            tag[nameof(RaidSystem.Stashes)] = TagCompoundBridge.ToTagCompound(dto);
        }

        public override void LoadData(TagCompound tag)
        {
            _respawnToHub = false;

            if (!tag.ContainsKey(nameof(RaidSystem.Stashes)))
            {
                return;
            }

            try
            {
                var stashTag = tag.Get<TagCompound>(nameof(RaidSystem.Stashes));
                var dto = TagCompoundBridge.FromTagCompound(stashTag);
                var stash = RaidSystem.GetOrCreateStash(PlayerId);
                RaidSystem.Persistence.Load(dto, stash, out _);
            }
            catch
            {
                // Ignore corrupt data to keep world loads safe.
            }
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (IsInRaid)
            {
                FailRun();
            }

            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void OnRespawn()
        {
            base.OnRespawn();

            if (_respawnToHub)
            {
                TeleportToSpawn();
                _respawnToHub = false;
            }
        }

        private void TeleportToSpawn()
        {
            var spawnPosition = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            Player.Teleport(spawnPosition, TeleportationStyleID.RodOfDiscord);
        }

        private bool IsInventoryFull()
        {
            for (var i = 0; i < Player.inventory.Length; i++)
            {
                if (Player.inventory[i].IsAir)
                {
                    return false;
                }
            }

            return true;
        }

        private static void Notify(string message)
        {
            Main.NewText(message);
        }
    }
}
