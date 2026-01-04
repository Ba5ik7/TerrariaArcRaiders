using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Players;
using TerrariaArcRaiders.Adapters.Systems;

namespace TerrariaArcRaiders.Adapters.NPCs
{
    // Minimal hub console NPC skeleton; interaction and spawn logic are added in later tasks.
    public class RaidHubConsoleNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 1;
            NPCID.Sets.ActsLikeTownNPC[Type] = false;
        }

        public override void SetDefaults()
        {
            NPC.width = 18;
            NPC.height = 40;
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
            button = raidPlayer.IsInRaid ? "Extract / Exit" : "Enter Raid";
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
                    RaidUiNotifications.Send(player, RaidUiNotifications.AlreadyInRaid);
                }
                return;
            }

            var exited = RaidSystem.TryInteractExit(player);
            if (!exited)
            {
                RaidUiNotifications.Send(player, RaidUiNotifications.NotInRaid);
            }
        }
    }
}