using System;
using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.WorldGen
{
    public enum ArcReservedSiteKind
    {
        Unknown = 0,
        RaidTerminal = 1,
        Other = 2,
    }

    public sealed class ArcReservedSite : IEquatable<ArcReservedSite>
    {
        public ArcReservedSite(ArcReservedSiteKind kind, int x, int y, int radius = 0)
        {
            Kind = kind;
            X = x;
            Y = y;
            Radius = Math.Max(0, radius);
        }

        public ArcReservedSiteKind Kind { get; }
        public int X { get; }
        public int Y { get; }
        public int Radius { get; }

        public bool Equals(ArcReservedSite other)
        {
            if (other is null)
            {
                return false;
            }

            return Kind == other.Kind && X == other.X && Y == other.Y && Radius == other.Radius;
        }

        public override bool Equals(object obj) => obj is ArcReservedSite other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)Kind, X, Y, Radius);
    }

    public sealed class ArcWorldData
    {
        public const int CurrentDataVersion = 1;

        public bool IsArcWorld { get; set; }
        public int DataVersion { get; set; } = CurrentDataVersion;
        public IntRect SafeHubRegion { get; set; }
        public Dictionary<ArcRegionId, IntRect> Regions { get; } = new();
        public List<ArcReservedSite> ReservedSites { get; } = new();

        public static ArcWorldData NonArc() => new ArcWorldData
        {
            IsArcWorld = false,
            DataVersion = 0,
            SafeHubRegion = default,
        };
    }
}
