using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class RaidSessionServiceTests
    {
        private readonly RaidSessionService _service = new();
        private readonly StashService _stashService = new();

        [TestMethod]
        public void Extract_moves_all_scrap_to_stash_and_clears_inventory()
        {
            var session = _service.StartSession("player-1");
            _service.AddScrap(session, 5);

            var stash = new Stash();
            _service.Extract(session, stash);

            Assert.AreEqual(5, stash.ArcScrapAmount);
            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Extracted, session.Status);
        }

        [TestMethod]
        public void Death_clears_inventory_and_leaves_stash_unchanged()
        {
            var session = _service.StartSession("player-1");
            _service.AddScrap(session, 3);

            var stash = new Stash();
            _service.Fail(session);
            _stashService.Deposit(session, stash);

            Assert.AreEqual(0, stash.ArcScrapAmount);
            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Failed, session.Status);
        }

        [TestMethod]
        public void Adding_scrap_is_ignored_when_session_finished()
        {
            var session = _service.StartSession("player-1");
            _service.Extract(session, new Stash());

            _service.AddScrap(session, 2);

            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Extracted, session.Status);
        }

        [TestMethod]
        public void Inventory_can_be_cleared_after_adds()
        {
            var session = _service.StartSession("player-1");
            _service.AddScrap(session, 2);
            _service.AddScrap(session, 3);

            session.ResetInventory();

            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
        }
    }
}
