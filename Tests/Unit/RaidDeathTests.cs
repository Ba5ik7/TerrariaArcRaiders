using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class RaidDeathTests
    {
        private readonly RaidSessionService _sessions = new();
        private readonly StashService _stash = new();

        [TestMethod]
        public void DeathClearsRaidInventoryAndLeavesStashUntouched()
        {
            var session = _sessions.StartSession("player-1");
            _sessions.AddScrap(session, 7);
            var stash = new Stash();

            _sessions.Fail(session);
            _stash.Deposit(session, stash);

            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount, "Raid inventory should be zeroed on death.");
            Assert.AreEqual(0, stash.ArcScrapAmount, "Stash should not receive scrap on death.");
            Assert.AreEqual(RaidSessionStatus.Failed, session.Status);
        }

        [TestMethod]
        public void DeathIsIdempotentAndDoesNotDuplicate()
        {
            var session = _sessions.StartSession("player-1");
            _sessions.AddScrap(session, 4);

            _sessions.Fail(session);
            _sessions.Fail(session);

            var stash = new Stash();
            _stash.Deposit(session, stash);

            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(0, stash.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Failed, session.Status);
        }
    }
}
