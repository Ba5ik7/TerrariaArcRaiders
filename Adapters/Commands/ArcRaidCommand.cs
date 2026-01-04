using Terraria;
using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Players;
using TerrariaArcRaiders.Adapters.Systems;

namespace TerrariaArcRaiders.Adapters.Commands
{
    // Dev-only raid command; behavior is gated and implemented incrementally in later tasks.
    public class ArcRaidCommand : ModCommand
    {
        private const string UsageText = "/arcraid <enter|exit|toggle>";

        public override CommandType Type => CommandType.Chat;

        public override string Command => "arcraid";

        public override string Description => "Dev raid control (enter/exit/toggle).";

        public override string Usage => UsageText;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var config = ArcRaidersConfig.Instance;
            var devToolsEnabled = config?.DevToolsEnabled == true;

            if (!devToolsEnabled)
            {
                caller.Reply(RaidUiNotifications.DevToolsDisabled);
                caller.Reply(GetUsageLine());
                return;
            }

            if (args.Length == 0)
            {
                caller.Reply(GetUsageLine());
                return;
            }

            var player = caller.Player;
            if (player is null)
            {
                caller.Reply("ARC Raid: player-only command.");
                return;
            }

            var raidPlayer = player.GetModPlayer<RaidPlayer>();

            var verb = args[0].ToLowerInvariant();
            switch (verb)
            {
                case "enter":
                    HandleEnter(player);
                    break;
                case "exit":
                    HandleExit(player);
                    break;
                case "toggle":
                    if (raidPlayer.IsInRaid)
                    {
                        HandleExit(player);
                    }
                    else
                    {
                        HandleEnter(player);
                    }
                    break;
                default:
                    caller.Reply(GetUsageLine());
                    break;
            }
        }

        private static void HandleEnter(Player player)
        {
            var entered = RaidSystem.TryInteractPortal(player);
            if (!entered)
            {
                RaidUiNotifications.Send(player, RaidUiNotifications.AlreadyInRaid);
                return;
            }

            RaidUiNotifications.Send(player, RaidUiNotifications.DevEnter);
        }

        private static void HandleExit(Player player)
        {
            var exited = RaidSystem.TryInteractExit(player);
            if (!exited)
            {
                RaidUiNotifications.Send(player, RaidUiNotifications.NotInRaid);
                return;
            }

            RaidUiNotifications.Send(player, RaidUiNotifications.DevExit);
        }

        private static string GetUsageLine() => $"Usage: {UsageText}";
    }
}
