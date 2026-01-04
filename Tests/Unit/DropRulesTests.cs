using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class DropRulesTests
    {
        private readonly DropRules _dropRules = new();
        private readonly RaidSessionService _service = new();

        [TestMethod]
        public void AwardDroneScrap_adds_one_when_session_active()
        {
            var session = _service.StartSession("player-1");
            _service.ActivateSession(session);

            _dropRules.AwardDroneScrap(session);

            Assert.AreEqual(1, session.RaidInventory.ArcScrapAmount);
        }

        [TestMethod]
        public void AwardDroneScrap_ignored_when_session_finished()
        {
            var session = _service.StartSession("player-1");
            _service.Extract(session, new Stash());

            _dropRules.AwardDroneScrap(session);

            Assert.AreEqual(0, session.RaidInventory.ArcScrapAmount);
        }
    }
}
