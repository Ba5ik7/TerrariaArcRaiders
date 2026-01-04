using System;
using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Core.Services
{
    public sealed class DropRules
    {
        public int GetScrapFromDroneKill()
        {
            // v0.1 simple rule: always 1 scrap per drone
            return 1;
        }

        public void AwardDroneScrap(RaidSession session)
        {
            if (session == null) return;
            if (session.Status == RaidSessionStatus.Unknown || session.Status == RaidSessionStatus.Extracted || session.Status == RaidSessionStatus.Failed)
            {
                return;
            }

            var amount = GetScrapFromDroneKill();
            session.RaidInventory.AddScrap(amount);
        }
    }
}
