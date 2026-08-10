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
        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (BossRushSystem.allBosses.Contains(npc.type))
            {
                // make sure eof is complety dead


                if (BossRushSystem.allBosses.Contains(npc.type))
                {

                    if (!(Main.npc.Any(n => n.life > 0 && n.boss) 
                    || Main.npc.Any(n => n.life > 0 && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail))) )
                    {
                        BossRushSystem.SummonNextBoss();
                    }

                }

            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
            npcLoot.RemoveWhere(_ => true); // remove all loot
        }
    }
}
