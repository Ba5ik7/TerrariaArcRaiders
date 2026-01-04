using System;

namespace TerrariaArcRaiders.Core.Models
{
    public sealed class RaidInventory
    {
        public int ArcScrapAmount { get; private set; }

        public void AddScrap(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
            }

            ArcScrapAmount = checked(ArcScrapAmount + amount);
        }

        public void Clear()
        {
            ArcScrapAmount = 0;
        }
    }
}
