using Terraria;

namespace TerrariaArcRaiders.Adapters.Systems
{
    // Shared, reusable messages for raid entry/exit and refusal paths.
    // Provides both pure string helpers and a minimal sender to keep adapters thin and testable.
    public static class RaidUiNotifications
    {
        public static string EnteringRaid => "Entering ARC raid zone.";

        public static string ExtractionSuccess => "Extraction successful. Stash updated.";

        public static string AlreadyInRaid => "Already in a raid session.";

        public static string NotInRaid => "You are not currently in a raid.";

        public static string DevToolsDisabled => "Dev tools are disabled for this world.";

        public static string DevEnter => "Dev: entering raid session.";

        public static string DevExit => "Dev: exiting raid session.";

        public static string InventoryFullNote => "Player inventory is full; stash still updated.";

        public static void Send(Player player, string message)
        {
            if (player == null || !player.active)
            {
                return;
            }

            Main.NewText(message);
        }
    }
}