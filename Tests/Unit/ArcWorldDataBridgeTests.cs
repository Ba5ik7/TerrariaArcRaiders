using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Core.Services;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class ArcWorldDataBridgeTests
    {
        [TestMethod]
        public void To_and_from_dto_round_trips_arc_world_data()
        {
            var data = new ArcWorldData
            {
                IsArcWorld = true,
                DataVersion = 3,
                SafeHubRegion = new IntRect(10, 20, 30, 40),
            };
            data.Regions[ArcRegionId.SafeHub] = new IntRect(10, 20, 30, 40);
            data.Regions[ArcRegionId.ArcWasteland] = new IntRect(100, 120, 50, 60);
            data.ReservedSites.Add(new ArcReservedSite(ArcReservedSiteKind.RaidTerminal, 200, 210, 5));

            var dto = ArcWorldDataBridge.ToTagCompoundDto(data);
            var roundTripped = ArcWorldDataBridge.FromTagCompoundDto(dto);

            Assert.IsTrue(roundTripped.IsArcWorld);
            Assert.AreEqual(data.DataVersion, roundTripped.DataVersion);
            Assert.AreEqual(data.SafeHubRegion, roundTripped.SafeHubRegion);
            CollectionAssert.AreEquivalent(data.Regions.Keys.ToList(), roundTripped.Regions.Keys.ToList());
            CollectionAssert.AreEquivalent(data.ReservedSites.Select(s => s.Kind).ToList(), roundTripped.ReservedSites.Select(s => s.Kind).ToList());
            Assert.AreEqual(data.ReservedSites[0].Radius, roundTripped.ReservedSites[0].Radius);
        }

        [TestMethod]
        public void FromTagCompoundDto_defaults_to_non_arc_when_missing()
        {
            var data = ArcWorldDataBridge.FromTagCompoundDto(null);

            Assert.IsFalse(data.IsArcWorld);
        }

        [TestMethod]
        public void FromTagCompoundDto_defaults_to_non_arc_when_safe_hub_missing()
        {
            var dto = new TagCompoundDto();
            dto.SetInt("isArc", 1);
            dto.SetInt("dataVersion", 1);

            var data = ArcWorldDataBridge.FromTagCompoundDto(dto);

            Assert.IsFalse(data.IsArcWorld);
        }

        [TestMethod]
        public void Reserved_sites_clamp_radius_and_skip_unknown_kind()
        {
            var dto = new TagCompoundDto();
            dto.SetInt("isArc", 1);
            dto.SetInt("dataVersion", 1);

            var safeHub = new TagCompoundDto();
            safeHub.SetInt("x", 0);
            safeHub.SetInt("y", 0);
            safeHub.SetInt("width", 10);
            safeHub.SetInt("height", 10);
            dto.SetCompound("safeHub", safeHub);

            var sites = new TagCompoundDto();
            var validSite = new TagCompoundDto();
            validSite.SetString("kind", ArcReservedSiteKind.RaidTerminal.ToString());
            validSite.SetInt("x", 5);
            validSite.SetInt("y", 6);
            validSite.SetInt("radius", -4);
            sites.SetCompound("0", validSite);

            var unknownSite = new TagCompoundDto();
            unknownSite.SetString("kind", "UnknownKind");
            unknownSite.SetInt("x", 1);
            unknownSite.SetInt("y", 1);
            sites.SetCompound("1", unknownSite);

            dto.SetCompound("reservedSites", sites);

            var data = ArcWorldDataBridge.FromTagCompoundDto(dto);

            Assert.IsTrue(data.IsArcWorld);
            Assert.AreEqual(1, data.ReservedSites.Count);
            Assert.AreEqual(0, data.ReservedSites[0].Radius, "Negative radius should clamp to zero.");
        }
    }
}
