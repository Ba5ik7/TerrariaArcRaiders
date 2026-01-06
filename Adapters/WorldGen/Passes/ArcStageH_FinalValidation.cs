using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    internal class ArcStageH_FinalValidation : GenPass, IArcWorldGenPass
    {
        public ArcStageH_FinalValidation() : base("Arc Stage H - Final Validation", 0.5f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageH_FinalValidation;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc final validation";
            ArcWorldGenLog.StageOrder("Stage H", "Final Validation");

            var data = ArcWorldSystem.WorldData;
            if (data == null || !data.IsArcWorld)
            {
                ArcWorldSystem.IsArcWorld = false;
                ArcWorldSystem.WorldData = ArcWorldData.NonArc();
                ArcWorldSystem.WorldGenIndicatorRunState.MarkCompleted(Stage);
                return;
            }

            var hasHub = data.SafeHubRegion.IsValid;
            var extraRegionCount = data.Regions?.Count > 0 ? data.Regions.Count - (data.Regions.ContainsKey(ArcRegionId.SafeHub) ? 1 : 0) : 0;

            if (!hasHub || extraRegionCount < 1)
            {
                // Fail safe: treat as non-Arc to protect world load if planning failed.
                ArcWorldSystem.IsArcWorld = false;
                ArcWorldSystem.WorldData = ArcWorldData.NonArc();
                ArcWorldGenLog.Info("Arc validation failed; reverting to non-Arc for safety.");
            }

            ArcWorldSystem.WorldGenIndicatorRunState.MarkCompleted(Stage);
        }
    }
}
