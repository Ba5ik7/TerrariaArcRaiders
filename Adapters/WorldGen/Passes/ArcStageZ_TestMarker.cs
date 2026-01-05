using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
#if DEBUG
    // Debug-only marker to demonstrate how to add a new Arc worldgen pass at a stage boundary.
    internal sealed class ArcStageZ_TestMarker : GenPass, IArcWorldGenPass
    {
        public ArcStageZ_TestMarker() : base("Arc Stage Z - Test Marker", 0.1f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageD_BiomePainting;

        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc test marker";
            ArcWorldGenLog.StageOrder("Stage Z", "Debug test marker");
        }
    }
#endif
}