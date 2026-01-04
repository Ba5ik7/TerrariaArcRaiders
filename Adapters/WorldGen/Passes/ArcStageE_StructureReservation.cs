using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: reserve structure slots; currently relies on planner-provided reserved sites.
    internal class ArcStageE_StructureReservation : GenPass
    {
        public ArcStageE_StructureReservation() : base("Arc Stage E - Structure Reservation", 0.5f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc structure reservation";
            ArcWorldGenLog.StageOrder("Stage E", "Structure Reservation");
            // No-op placeholder; reserved sites already provided by planner.
            _ = ArcWorldSystem.WorldData?.ReservedSites;
        }
    }
}
