using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: reserve structure slots; currently relies on planner-provided reserved sites.
    internal class ArcStageE_StructureReservation : GenPass, IArcWorldGenPass
    {
        public ArcStageE_StructureReservation() : base("Arc Stage E - Structure Reservation", 0.5f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageE_StructureReservation;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc structure reservation";
            ArcWorldGenLog.StageOrder("Stage E", "Structure Reservation");
            // No-op placeholder; reserved sites already provided by planner (bounded list, no scans).
            _ = ArcWorldSystem.WorldData?.ReservedSites;
        }
    }
}
