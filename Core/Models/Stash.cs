using System;

namespace TerrariaArcRaiders.Core.Models
{
    public sealed class Stash
    {
        public int ArcScrapAmount { get; private set; }

        public void DepositScrap(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
            }

            ArcScrapAmount = checked(ArcScrapAmount + amount);
        }

        public bool TryWithdrawScrap(int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
            }

            if (amount > ArcScrapAmount)
            {
                return false;
            }

            ArcScrapAmount -= amount;
            return true;
        }

        public void Clear()
        {
            ArcScrapAmount = 0;
        }
    }
}
