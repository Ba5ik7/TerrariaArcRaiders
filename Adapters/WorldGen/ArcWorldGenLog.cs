using Terraria.ModLoader;

namespace TerrariaArcRaiders.Adapters.WorldGen
{
    // Lightweight helper for consistent worldgen debug logging.
    internal static class ArcWorldGenLog
    {
        private const string Prefix = "[ArcWorldGen]";

        internal static bool Enabled { get; set; } = true;

        internal static void Stage(string stageName, string details = null)
        {
            if (!Enabled || string.IsNullOrWhiteSpace(stageName))
            {
                return;
            }

            var logger = ModContent.GetInstance<TerrariaArcRaiders>().Logger;
            var suffix = string.IsNullOrWhiteSpace(details) ? string.Empty : $" - {details}";
            logger.Debug($"{Prefix} {stageName}{suffix}");
        }

        internal static void Info(string message)
        {
            if (!Enabled || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            ModContent.GetInstance<TerrariaArcRaiders>().Logger.Info($"{Prefix} {message}");
        }
    }
}
