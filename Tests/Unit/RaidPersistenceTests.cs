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

        [TestMethod]
        public void Load_clamps_negative_values_and_preserves_status()
        {
            var root = new TagCompoundDto();
            root.SetInt("stash", -12);
            var sessionCompound = new TagCompoundDto();
            sessionCompound.SetString("playerId", "player-1");
            sessionCompound.SetString("status", RaidSessionStatus.Active.ToString());
            sessionCompound.SetInt("scrap", -4);
            root.SetCompound("session", sessionCompound);

            var stash = new Stash();
            _persistence.Load(root, stash, out var session);

            Assert.AreEqual(0, stash.ArcScrapAmount, "Negative stash should clamp to zero.");
            Assert.IsNotNull(session);
            Assert.AreEqual(0, session!.RaidInventory.ArcScrapAmount, "Negative scrap should clamp to zero.");
            Assert.AreEqual(RaidSessionStatus.Active, session.Status);
        }

        [TestMethod]
        public void Load_handles_missing_stash_but_session_present()
        {
            var root = new TagCompoundDto();
            var sessionCompound = new TagCompoundDto();
            sessionCompound.SetString("playerId", "player-1");
            sessionCompound.SetString("status", RaidSessionStatus.Extracted.ToString());
            sessionCompound.SetInt("scrap", 2);
            root.SetCompound("session", sessionCompound);

            var stash = new Stash();
            _persistence.Load(root, stash, out var session);

            Assert.AreEqual(0, stash.ArcScrapAmount, "Missing stash key should default to zero.");
            Assert.IsNotNull(session);
            Assert.AreEqual(2, session!.RaidInventory.ArcScrapAmount);
            Assert.AreEqual(RaidSessionStatus.Extracted, session.Status);
        }
    }
}
