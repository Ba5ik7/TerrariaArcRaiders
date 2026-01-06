using System;
using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public sealed class ArcWorldGenIndicatorLayoutService
    {
        public IReadOnlyList<ArcWorldGenIndicatorPlacement> BuildHubBoardPlacements(IntRect hubRegion, IReadOnlyList<ArcWorldGenStage> stages)
        {
            // Backwards-compatible overload when world bounds are not available yet.
            return BuildHubBoardPlacements(hubRegion, stages, worldWidth: 0, worldHeight: 0);
        }

        public IReadOnlyList<ArcWorldGenIndicatorPlacement> BuildHubBoardPlacements(IntRect hubRegion, IReadOnlyList<ArcWorldGenStage> stages, int worldWidth, int worldHeight)
        {
            if (!hubRegion.IsValid || stages == null || stages.Count == 0)
            {
                return Array.Empty<ArcWorldGenIndicatorPlacement>();
            }

            // Keep placements within the planned hub region to avoid needing world-size inputs and to ensure bounded work.
            // Layout: column-major grid starting near hub top-left (with padding), deterministic by stage order.
            // Spacing is derived from marker footprint so markers do not overlap.
            var startX = hubRegion.X + ArcWorldGenIndicatorConstants.BoardPaddingTiles + ArcWorldGenIndicatorConstants.FramePaddingTiles;
            var startY = hubRegion.Y + ArcWorldGenIndicatorConstants.BoardPaddingTiles + ArcWorldGenIndicatorConstants.FramePaddingTiles;

            var stepX = ArcWorldGenIndicatorConstants.MarkerSizeTiles + ArcWorldGenIndicatorConstants.MarkerSpacingTiles;
            var stepY = ArcWorldGenIndicatorConstants.MarkerSizeTiles + ArcWorldGenIndicatorConstants.MarkerSpacingTiles;

            var usableHeight = Math.Max(1, hubRegion.Height - ((ArcWorldGenIndicatorConstants.BoardPaddingTiles + ArcWorldGenIndicatorConstants.FramePaddingTiles) * 2));
            var maxRows = Math.Max(1, usableHeight / Math.Max(1, stepY));

            var placements = new List<ArcWorldGenIndicatorPlacement>(stages.Count);
            for (var index = 0; index < stages.Count; index++)
            {
                var stage = stages[index];
                var column = index / maxRows;
                var row = index % maxRows;

                // tileX/tileY are the top-left of the marker base.
                var tileX = startX + (column * stepX);
                var tileY = startY + (row * stepY);

                // Clamp within hub bounds defensively, accounting for marker footprint.
                var minBaseX = hubRegion.X + ArcWorldGenIndicatorConstants.FramePaddingTiles;
                var maxBaseX = hubRegion.Right - ArcWorldGenIndicatorConstants.BaseSizeTiles - ArcWorldGenIndicatorConstants.FramePaddingTiles;
                var minBaseY = hubRegion.Y + ArcWorldGenIndicatorConstants.FramePaddingTiles;
                var maxBaseY = hubRegion.Bottom - ArcWorldGenIndicatorConstants.BaseSizeTiles - ArcWorldGenIndicatorConstants.FramePaddingTiles;

                tileX = Clamp(tileX, minBaseX, Math.Max(minBaseX, maxBaseX));
                tileY = Clamp(tileY, minBaseY, Math.Max(minBaseY, maxBaseY));

                // Optionally clamp to world bounds when provided.
                if (worldWidth > 0)
                {
                    tileX = Clamp(tileX, 0, Math.Max(0, worldWidth - 1));
                }

                if (worldHeight > 0)
                {
                    tileY = Clamp(tileY, 0, Math.Max(0, worldHeight - 1));
                }

                placements.Add(new ArcWorldGenIndicatorPlacement(stage, tileX, tileY, ArcWorldGenIndicatorLegend.GetLabel(stage)));
            }

            return placements;
        }

        private static int Clamp(int value, int minInclusive, int maxInclusive)
        {
            if (value < minInclusive)
            {
                return minInclusive;
            }

            return value > maxInclusive ? maxInclusive : value;
        }
    }
}
