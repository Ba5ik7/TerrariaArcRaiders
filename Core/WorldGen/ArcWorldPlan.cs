using System.Collections.Generic;

namespace TerrariaArcRaiders.Core.WorldGen
{
    public sealed class ArcWorldPlan
    {
        public ArcWorldPlan(IntRect safeHubRegion, Dictionary<ArcRegionId, IntRect> regions, List<ArcReservedSite> reservedSites)
        {
            SafeHubRegion = safeHubRegion;
            Regions = regions ?? new Dictionary<ArcRegionId, IntRect>();
            ReservedSites = reservedSites ?? new List<ArcReservedSite>();
        }

        public IntRect SafeHubRegion { get; }
        public Dictionary<ArcRegionId, IntRect> Regions { get; }
        public List<ArcReservedSite> ReservedSites { get; }
    }
}
