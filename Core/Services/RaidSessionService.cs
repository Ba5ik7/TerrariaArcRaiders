using System;
using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Core.Services
{
    public sealed class RaidSessionService
    {
        public RaidSession StartSession(string playerId)
        {
            return new RaidSession(playerId);
        }

        public void ActivateSession(RaidSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            session.SetStatus(RaidSessionStatus.Active);
        }

        public void AddScrap(RaidSession session, int amount)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (session.Status == RaidSessionStatus.Unknown || session.Status == RaidSessionStatus.Failed || session.Status == RaidSessionStatus.Extracted)
            {
                return;
            }

            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be non-negative.");
            }

            session.RaidInventory.AddScrap(amount);
            session.SetStatus(session.Status == RaidSessionStatus.Entered ? RaidSessionStatus.Active : session.Status);
        }

        public void Extract(RaidSession session, Stash stash)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (stash == null) throw new ArgumentNullException(nameof(stash));

            if (session.Status == RaidSessionStatus.Unknown || session.Status == RaidSessionStatus.Failed)
            {
                return;
            }

            var scrap = session.RaidInventory.ArcScrapAmount;
            if (scrap > 0)
            {
                stash.DepositScrap(scrap);
            }

            session.RaidInventory.Clear();
            session.SetStatus(RaidSessionStatus.Extracted);
        }

        public void Fail(RaidSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));

            if (session.Status == RaidSessionStatus.Unknown || session.Status == RaidSessionStatus.Extracted)
            {
                return;
            }

            session.RaidInventory.Clear();
            session.SetStatus(RaidSessionStatus.Failed);
        }
    }
}
