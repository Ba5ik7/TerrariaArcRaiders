using System;
using System.Collections.Generic;
using System.Text;

namespace TerrariaArcRaiders.Core.WorldGen
{
    // Deterministic, tModLoader-free planner for Arc world regions and reserved sites.
    public sealed class ArcWorldPlanService
    {
        private const int MinHubWidth = 48;
        private const int MinHubHeight = 32;
        private const int MinRegionWidth = 96;
        private const int MinRegionHeight = 48;

        public ArcWorldPlan BuildPlan(int worldWidth, int worldHeight, string seedText)
        {
            var width = Math.Max(worldWidth, 200);
            var height = Math.Max(worldHeight, 200);

            var seed = ComposeSeed(seedText, width, height);
            var rng = new Random(seed);

            var safeHub = PlanSafeHub(width, height, rng);
            var regions = new Dictionary<ArcRegionId, IntRect>
            {
                [ArcRegionId.SafeHub] = safeHub,
            };

            var extraRegion = PlanExtraRegion(width, height, safeHub, rng);
            regions[extraRegion.regionId] = extraRegion.rect;

            var reservedSites = new List<ArcReservedSite>
            {
                new ArcReservedSite(ArcReservedSiteKind.RaidTerminal, safeHub.X + safeHub.Width / 2, safeHub.Y + safeHub.Height / 2, radius: 6),
            };

            return new ArcWorldPlan(safeHub, regions, reservedSites);
        }

        private static IntRect PlanSafeHub(int worldWidth, int worldHeight, Random rng)
        {
            var hubWidth = Math.Max(MinHubWidth, worldWidth / 16);
            var hubHeight = Math.Max(MinHubHeight, worldHeight / 18);

            var centerX = worldWidth / 2 + rng.Next(-worldWidth / 20, worldWidth / 20 + 1);
            var centerY = worldHeight / 3 + rng.Next(-worldHeight / 30, worldHeight / 30 + 1);

            var x = Clamp(centerX - hubWidth / 2, 0, worldWidth - hubWidth);
            var y = Clamp(centerY - hubHeight / 2, 0, worldHeight - hubHeight);

            return new IntRect(x, y, hubWidth, hubHeight);
        }

        private static (ArcRegionId regionId, IntRect rect) PlanExtraRegion(int worldWidth, int worldHeight, IntRect hub, Random rng)
        {
            var regionWidth = Math.Max(MinRegionWidth, worldWidth / 12);
            var regionHeight = Math.Max(MinRegionHeight, worldHeight / 14);

            var horizontalOffset = hub.Width + rng.Next(regionWidth / 3, regionWidth);
            var placeRight = rng.NextDouble() > 0.5;
            var centerX = placeRight ? hub.Right + horizontalOffset : hub.X - horizontalOffset;
            var centerY = hub.Y + hub.Height / 2 + rng.Next(-regionHeight / 3, regionHeight / 3 + 1);

            centerX = Clamp(centerX, regionWidth / 2, worldWidth - regionWidth / 2);
            centerY = Clamp(centerY, regionHeight / 2, worldHeight - regionHeight / 2);

            var x = centerX - regionWidth / 2;
            var y = centerY - regionHeight / 2;

            var rect = new IntRect(x, y, regionWidth, regionHeight);
            return (ArcRegionId.ArcWasteland, rect);
        }

        private static int ComposeSeed(string seedText, int width, int height)
        {
            var baseSeed = StableHash(seedText ?? string.Empty);
            return unchecked((int)(baseSeed ^ (uint)(width * 397) ^ (uint)(height * 857))); // mix dimensions to avoid collisions
        }

        private static uint StableHash(string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            const uint offset = 2166136261;
            const uint prime = 16777619;

            uint hash = offset;
            foreach (var b in bytes)
            {
                hash ^= b;
                hash *= prime;
            }

            return hash;
        }

        private static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                return min;
            }

            return value > max ? max : value;
        }
    }
}
