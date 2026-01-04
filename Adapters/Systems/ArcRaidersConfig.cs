using System.ComponentModel;
using Terraria.ModLoader;

namespace TerrariaArcRaiders.Adapters.Systems
{
    // Server-side config to gate developer/testing tools; defaults to disabled for normal play.
    public class ArcRaidersConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static ArcRaidersConfig Instance => ModContent.GetInstance<ArcRaidersConfig>();

        [Label("Enable dev tools (raid entry/exit debug)")]
        [DefaultValue(false)]
        public bool DevToolsEnabled { get; set; } = false;
    }
}