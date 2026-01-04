using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: future content will place Arc structures at reserved sites.
    internal class ArcStageF_StructurePlacement : GenPass, IArcWorldGenPass
    {
        public ArcStageF_StructurePlacement() : base("Arc Stage F - Structure Placement", 0.5f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageF_StructurePlacement;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc structure placement";
            ArcWorldGenLog.StageOrder("Stage F", "Structure Placement (placeholder)");
            // No-op placeholder to maintain stage order visibility; avoid any tile loops until structure logic exists.
        }
    }
}
