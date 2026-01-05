using System;
using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public sealed class ArcWorldGenIndicatorLayoutService
    {
        public IReadOnlyList<ArcWorldGenIndicatorPlacement> BuildHubBoardPlacements(IntRect hubRegion, IReadOnlyList<ArcWorldGenStage> stages)
        {
            if (!hubRegion.IsValid || stages == null || stages.Count == 0)
            {
                return Array.Empty<ArcWorldGenIndicatorPlacement>();
            }

            // Keep placements within the planned hub region to avoid needing world-size inputs and to ensure bounded work.
            // Layout: column-major grid starting near hub top-left (with padding), deterministic by stage order.
            var paddingX = 2;
            var paddingY = 2;
            var startX = hubRegion.X + paddingX;
            var startY = hubRegion.Y + paddingY;

            var usableHeight = Math.Max(1, hubRegion.Height - (paddingY * 2));
            var maxRows = Math.Max(1, usableHeight / 2);

            var placements = new List<ArcWorldGenIndicatorPlacement>(stages.Count);
            for (var index = 0; index < stages.Count; index++)
            {
                var stage = stages[index];
                var column = index / maxRows;
                var row = index % maxRows;

                var tileX = startX + (column * 3);
                var tileY = startY + (row * 2);

                // Clamp within hub bounds defensively.
                tileX = Clamp(tileX, hubRegion.X, Math.Max(hubRegion.X, hubRegion.Right - 1));
                tileY = Clamp(tileY, hubRegion.Y, Math.Max(hubRegion.Y, hubRegion.Bottom - 1));

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
