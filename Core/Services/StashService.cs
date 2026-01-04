using System;
using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Core.Services
{
    public sealed class StashService
    {
        public void Deposit(RaidSession session, Stash stash)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (stash == null) throw new ArgumentNullException(nameof(stash));

            var amount = session.RaidInventory.ArcScrapAmount;
            if (amount <= 0)
            {
                return;
            }

            stash.DepositScrap(amount);
            session.RaidInventory.Clear();
        }

        public bool TryWithdraw(Stash stash, int amount)
        {
            if (stash == null) throw new ArgumentNullException(nameof(stash));
            return stash.TryWithdrawScrap(amount);
        }
    }
}
