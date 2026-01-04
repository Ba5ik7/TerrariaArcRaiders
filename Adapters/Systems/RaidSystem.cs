using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Adapters.Systems
{
    public class RaidSystem : ModSystem
    {
        private const string StashKey = "raidStashes";

        internal static IRaidPersistence Persistence { get; set; } = new RaidPersistence();

        internal static Dictionary<string, Stash> Stashes { get; } = new();

        internal static Stash GetOrCreateStash(string playerId)
        {
            if (!Stashes.TryGetValue(playerId, out var stash))
            {
                stash = new Stash();
                Stashes[playerId] = stash;
            }

            return stash;
        }

        public override void OnWorldUnload()
        {
            Stashes.Clear();
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
        }

        public override void LoadWorldData(TagCompound tag)
        {
            Stashes.Clear();

            if (!tag.ContainsKey(StashKey))
            {
                return;
            }

            var stashRoot = tag.Get<TagCompound>(StashKey);
            foreach (var pair in stashRoot)
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
        }
    }
}
