#nullable enable

using System;
using TerrariaArcRaiders.Core.Models;

namespace TerrariaArcRaiders.Core.Services
{
    public sealed class RaidPersistence : IRaidPersistence
    {
        private const string KeyStash = "stash";
        private const string KeySession = "session";
        private const string KeyScrap = "scrap";
        private const string KeyPlayerId = "playerId";
        private const string KeyStatus = "status";

        public TagCompoundDto Save(Stash stash, RaidSession? sessionSnapshot)
        {
            var root = new TagCompoundDto();
            root.SetInt(KeyStash, Math.Max(0, stash?.ArcScrapAmount ?? 0));

            if (sessionSnapshot != null)
            {
                var session = new TagCompoundDto();
                session.SetString(KeyPlayerId, sessionSnapshot.PlayerId);
                session.SetString(KeyStatus, sessionSnapshot.Status.ToString());
                session.SetInt(KeyScrap, Math.Max(0, sessionSnapshot.RaidInventory.ArcScrapAmount));
                root.SetCompound(KeySession, session);
            }

            return root;
        }

        public void Load(TagCompoundDto? data, Stash stash, out RaidSession? sessionSnapshot)
        {
            sessionSnapshot = null;

            if (stash == null) throw new ArgumentNullException(nameof(stash));

            stash.Clear();

            if (data == null)
            {
                return;
            }

            var stashAmount = Math.Max(0, data.GetInt(KeyStash));
            if (stashAmount > 0)
            {
                stash.DepositScrap(stashAmount);
            }

            var sessionData = data.GetCompound(KeySession);
            if (sessionData == null)
            {
                return;
            }

            var playerId = sessionData.GetString(KeyPlayerId);
            var statusString = sessionData.GetString(KeyStatus);

            if (string.IsNullOrWhiteSpace(playerId) || string.IsNullOrWhiteSpace(statusString))
            {
                return;
            }

            if (!Enum.TryParse(statusString, out RaidSessionStatus status))
            {
                return;
            }

            var session = new RaidSession(playerId);
            session.RaidInventory.Clear();

            var scrap = Math.Max(0, sessionData.GetInt(KeyScrap));
            if (scrap > 0)
            {
                session.RaidInventory.AddScrap(scrap);
            }

            session.SetStatus(status);
            sessionSnapshot = session;
        }
    }
}
