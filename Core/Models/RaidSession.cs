using System;

namespace TerrariaArcRaiders.Core.Models
{
    public sealed class RaidSession
    {
        public RaidSession(string playerId)
        {
            PlayerId = playerId ?? throw new ArgumentNullException(nameof(playerId));
            Status = RaidSessionStatus.Entered;
            RaidInventory = new RaidInventory();
            CreatedAtUtc = DateTimeOffset.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }

        public string PlayerId { get; }

        public RaidSessionStatus Status { get; private set; }

        public RaidInventory RaidInventory { get; }

        public DateTimeOffset CreatedAtUtc { get; }

        public DateTimeOffset UpdatedAtUtc { get; private set; }

        public void SetStatus(RaidSessionStatus status)
        {
            Status = status;
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }

        public void ResetInventory()
        {
            RaidInventory.Clear();
            UpdatedAtUtc = DateTimeOffset.UtcNow;
        }
    }
}
