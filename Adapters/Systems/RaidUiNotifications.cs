using Terraria;

namespace TerrariaArcRaiders.Adapters.Systems
{
    // Shared, reusable messages for raid entry/exit and refusal paths.
    // Provides both pure string helpers and a minimal sender to keep adapters thin and testable.
    public static class RaidUiNotifications
    {
        public static string EnteringRaid => "ARC Raid: entering session.";

        public static string ExtractionSuccess => "ARC Raid: extraction complete. Stash updated.";

        public static string AlreadyInRaid => "ARC Raid: already in a raid.";

        public static string NotInRaid => "ARC Raid: not currently in a raid.";

        public static string DevToolsDisabled => "ARC Raid dev tools are disabled; enable them in config to use /arcraid.";

        public static string DevEnter => "Dev: raid session entered.";

        public static string DevExit => "Dev: raid session exited.";

        public static string InventoryFullNote => "ARC Raid: inventory full; stash still updated.";

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