using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: future implementations will ensure raid anchor tiles/objects are placed.
    internal class ArcStageG_RaidAnchors : GenPass
    {
        public ArcStageG_RaidAnchors() : base("Arc Stage G - Raid Anchors", 0.5f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc raid anchors";
            ArcWorldGenLog.StageOrder("Stage G", "Raid Anchors (reserved only)");
            // No-op placeholder; reserved sites are already recorded for future placement.
            _ = ArcWorldSystem.WorldData?.ReservedSites;
        }
    }
}
