using System.Linq;
using BossRush.Systems;
using Luminance.Common.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace BossRush.VanillaTweaks.Bosses
{
    public class BossRushModeBoss : GlobalNPC
    {
        
        private BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();
        static float EocSpeedMultiplier => BossRushSystem.eocSpeedMultiplier;
        public override void SetDefaults(NPC npc)
        {
            
            if (BossRushSystem.IsBossRushMode)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        npc.scale = 10;
                        goto case NPCID.EyeofCthulhu;
                    case NPCID.EyeofCthulhu:
                        npc.lifeMax *= 30;
                        npc.damage *= 10;
                        break;
                    case NPCID.Deerclops:
                        npc.lifeMax *= 5;
                        npc.damage *= 3;
                        break;
                    case NPCID.QueenBee:
                        npc.lifeMax *= 10;
                        npc.damage *= 3;
                        break;
                }
            }
        }
        public override void PostAI(NPC npc)
        {
            base.PostAI(npc);
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    if (npc.target != -1 && npc.velocity.Length() < 20)
                    {
                        if (npc.ai[1] == 4)
                        {
                            npc.velocity *= EocSpeedMultiplier;
                        }
                        
                    }
                    break;
            }
        }
        // ts pmo icl
        public override void OnKill(NPC npc)
        {
            
            // Treat Eater of Worlds body/tail/head as a boss death trigger as well
            bool isRelevantBoss = BossRushSystem.allBosses.Append(NPCID.Spazmatism).Contains(npc.type)
                || npc.type == NPCID.EaterofWorldsBody
                || npc.type == NPCID.EaterofWorldsTail
                || npc.type == NPCID.EaterofWorldsHead;

            if (isRelevantBoss)
            {
                // make sure no bosses or Eater segments remain alive/active
                bool anyBossesAlive = Main.npc.Any(n => n.active && n.life > 0 && n.boss);
                bool anyEaterSegmentsAlive = Main.npc.Any(n => n.active && n.life > 0 && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail));

                if (!(anyBossesAlive || anyEaterSegmentsAlive))
                {
                    BossRushSystem.SummonNextBoss();
                }
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            // this is permanant
            base.ModifyNPCLoot(npc, npcLoot);
            // npcLoot.RemoveWhere(_ => true); // remove all loot
        }
    }
}
