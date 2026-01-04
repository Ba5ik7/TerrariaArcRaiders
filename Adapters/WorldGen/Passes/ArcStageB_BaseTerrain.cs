using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: future implementations will lay Arc-specific base terrain.
    internal class ArcStageB_BaseTerrain : GenPass
    {
        public ArcStageB_BaseTerrain() : base("Arc Stage B - Base Terrain", 0.5f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc base terrain";
            ArcWorldGenLog.StageOrder("Stage B", "Base Terrain Layout");
            // No-op placeholder to keep vanilla terrain until Arc terrain is defined.
        }
    }
}
