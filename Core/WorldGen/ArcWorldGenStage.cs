namespace TerrariaArcRaiders.Core.WorldGen
{
    public enum ArcWorldGenStage
    {
        StageA_Setup = 0,
        StageB_BaseTerrain = 1,
        StageC_RegionPlanning = 2,
        StageD_BiomePainting = 3,
        StageE_StructureReservation = 4,
        StageF_StructurePlacement = 5,
        StageG_RaidAnchors = 6,
        StageH_FinalValidation = 7,

        // Debug-only stage markers should use values outside the main pipeline range so they don't affect ordering.
        StageZ_TestMarker = 100,
    }
}
