using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TerrariaArcRaiders.Adapters.Players;
using TerrariaArcRaiders.Core.Services;

namespace TerrariaArcRaiders.Adapters.NPCs
{
    public class ArcDroneNPC : ModNPC
    {
        private readonly DropRules _dropRules = new();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Firefly];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 10;
            NPC.defense = 2;
            NPC.lifeMax = 30;
            NPC.value = 0;
            NPC.knockBackResist = 0.8f;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            AIType = NPCID.BlueSlime;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            // Simple gate: only allow spawn when player is in a raid session.
            if (spawnInfo.Player is null)
            {
                return 0f;
            }

            var raidPlayer = spawnInfo.Player.GetModPlayer<RaidPlayer>();
            return raidPlayer.IsInRaid ? 0.2f : 0f;
        }

        public override void OnKill()
        {
            var player = Main.player[NPC.target];
            if (player == null || !player.active)
            {
                return;
            }

            var raidPlayer = player.GetModPlayer<RaidPlayer>();
            if (!raidPlayer.IsInRaid)
            {
                return;
            }

            var session = raidPlayer.CurrentSession;
            if (session == null)
            {
                return;
            }

            _dropRules.AwardDroneScrap(session);
        }
    }
}
