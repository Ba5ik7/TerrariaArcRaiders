using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen.Indicators;
using TerrariaArcRaiders.Core.WorldGen;
using TerrariaArcRaiders.Core.WorldGen.Indicators;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Places in-world (vanilla-safe) visual markers for Arc worldgen stages.
    // This pass is intended to run AFTER vanilla worldgen tasks so markers are not overwritten.
    internal sealed class ArcStageI_VisualIndicators : GenPass
    {
        public ArcStageI_VisualIndicators() : base("Arc Stage I - Visual Indicators", 0.1f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            if (!ArcWorldSystem.IsArcWorld)
            {
                return;
            }

            var config = ArcRaidersConfig.Instance;
            if (config == null || !config.WorldGenVisualIndicatorsEnabled)
            {
                return;
            }

            progress.Message = "Arc visual indicators";

            try
            {
                var data = ArcWorldSystem.WorldData;
                if (data == null || !data.SafeHubRegion.IsValid)
                {
                    return;
                }

                var hub = data.SafeHubRegion;

                // Until per-stage completion marking is wired (T014+), fall back to "all stages".
                // Once completion is tracked, this will place markers only for completed stages.
                var completedStages = ArcWorldSystem.WorldGenIndicatorRunState.GetCompletedStagesInOrder();
                var stagesToShow = completedStages.Count > 0 ? completedStages : GetAllStagesInOrder();

                var layout = new ArcWorldGenIndicatorLayoutService();
                var placements = layout.BuildHubBoardPlacements(hub, stagesToShow);

                var placer = new ArcWorldGenIndicatorPlacer();
                _ = placer.TryPlaceIndicatorBoard(hub, placements);
            }
            catch
            {
                // Fail-safe: visual indicators must never block worldgen.
            }
        }

        private static IReadOnlyList<ArcWorldGenStage> GetAllStagesInOrder()
        {
            var stages = Enum.GetValues(typeof(ArcWorldGenStage)).Cast<ArcWorldGenStage>().ToList();
            stages.Sort((a, b) => ((int)a).CompareTo((int)b));
            return stages;
        }
    }
}
