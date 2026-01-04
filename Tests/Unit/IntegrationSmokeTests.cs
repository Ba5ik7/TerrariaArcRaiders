using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class IntegrationSmokeTests
    {
        [TestMethod]
        public void EnterLootExtract_TransfersToStashAndClearsRaidInventory()
        {
            var sessionService = new RaidSessionService();
            var stashService = new StashService();
            var session = sessionService.StartSession("player-1");

            // Simulate loot in raid
            sessionService.AddScrap(session, 5);
            Assert.AreEqual(RaidSessionStatus.Active, session.Status);
            Assert.AreEqual(5, session.RaidInventory.ArcScrapAmount);

            // Extract and deposit
            var stash = new Stash();
            sessionService.Extract(session, stash);
            Assert.AreEqual(RaidSessionStatus.Extracted, session.Status);
            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(5, stash.ArcScrapAmount);

            // Repeat extract to confirm no duplication
            sessionService.Extract(session, stash);
            Assert.AreEqual(5, stash.ArcScrapAmount);
            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);

            // Any further loot attempts after extraction should be ignored
            sessionService.AddScrap(session, 3);
            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(5, stash.ArcScrapAmount);
        }
    }
}
