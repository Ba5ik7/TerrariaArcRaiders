using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: future implementations will ensure raid anchor tiles/objects are placed.
    internal class ArcStageG_RaidAnchors : GenPass, IArcWorldGenPass
    {
        public ArcStageG_RaidAnchors() : base("Arc Stage G - Raid Anchors", 0.5f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageG_RaidAnchors;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc raid anchors";
            ArcWorldGenLog.StageOrder("Stage G", "Raid Anchors (reserved only)");
            // No-op placeholder; reserved sites are already recorded for future placement (bounded list, no scans).
            _ = ArcWorldSystem.WorldData?.ReservedSites;

            ArcWorldSystem.WorldGenIndicatorRunState.MarkCompleted(Stage);
        }
    }
}
