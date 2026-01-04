using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Systems;

namespace TerrariaArcRaiders.Adapters.Commands
{
    // Dev-only raid command; behavior is gated and implemented incrementally in later tasks.
    public class ArcRaidCommand : ModCommand
    {
        private const string UsageText = "/arcraid <enter|exit|toggle>";

        public override CommandType Type => CommandType.Chat;

        public override string Command => "arcraid";

        public override string Description => "Dev raid control (stubbed).";

        public override string Usage => UsageText;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var config = ArcRaidersConfig.Instance;
            var devToolsEnabled = config?.DevToolsEnabled == true;

            if (!devToolsEnabled)
            {
                caller.Reply(RaidUiNotifications.DevToolsDisabled);
                caller.Reply($"Usage: {UsageText}");
                return;
            }

            if (args.Length == 0)
            {
                caller.Reply($"Usage: {UsageText}");
                return;
            }

            var verb = args[0].ToLowerInvariant();
            switch (verb)
            {
                case "enter":
                case "exit":
                case "toggle":
                    caller.Reply($"Dev command ready: '{verb}' will be wired to raid interactions in the next task.");
                    break;
                default:
                    caller.Reply($"Usage: {UsageText}");
                    break;
            }
        }
    }
}
