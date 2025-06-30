using System;
using System.Linq;
using BossRush.utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;



namespace BossRush.Bosses
{
    public class BossRushModeBoss : GlobalNPC, BossSummonManagment
    {
        private static long[] preharmodeBosses =
        {
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.Deerclops,
            NPCID.QueenBee,
            NPCID.SkeletronHead,
            NPCID.WallofFlesh
        };
        private static int currentBoss = NPCID.EyeofCthulhu;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (preharmodeBosses.Contains(npc.type))
            {
                npc.lifeMax *= 100;
                npc.damage = 200;
            }
        }

        public override bool PreAI(NPC npc)
        {
            return true;
        }

        public override void PostAI(NPC npc)
        {
            if (currentBoss == NPCID.EyeofCthulhu)
            {
                npc.despawnEncouraged = false;
                npc.DiscourageDespawn(10);
            }
        }

        public override void AI(NPC npc)
        {
            base.AI(npc);
            if (npc.type == NPCID.EyeofCthulhu && Globals.isBossRushMode)
            {
                npc.EncourageDespawn(10);
            }

        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            npc.BigMimicSpawnSmoke();
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (Globals.isBossRushMode && preharmodeBosses.Contains(npc.type))
            {
                Main.NewText("Placeholder");
                IEntitySource entitySource = new EntitySource_Parent(npc);
                NPC.NewNPC(
                    entitySource,
                    (int)npc.position.X + 50,
                    (int)npc.position.Y + 50,
                    (int)preharmodeBosses[Array.IndexOf(preharmodeBosses, npc.type) + 1]
                );
                currentBoss = (int)preharmodeBosses[Array.IndexOf(preharmodeBosses, npc.type) + 1];
            }
        }
    }
    
}
