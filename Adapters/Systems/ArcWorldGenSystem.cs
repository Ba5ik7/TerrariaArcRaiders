using System.Collections.Generic;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.WorldGen;
using TerrariaArcRaiders.Adapters.WorldGen.Passes;

namespace TerrariaArcRaiders.Adapters.Systems
{
    // Wires Arc worldgen stages into Terraria's worldgen pipeline for Arc worlds only.
    public class ArcWorldGenSystem : ModSystem
    {
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            if (!ArcWorldSystem.IsArcWorld)
            {
                return;
            }

            var pipeline = ArcWorldGenPipeline.CreateWithDefaultPasses();
            var arcPasses = pipeline.BuildOrderedPasses();

            // Prepend Arc passes to run before vanilla tasks while keeping vanilla tasks for now (placeholder layout).
            tasks.InsertRange(0, arcPasses);
            foreach (var pass in arcPasses)
            {
                totalWeight += pass.Weight;
            }
        }
    }
}
