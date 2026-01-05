using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public static class ArcWorldGenIndicatorLegend
    {
        private static readonly Dictionary<ArcWorldGenStage, string> StageLabels = new()
        {
            [ArcWorldGenStage.StageA_Setup] = "Stage A: Setup",
            [ArcWorldGenStage.StageB_BaseTerrain] = "Stage B: Base Terrain",
            [ArcWorldGenStage.StageC_RegionPlanning] = "Stage C: Region Planning",
            [ArcWorldGenStage.StageD_BiomePainting] = "Stage D: Biome Painting",
            [ArcWorldGenStage.StageE_StructureReservation] = "Stage E: Structure Reservation",
            [ArcWorldGenStage.StageF_StructurePlacement] = "Stage F: Structure Placement",
            [ArcWorldGenStage.StageG_RaidAnchors] = "Stage G: Raid Anchors",
            [ArcWorldGenStage.StageH_FinalValidation] = "Stage H: Final Validation",
        };

        public static IReadOnlyDictionary<ArcWorldGenStage, string> GetStageLabels()
        {
            return StageLabels;
        }

        public static string GetLabel(ArcWorldGenStage stage)
        {
            return StageLabels.TryGetValue(stage, out var label) ? label : stage.ToString();
        }
    }
}
