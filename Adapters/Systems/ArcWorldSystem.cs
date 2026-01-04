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

        internal static bool IsArcWorld { get; private set; }
        internal static ArcWorldData WorldData { get; private set; } = ArcWorldData.NonArc();
        internal static ArcWorldSelection Selection { get; private set; } = ArcWorldSelection.FromSeedText(null);

        public override void OnWorldUnload()
        {
            ResetToNonArc();
        }

        public override void PreWorldGen()
        {
            ResetToNonArc();

            var selection = ArcWorldSelection.FromSeedText(WorldGen.currentWorldSeed);
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
        }

        private static void ResetToNonArc()
        {
            IsArcWorld = false;
            WorldData = ArcWorldData.NonArc();
            Selection = ArcWorldSelection.FromSeedText(null);
        }
    }
}
