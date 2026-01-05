using System.Collections.Generic;
using Terraria;
using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    internal class ArcStageC_RegionPlanning : GenPass, IArcWorldGenPass
    {
        public ArcStageC_RegionPlanning() : base("Arc Stage C - Region Planning", 0.75f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageC_RegionPlanning;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc region planning";
            ArcWorldGenLog.StageOrder("Stage C", "Region Planning");

            var planner = new ArcWorldPlanService();
            var plan = planner.BuildPlan(Main.maxTilesX, Main.maxTilesY, ArcWorldSystem.Selection.RawSeedText);

            var data = ArcWorldSystem.WorldData ?? ArcWorldData.NonArc();
            data.IsArcWorld = true;
            data.DataVersion = ArcWorldData.CurrentDataVersion;
            data.SafeHubRegion = plan.SafeHubRegion;

            data.Regions.Clear();
            foreach (var pair in plan.Regions)
            {
                data.Regions[pair.Key] = pair.Value;
            }

            data.ReservedSites.Clear();
            data.ReservedSites.AddRange(plan.ReservedSites);

            ArcWorldSystem.WorldData = data;
            // Planner already bounds rectangles; no full-world scans are performed here.
        }
    }
}
