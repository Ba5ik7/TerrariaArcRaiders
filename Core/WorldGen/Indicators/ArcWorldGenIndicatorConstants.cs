namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public static class ArcWorldGenIndicatorConstants
    {
        // "Overkill" marker sizing to make stage indicators highly visible in-game.
        public const int BaseSizeTiles = 11;        // square base size (NxN)
        public const int FramePaddingTiles = 2;     // glass frame padding around the base
        public const int MarkerSpacingTiles = 2;    // spacing between markers on the board
        public const int BoardPaddingTiles = 2;     // padding from hub rect edge

        public const int MarkerSizeTiles = BaseSizeTiles + (FramePaddingTiles * 2);
    }
}

