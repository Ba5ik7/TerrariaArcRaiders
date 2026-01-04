using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class RaidPersistenceTests
    {
        private readonly RaidPersistence _persistence = new();

        [TestMethod]
        public void Save_and_load_stash_and_session_round_trip()
        {
            var stash = new Stash();
            stash.DepositScrap(10);

            var session = new RaidSession("player-1");
            session.RaidInventory.AddScrap(3);
            session.SetStatus(RaidSessionStatus.Active);

            var dto = _persistence.Save(stash, session);

            var loadedStash = new Stash();
            _persistence.Load(dto, loadedStash, out var loadedSession);

            Assert.AreEqual(10, loadedStash.ArcScrapAmount);
            Assert.IsNotNull(loadedSession);
            Assert.AreEqual("player-1", loadedSession!.PlayerId);
            Assert.AreEqual(3, loadedSession.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Active, loadedSession.Status);
        }

        [TestMethod]
        public void Load_handles_missing_data_with_defaults()
        {
            var stash = new Stash();
            _persistence.Load(null, stash, out var session);

            Assert.AreEqual(0, stash.ArcScrapAmount);
            Assert.IsNull(session);
        }

        [TestMethod]
        public void Load_ignores_corrupt_session_status()
        {
            var root = new TagCompoundDto();
            root.SetInt("stash", 5);
            var sessionCompound = new TagCompoundDto();
            sessionCompound.SetString("playerId", "player-1");
            sessionCompound.SetString("status", "NOT_A_STATUS");
            sessionCompound.SetInt("scrap", 2);
            root.SetCompound("session", sessionCompound);

            var stash = new Stash();
            _persistence.Load(root, stash, out var session);

            Assert.AreEqual(5, stash.ArcScrapAmount);
            Assert.IsNull(session);
        }
    }
}
