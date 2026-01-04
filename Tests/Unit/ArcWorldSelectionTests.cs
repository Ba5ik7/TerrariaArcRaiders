using Microsoft.VisualStudio.TestTools.UnitTesting;
using TerrariaArcRaiders.Core.WorldGen;

namespace TerrariaArcRaiders.Tests.Unit
{
    [TestClass]
    public class ArcWorldSelectionTests
    {
        [TestMethod]
        public void FromSeedText_detects_arc_prefix_case_insensitive()
        {
            var selection = ArcWorldSelection.FromSeedText("ARC:my-seed");

            Assert.IsTrue(selection.IsArcWorld);
            Assert.AreEqual(ArcWorldSelectionMode.ArcRaider, selection.Mode);
            Assert.AreEqual(ArcWorldSelectionSource.SeedPrefix, selection.Source);
            Assert.AreEqual("ARC:my-seed", selection.RawSeedText);
        }

        [TestMethod]
        public void FromSeedText_returns_non_arc_for_other_seeds()
        {
            var selection = ArcWorldSelection.FromSeedText("normal-seed");

            Assert.IsFalse(selection.IsArcWorld);
            Assert.AreEqual(ArcWorldSelectionMode.None, selection.Mode);
            Assert.AreEqual(ArcWorldSelectionSource.Unknown, selection.Source);
            Assert.AreEqual("normal-seed", selection.RawSeedText);
        }

        [TestMethod]
        public void FromSeedText_defaults_to_none_when_empty()
        {
            var selection = ArcWorldSelection.FromSeedText(string.Empty);

            Assert.IsFalse(selection.IsArcWorld);
            Assert.AreEqual(ArcWorldSelectionMode.None, selection.Mode);
            Assert.AreEqual(ArcWorldSelectionSource.Unknown, selection.Source);
            Assert.AreEqual(string.Empty, selection.RawSeedText);
        }
    }
}
