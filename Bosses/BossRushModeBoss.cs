using System;
using System.Linq;
using System.Numerics;
using BossRush.Systems;
using BossRush.utils;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush.Bosses
{
    public class BossRushModeBoss : GlobalNPC
    {
        private static readonly long[] preharmodeBosses =
        {
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.Deerclops,
            NPCID.QueenBee,
            NPCID.EaterofWorldsHead,
            NPCID.BrainofCthulhu,
            NPCID.SkeletronHead,
            NPCID.WallofFlesh
        };
        private static readonly long[] hardmodeBosses =
        {
            NPCID.QueenSlimeBoss,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            NPCID.Retinazer,
            NPCID.Spazmatism,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.HallowBoss, // empress of light
            NPCID.DukeFishron,
            NPCID.CultistBoss,
            NPCID.MoonLordCore
        };
        private static int currentBoss = NPCID.KingSlime;

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (BossRushSystem.isBossRushMode)
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
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            base.OnSpawn(npc, source);
            if (
                BossRushSystem.isBossRushMode
                && (preharmodeBosses.Contains(npc.type) || hardmodeBosses.Contains(npc.type))
            )
            {
                for (int i = 0; i < 200; i++)
                {
                    Dust.NewDust(
                        npc.position,
                        (int)npc.Size.X,
                        (int)npc.Size.Y,
                        DustID.Blood,
                        1 + Random.Shared.Next(10),
                        1 + Random.Shared.Next(10),
                        1
                    );
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            base.OnKill(npc);
            if (BossRushSystem.isBossRushMode && preharmodeBosses.Contains(npc.type))
            {
                Main.NewText("Placeholder");
                IEntitySource entitySource = new EntitySource_Parent(npc);
                NPC.NewNPC(
                    npc.GetSource_FromAI(),
                    (int)npc.position.X + 100,
                    (int)npc.position.Y + 100,
                    (int)preharmodeBosses[Array.IndexOf(preharmodeBosses, npc.type) + 1]
                );
                currentBoss = (int)preharmodeBosses[Array.IndexOf(preharmodeBosses, npc.type) + 1];
            }
        }
    }
}
