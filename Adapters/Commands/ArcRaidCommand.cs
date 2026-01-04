using System;
using Terraria.ModLoader;

namespace TerrariaArcRaiders.Adapters.Commands
{
    // Dev-only raid command skeleton; behavior will be added in later tasks.
    public class ArcRaidCommand : ModCommand
    {
        private const string UsageText = "/arcraid <enter|exit|toggle>";

        public override CommandType Type => CommandType.Chat;

        public override string Command => "arcraid";

        public override string Description => "Dev raid control (stubbed).";

        public override string Usage => UsageText;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
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
                    caller.Reply("Dev command stub: behavior not yet implemented.");
                    break;
                default:
                    caller.Reply($"Usage: {UsageText}");
                    break;
            }
        }
    }
}
