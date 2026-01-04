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
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.DemonEye];
        }

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 32;

            NPC.damage = 20;
            NPC.defense = 4;
            NPC.lifeMax = 60;

            NPC.knockBackResist = 0.8f;
            NPC.value = Item.buyPrice(silver: 5);

            NPC.aiStyle = NPCAIStyleID.DemonEye;
            AIType = NPCID.DemonEye;

            NPC.noGravity = true;
            NPC.noTileCollide = false;

            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;

            NPC.buffImmune[BuffID.Confused] = false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Smoke, 0f, -1f);
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric, 0f, -1f);
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player is null)
            {
                return 0f;
            }

            var raidPlayer = spawnInfo.Player.GetModPlayer<RaidPlayer>();
            return raidPlayer.IsInRaid ? 0.9f : 0f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.Electrified, 60 * 3);
            }
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
