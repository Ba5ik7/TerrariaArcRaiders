using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Placeholder: will paint Arc biome tiles based on planned regions.
    internal class ArcStageD_BiomePainting : GenPass, IArcWorldGenPass
    {
        public ArcStageD_BiomePainting() : base("Arc Stage D - Biome Painting", 0.5f)
        {
        }

        public ArcWorldGenStage Stage => ArcWorldGenStage.StageD_BiomePainting;
        public GenPass AsGenPass() => this;

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc biome painting";
            ArcWorldGenLog.StageOrder("Stage D", "Biome Painting (placeholder)");
            // No-op placeholder; future passes will operate within planned rectangles to avoid full-world scans.
        }
    }
}
