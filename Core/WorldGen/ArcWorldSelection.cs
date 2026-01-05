using System;

namespace TerrariaArcRaiders.Core.WorldGen
{
    public enum ArcWorldSelectionMode
    {
        None = 0,
        ArcRaider = 1,
    }

    public enum ArcWorldSelectionSource
    {
        Unknown = 0,
        SeedPrefix = 1,
        Other = 2,
    }

    public sealed class ArcWorldSelection
    {
        private ArcWorldSelection(ArcWorldSelectionMode mode, ArcWorldSelectionSource source, string rawSeedText)
        {
            Mode = mode;
            Source = source;
            RawSeedText = rawSeedText;
        }

        public ArcWorldSelectionMode Mode { get; }
        public ArcWorldSelectionSource Source { get; }
        public string RawSeedText { get; }

        public bool IsArcWorld => Mode == ArcWorldSelectionMode.ArcRaider;

        public static ArcWorldSelection FromSeedText(string? seedText)
        {
            if (string.IsNullOrWhiteSpace(seedText))
            {
                return new ArcWorldSelection(ArcWorldSelectionMode.None, ArcWorldSelectionSource.Unknown, string.Empty);
            }

            var trimmed = seedText.Trim();
            if (trimmed.StartsWith("arc:", StringComparison.OrdinalIgnoreCase))
            {
                return new ArcWorldSelection(ArcWorldSelectionMode.ArcRaider, ArcWorldSelectionSource.SeedPrefix, trimmed);
            }

            return new ArcWorldSelection(ArcWorldSelectionMode.None, ArcWorldSelectionSource.Unknown, trimmed);
        }
    }
}
