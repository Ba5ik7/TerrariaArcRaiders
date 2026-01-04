using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Players;
using TerrariaArcRaiders.Adapters.Systems;

namespace TerrariaArcRaiders.Adapters.NPCs
{
    // Minimal hub console NPC skeleton; interaction and spawn logic are added in later tasks.
    public class RaidHubConsoleNPC : ModNPC
    {
        private const int MessageCooldownTicks = 60; // ~1 second at 60fps

        private static readonly Dictionary<int, LastMessage> LastMessageByPlayer = new();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = false;
        }

        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.friendly = true;
            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1; // No movement/AI yet
        }

        public override bool CanChat() => true;

        public override void SetChatButtons(ref string button, ref string button2)
        {
            var raidPlayer = Main.LocalPlayer.GetModPlayer<RaidPlayer>();
            button = raidPlayer.IsInRaid ? "Extract / Exit" : "Enter Raid"; // Text kept concise; messages unified elsewhere.
            button2 = Language.GetTextValue("LegacyInterface.52"); // Close
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (!firstButton)
            {
                return;
            }

            var player = Main.LocalPlayer;
            if (player is null)
            {
                return;
            }

            var raidPlayer = player.GetModPlayer<RaidPlayer>();

            if (!raidPlayer.IsInRaid)
            {
                var entered = RaidSystem.TryInteractPortal(player);
                if (!entered)
                {
                    TrySendRefusal(player, RaidUiNotifications.AlreadyInRaid);
                }
                return;
            }

            var exited = RaidSystem.TryInteractExit(player);
            if (!exited)
            {
                TrySendRefusal(player, RaidUiNotifications.NotInRaid);
            }
        }

        private static bool TrySendRefusal(Player player, string message)
        {
            var now = Main.GameUpdateCount;
            var playerId = player.whoAmI;

            if (LastMessageByPlayer.TryGetValue(playerId, out var last)
                && last.Message == message
                && now - last.Tick < MessageCooldownTicks)
            {
                return false;
            }

            LastMessageByPlayer[playerId] = new LastMessage
            {
                Message = message,
                Tick = now
            };

            RaidUiNotifications.Send(player, message);
            return true;
        }

        private struct LastMessage
        {
            public string Message { get; init; }

            public ulong Tick { get; init; }
        }
    }
}
