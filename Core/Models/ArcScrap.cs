namespace TerrariaArcRaiders.Core.Models
{
    public sealed class ArcScrap
    {
        public const string ResourceId = "arc_scrap";
        public const int MaxStack = 999;
        public const int DefaultValue = 1;

        public static bool IsValidQuantity(int amount)
        {
            return amount >= 0 && amount <= MaxStack;
        }
    }
}
