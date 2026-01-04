using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
    }
}