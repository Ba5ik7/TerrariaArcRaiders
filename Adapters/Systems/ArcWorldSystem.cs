using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.Systems
{
    // Tracks Arc world identity and persists minimal Arc metadata.
    public class ArcWorldSystem : ModSystem
    {
        private const string DataKey = "arcWorld";
        private const string HeaderIsArcKey = "arc:isArc";
        private const string HeaderVersionKey = "arc:dataVersion";

        internal static bool IsArcWorld { get; set; }
        internal static ArcWorldData WorldData { get; set; } = ArcWorldData.NonArc();
        internal static ArcWorldSelection Selection { get; set; } = ArcWorldSelection.FromSeedText(null);

        public static bool TryGetHubRegion(out IntRect hub)
        {
            hub = default;
            if (!IsArcWorld || WorldData == null)
            {
                return false;
            }

            hub = WorldData.SafeHubRegion;
            return hub.IsValid;
        }

        public static IReadOnlyList<ArcReservedSite> GetReservedSites()
        {
            if (!IsArcWorld || WorldData == null || WorldData.ReservedSites == null)
            {
                return Array.Empty<ArcReservedSite>();
            }

            return WorldData.ReservedSites;
        }

        public override void OnWorldUnload()
        {
            ResetToNonArc();
        }

        public override void PreWorldGen()
        {
            ResetToNonArc();

            var selection = ArcWorldSelection.FromSeedText(Terraria.WorldGen.currentWorldSeed);
            Selection = selection;
            if (!selection.IsArcWorld)
            {
                return;
            }

            IsArcWorld = true;
            WorldData = new ArcWorldData
            {
                IsArcWorld = true,
                DataVersion = ArcWorldData.CurrentDataVersion,
                SafeHubRegion = new IntRect(0, 0, 1, 1),
            };
        }

        public override void LoadWorldData(TagCompound tag)
        {
            ResetToNonArc();

            try
            {
                if (tag != null && tag.ContainsKey(DataKey) && tag[DataKey] is TagCompound dataTag)
                {
                    var loaded = ArcWorldDataBridge.FromTagCompound(dataTag);
                    ApplyLoadedData(loaded);
                    return;
                }
            }
            catch
            {
                // Fall through to non-Arc defaults for world safety.
            }

            ApplyLoadedData(ArcWorldData.NonArc());
        }

        public override void OnWorldLoad()
        {
            if (WorldData == null)
            {
                ResetToNonArc();
            }
        }

        public override void SaveWorldHeader(TagCompound tag)
        {
            if (!IsArcWorld)
            {
                return;
            }

            tag[HeaderIsArcKey] = true;
            tag[HeaderVersionKey] = WorldData?.DataVersion ?? ArcWorldData.CurrentDataVersion;
        }

        public override void SaveWorldData(TagCompound tag)
        {
            if (!IsArcWorld)
            {
                return;
            }

            EnsureSafeHubRegion();
            tag[DataKey] = ArcWorldDataBridge.ToTagCompound(WorldData);
        }

        private static void ApplyLoadedData(ArcWorldData data)
        {
            WorldData = data ?? ArcWorldData.NonArc();
            IsArcWorld = WorldData.IsArcWorld;

            if (IsArcWorld)
            {
                EnsureSafeHubRegion();
                ArcWorldGenLog.Info("Arc world detected (load)");
            }
            else
            {
                ResetToNonArc();
            }
        }

        private static void EnsureSafeHubRegion()
        {
            if (WorldData == null)
            {
                WorldData = ArcWorldData.NonArc();
            }

            if (!WorldData.SafeHubRegion.IsValid)
            {
                WorldData.SafeHubRegion = new IntRect(0, 0, 1, 1);
            }

            if (WorldData.DataVersion <= 0)
            {
                WorldData.DataVersion = ArcWorldData.CurrentDataVersion;
            }
        }

        private static void ResetToNonArc()
        {
            IsArcWorld = false;
            WorldData = ArcWorldData.NonArc();
            Selection = ArcWorldSelection.FromSeedText(null);
        }
    }
}
