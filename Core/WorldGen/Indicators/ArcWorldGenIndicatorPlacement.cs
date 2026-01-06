namespace TerrariaArcRaiders.Core.WorldGen.Indicators
{
    public readonly struct ArcWorldGenIndicatorPlacement
    {
        public ArcWorldGenIndicatorPlacement(ArcWorldGenStage stage, int tileX, int tileY, string label)
        {
            Stage = stage;
            TileX = tileX;
            TileY = tileY;
            Label = label ?? string.Empty;
        }

        public ArcWorldGenStage Stage { get; }
        public int TileX { get; }
        public int TileY { get; }
        public string Label { get; }
    }
}
