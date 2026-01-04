using Terraria.IO;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    internal class ArcStageA_Setup : GenPass
    {
        public ArcStageA_Setup() : base("Arc Stage A - World Tagging & Setup", 0.5f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Arc setup";
            ArcWorldGenLog.StageOrder("Stage A", "World Tagging & Setup");

            // Ensure Arc world state is initialized with safe defaults.
            ArcWorldSystem.IsArcWorld = true;
            if (ArcWorldSystem.WorldData == null)
            {
                ArcWorldSystem.WorldData = ArcWorldData.NonArc();
            }

            ArcWorldSystem.WorldData.IsArcWorld = true;
            ArcWorldSystem.WorldData.DataVersion = ArcWorldData.CurrentDataVersion;
        }
    }
}
