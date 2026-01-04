using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class ArcWorldPlanServiceTests
    {
        private readonly ArcWorldPlanService _service = new();

        [TestMethod]
        public void BuildPlan_is_deterministic_for_same_inputs()
        {
            var planA = _service.BuildPlan(8400, 2400, "arc:seed-1");
            var planB = _service.BuildPlan(8400, 2400, "arc:seed-1");

            Assert.AreEqual(planA.SafeHubRegion, planB.SafeHubRegion);
            CollectionAssert.AreEquivalent(planA.Regions.Keys.ToList(), planB.Regions.Keys.ToList());

            foreach (var key in planA.Regions.Keys)
            {
                Assert.AreEqual(planA.Regions[key], planB.Regions[key]);
            }

            Assert.AreEqual(planA.ReservedSites.Count, planB.ReservedSites.Count);
            for (var i = 0; i < planA.ReservedSites.Count; i++)
            {
                var siteA = planA.ReservedSites[i];
                var siteB = planB.ReservedSites[i];
                Assert.AreEqual(siteA.Kind, siteB.Kind);
                Assert.AreEqual(siteA.X, siteB.X);
                Assert.AreEqual(siteA.Y, siteB.Y);
                Assert.AreEqual(siteA.Radius, siteB.Radius);
            }
        }

        [TestMethod]
        public void BuildPlan_produces_safe_hub_and_extra_region_within_bounds()
        {
            const int width = 6000;
            const int height = 1800;

            var plan = _service.BuildPlan(width, height, "arc:seed-2");

            Assert.IsTrue(plan.SafeHubRegion.IsValid);
            Assert.IsTrue(plan.Regions.ContainsKey(ArcRegionId.SafeHub));
            Assert.IsTrue(plan.Regions.ContainsKey(ArcRegionId.ArcWasteland));

            Assert.IsTrue(plan.SafeHubRegion.X >= 0 && plan.SafeHubRegion.Right <= width);
            Assert.IsTrue(plan.SafeHubRegion.Y >= 0 && plan.SafeHubRegion.Bottom <= height);

            var wasteland = plan.Regions[ArcRegionId.ArcWasteland];
            Assert.IsTrue(wasteland.IsValid);
            Assert.IsTrue(wasteland.X >= 0 && wasteland.Right <= width);
            Assert.IsTrue(wasteland.Y >= 0 && wasteland.Bottom <= height);

            Assert.IsTrue(plan.ReservedSites.Count >= 1);
            var hubSite = plan.ReservedSites[0];
            Assert.AreEqual(ArcReservedSiteKind.RaidTerminal, hubSite.Kind);
            Assert.IsTrue(hubSite.X >= plan.SafeHubRegion.X && hubSite.X <= plan.SafeHubRegion.Right);
            Assert.IsTrue(hubSite.Y >= plan.SafeHubRegion.Y && hubSite.Y <= plan.SafeHubRegion.Bottom);
        }

        [TestMethod]
        public void BuildPlan_varies_with_seed()
        {
            var planA = _service.BuildPlan(5000, 1400, "arc:seed-3");
            var planB = _service.BuildPlan(5000, 1400, "arc:seed-4");

            var sameHub = planA.SafeHubRegion.Equals(planB.SafeHubRegion);
            var sameWasteland = planA.Regions[ArcRegionId.ArcWasteland].Equals(planB.Regions[ArcRegionId.ArcWasteland]);

            Assert.IsFalse(sameHub && sameWasteland, "Different seeds should alter planned regions deterministically.");
        }
    }
}
