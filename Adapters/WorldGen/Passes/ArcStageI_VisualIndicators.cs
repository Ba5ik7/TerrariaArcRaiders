using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.IO;
using Terraria.ID;
using Terraria.WorldBuilding;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Adapters.WorldGen.Indicators;
using TerrariaArcRaiders.Core.WorldGen;
using TerrariaArcRaiders.Core.WorldGen.Indicators;
using TWorldGen = Terraria.WorldGen;

namespace TerrariaArcRaiders.Adapters.WorldGen.Passes
{
    // Places in-world (vanilla-safe) visual markers for Arc worldgen stages.
    // This pass is intended to run AFTER vanilla worldgen tasks so markers are not overwritten.
    internal sealed class ArcStageI_VisualIndicators : GenPass
    {
        public ArcStageI_VisualIndicators() : base("Arc Stage I - Visual Indicators", 0.1f)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            if (!ArcWorldSystem.IsArcWorld)
            {
                return;
            }

            var config = ArcRaidersConfig.Instance;
            if (config == null || !config.WorldGenVisualIndicatorsEnabled)
            {
                return;
            }

            progress.Message = "Arc visual indicators";

            try
            {
                var data = ArcWorldSystem.WorldData;
                if (data == null || !data.SafeHubRegion.IsValid)
                {
                    return;
                }

                var hub = data.SafeHubRegion;

                // Until per-stage completion marking is wired (T014+), fall back to "all stages".
                // Once completion is tracked, this will place markers only for completed stages.
                var completedStages = ArcWorldSystem.WorldGenIndicatorRunState.GetCompletedStagesInOrder();
                var stagesToShow = completedStages.Count > 0 ? completedStages : GetAllStagesInOrder();

                var layout = new ArcWorldGenIndicatorLayoutService();
                var placements = layout.BuildHubBoardPlacements(hub, stagesToShow, worldWidth: Main.maxTilesX, worldHeight: Main.maxTilesY);

                var placer = new ArcWorldGenIndicatorPlacer();
                _ = placer.TryPlaceIndicatorBoard(hub, placements);

                TryPlaceReservedSiteMarker(data, hub);
            }
            catch
            {
                // Fail-safe: visual indicators must never block worldgen.
            }
        }

        private static void TryPlaceReservedSiteMarker(ArcWorldData data, IntRect hub)
        {
            if (data?.ReservedSites == null || data.ReservedSites.Count == 0)
            {
                return;
            }

            var site = data.ReservedSites[0];
            if (site == null)
            {
                return;
            }

            // Deterministic, bounded local search for an empty tile near the planned site.
            // We only place a small, vanilla-safe marker (a gold brick + torch) and never clear tiles.
            var searchRadius = Math.Max(2, Math.Min(10, site.Radius + 2));

            for (var dy = -searchRadius; dy <= searchRadius; dy++)
            {
                for (var dx = -searchRadius; dx <= searchRadius; dx++)
                {
                    var x = site.X + dx;
                    var y = site.Y + dy;

                    if (!hub.IsValid || x < hub.X || x >= hub.Right || y < hub.Y || y >= hub.Bottom)
                    {
                        continue;
                    }

                    if (!TWorldGen.InWorld(x, y, 10) || !TWorldGen.InWorld(x, y - 1, 10))
                    {
                        continue;
                    }

                    var baseTile = Framing.GetTileSafely(x, y);
                    var torchTile = Framing.GetTileSafely(x, y - 1);
                    if (baseTile.HasTile || torchTile.HasTile)
                    {
                        continue;
                    }

                    if (!TWorldGen.PlaceTile(x, y, TileID.GoldBrick, mute: true, forced: false, plr: -1, style: 0))
                    {
                        continue;
                    }

                    _ = TWorldGen.PlaceTile(x, y - 1, TileID.Torches, mute: true, forced: false, plr: -1, style: 0);
                    TWorldGen.SquareTileFrame(x, y);
                    TWorldGen.SquareTileFrame(x, y - 1);
                    return;
                }
            }
        }

        private static IReadOnlyList<ArcWorldGenStage> GetAllStagesInOrder()
        {
            var stages = Enum.GetValues(typeof(ArcWorldGenStage)).Cast<ArcWorldGenStage>().ToList();
            stages.Sort((a, b) => ((int)a).CompareTo((int)b));
            return stages;
        }
    }
}
