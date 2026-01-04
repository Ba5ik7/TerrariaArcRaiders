using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: future content will place Arc structures at reserved sites.
    internal class ArcStageF_StructurePlacement : GenPass
    {
        public ArcStageF_StructurePlacement() : base("Arc Stage F - Structure Placement", 0.5f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc structure placement";
            ArcWorldGenLog.StageOrder("Stage F", "Structure Placement (placeholder)");
            // No-op placeholder to maintain stage order visibility.
        }
    }
}
