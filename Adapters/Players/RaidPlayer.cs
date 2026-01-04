using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Systems;
using TerrariaArcRaiders.Core.Models;
using TerrariaArcRaiders.Core.Services;

#nullable enable

namespace TerrariaArcRaiders.Adapters.Players
{
    public class RaidPlayer : ModPlayer
    {
        private readonly RaidSessionService _sessionService = new();

        private RaidSession? _session;

        private bool _respawnToHub;

        public RaidSession? CurrentSession => _session;

        public bool IsInRaid => _session != null && _session.Status != RaidSessionStatus.Unknown && _session.Status != RaidSessionStatus.Extracted && _session.Status != RaidSessionStatus.Failed;

        private string PlayerId => Player.name;

        public bool TryEnterRaid()
        {
            if (IsInRaid)
            {
                return false;
            }

            _session = _sessionService.StartSession(PlayerId);
            _sessionService.ActivateSession(_session);
            return true;
        }

        public void AwardScrap(int amount)
        {
            if (!IsInRaid)
            {
                return;
            }

            _sessionService.AddScrap(_session!, amount);
        }

        public bool TryExtract()
        {
            if (_session == null)
            {
                return false;
            }

            var stash = RaidSystem.GetOrCreateStash(PlayerId);
            _sessionService.Extract(_session, stash);
            _session = null;
            return true;
        }

        public void FailRun()
        {
            if (_session == null)
            {
                return;
            }

            _sessionService.Fail(_session);
            _session = null;
            _respawnToHub = true;
        }

        public override void OnEnterWorld()
        {
            _session = null;
            _ = RaidSystem.GetOrCreateStash(PlayerId);
            _respawnToHub = false;
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (IsInRaid)
            {
                FailRun();
            }

            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void OnRespawn()
        {
            base.OnRespawn();

            if (_respawnToHub)
            {
                TeleportToSpawn();
                _respawnToHub = false;
            }
        }

        private void TeleportToSpawn()
        {
            var spawnPosition = new Vector2(Main.spawnTileX * 16, Main.spawnTileY * 16);
            Player.Teleport(spawnPosition, TeleportationStyleID.RodOfDiscord);
        }
    }
}
