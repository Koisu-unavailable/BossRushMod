using System.Linq;
using BossRush.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace BossRush.Bosses
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
                BossRushSystem.SummonNextBoss();
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            base.ModifyNPCLoot(npc, npcLoot);
        }
    }
}
