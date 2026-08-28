using System;
using System.Linq;
using BossRush.Systems;
using Microsoft.Xna.Framework;
using Steamworks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace BossRush.VanillaTweaks.Bosses
{
    public class BossRushModeBoss : GlobalNPC
    {

        private BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();
        public override bool InstancePerEntity => true;
        private Vector2 lastPosition;
        private bool hasLastPosition;
        public override void SetDefaults(NPC npc)
        {
            if (BossRushSystem.IsBossRushMode)
            {
                switch (npc.type)
                {
                    case NPCID.KingSlime:
                        npc.lifeMax = 10000; // moodlord master health
                        npc.damage = 150;
                        break;
                    case NPCID.EyeofCthulhu:
                        npc.lifeMax = 9800;
                        npc.damage = 150;
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
            switch (npc.type)
            {
                case NPCID.EyeofCthulhu:
                    SpeedUpNPC(npc, BossRushSystem.eocSpeedMultiplier, BossRushSystem.eocMaxSpeed);

                    // check not dashing
                    if (npc.ai[1] == 0)

                        ReverseIfTooFar(npc, BossRushSystem.eocMaxDistance);

                    lastPosition = npc.position;
                    hasLastPosition = true;
                    break;

            }

        }
        private void ReverseIfTooFar(NPC npc, float maxDistance)
        {
            if (hasLastPosition && npc.target >= 0 && npc.target < Main.maxPlayers && Main.player[npc.target].active)
            {
                Player target = Main.player[npc.target];
                float distanceSquared = Vector2.DistanceSquared(target.Center, npc.Center);
                float previousDistanceSquared = Vector2.DistanceSquared(target.Center, lastPosition + npc.Size / 2f);
                if (distanceSquared > maxDistance * maxDistance && distanceSquared >= previousDistanceSquared)
                {
                    Vector2 toTarget = Main.player[npc.target].position - npc.Center;

                    float targetRotation = toTarget.ToRotation();
                    float currentRotation = npc.velocity.ToRotation();

                    float rotation = MathHelper.WrapAngle(targetRotation - currentRotation);

                    npc.velocity = npc.velocity.RotatedBy(rotation);
                }
            }
        }
        /// <summary>
        /// Speeds up an npc by a certain amount. If the resulting speed is too high, it will try to get the velocity vector as close to maxMagnitude as possible
        /// </summary>
        /// <param name="npc">npc</param>
        /// <param name="mult">speed multiplier</param>
        /// <param name="maxMagnitude">Maximum magnitude of the velocity vector</param>
        private void SpeedUpNPC(NPC npc, float mult, float maxMagnitude)
        {
            // try get as close to max speed as possible
            for (float tryMult = mult; tryMult > 1; tryMult -= 0.001f)
            {
                var newSpeed = npc.velocity * tryMult;
                if (!(newSpeed.Length() > maxMagnitude))
                {
                    npc.velocity = newSpeed;
                    break;
                }
            }

        }
        // ts pmo icl
        public override void OnKill(NPC npc)
        {
            if (!BossRushSystem.IsBossRushMode)
            {
                return;
            }

            BossRushSystem.allBosses.Append(NPCID.Spazmatism).ToList().ForEach(n => Console.WriteLine(n));
            bool isRelevantBoss = BossRushSystem.allBosses.Append(NPCID.Spazmatism).Contains(npc.type)
                || npc.type == NPCID.EaterofWorldsBody
                || npc.type == NPCID.EaterofWorldsTail
                || npc.type == NPCID.EaterofWorldsHead;

            if (isRelevantBoss)
            {
                // make sure no bosses or Eater segments remain alive/active
                bool anyBossesAlive = Main.npc.Any(n => n.life > 0 && n.boss);
                bool anyEaterSegmentsAlive = Main.npc.Any(n => n.life > 0 && (n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsTail));

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
