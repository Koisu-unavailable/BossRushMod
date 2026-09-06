using System;
using System.Linq;
using BossRush.Systems;
using Microsoft.Xna.Framework;
using Mono.CompilerServices.SymbolWriter;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
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
        public bool hasWOFTeleported = false;
        public override void SetDefaults(NPC npc)
        {
            if (BossRushSystem.IsBossRushMode)
            {
                if (npc.type == BossRushSystem.CurrentBoss)
                {
                    npc.lifeMax = BossRushSystem.CurrentBoss.Health ?? npc.lifeMax;
                    npc.lifeMax *= BossRushSystem.CurrentBoss.healthMult ?? 1;
                    npc.damage = BossRushSystem.CurrentBoss.Damage ?? npc.damage;
                    npc.damage *= BossRushSystem.CurrentBoss.DamageMult ?? 1;
                }
                if (BossRushSystem.CurrentBoss.extraEnemiesToBuff == null) { return; }
                if (BossRushSystem.CurrentBoss.extraEnemiesToBuff.Contains(npc.type))
                {
                    npc.lifeMax *= BossRushSystem.CurrentBoss.healthMult ?? 1;
                    npc.damage *= BossRushSystem.CurrentBoss.DamageMult ?? 1;
                }

            }
        }
        // modify npc AI's if required
        public override void PostAI(NPC npc)
        {
            if (BossRushSystem.IsBossRushMode)
            {
                if (!(BossRushSystem.CurrentBoss.PostAI == null) && npc.type == BossRushSystem.CurrentBoss.Type)
                {
                    BossRushSystem.CurrentBoss.PostAI(npc, this);
                }

            }
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (BossRushSystem.IsBossRushMode)
            {
                if (!(BossRushSystem.CurrentBoss.OnSpawn == null) && npc.type == BossRushSystem.CurrentBoss.Type)
                {
                    BossRushSystem.CurrentBoss.OnSpawn(npc, source);
                }

            }
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitPlayer(npc, target, ref modifiers);
            if (BossRushSystem.IsBossRushMode)
            {
                if (npc.type == NPCID.VileSpit)
                {
                    modifiers.IncomingDamageMultiplier *= 10;
                }
            }
        }
        public static void WofPostAI(NPC npc, BossRushModeBoss bossRushModeBoss)
        {
            if (!bossRushModeBoss.hasWOFTeleported)
            {
                var bossRushSystem = ModContent.GetInstance<BossRushSystem>();
                var searchCenter = npc.Center;
                var searchArea = new Rectangle((int)searchCenter.X - 200, (int)searchCenter.Y, 400, 400);
                var tpPos = Utils.FindValidTpPos(searchArea, searchCenter,  6);
                foreach (Player p in bossRushSystem.Players)
                {
                    if (!p.active || !Utils.IsValidTeleportPosition(tpPos, p.Size))
                    {
                        Main.NewText("WOFPostAI: Couldn't find valid tpPos");
                        continue;
                    }

                    // teleport is only called on server in multiplayer so p.Teleport is fine here
                    Main.NewText($"WOFPostAI: TPing player: {p.name} to {tpPos}");
                    p.GetModPlayer<BossRushPlayer>().previousPos = p.position;
                    p.Teleport(tpPos, TeleportationStyleID.TeleportationPotion);
                }
                bossRushModeBoss.hasWOFTeleported = true;
                
            }
            SpeedUpNPC(npc, BossRushSystem.WOFSpeedMultiplier, BossRushSystem.WOFMaxSpeed); 
        }
        public static void EocPostAI(NPC npc, BossRushModeBoss globalNPC)
        {
            SpeedUpNPC(npc, BossRushSystem.eocSpeedMultiplier, BossRushSystem.eocMaxSpeed);

            // check not dashing
            if (npc.ai[1] == 0)
                ReverseIfTooFar(npc, BossRushSystem.eocMaxDistance, globalNPC);
            globalNPC.lastPosition = npc.position;
            globalNPC.hasLastPosition = true;
        }
        /// <summary>
        /// ai generated
        /// </summary>
        private static void ReverseIfTooFar(NPC npc, float maxDistance, BossRushModeBoss globalNPC)
        {
            if (globalNPC.hasLastPosition && npc.target >= 0 && npc.target < Main.maxPlayers && Main.player[npc.target].active)
            {
                Player target = Main.player[npc.target];
                float distanceSquared = Vector2.DistanceSquared(target.Center, npc.Center);
                float previousDistanceSquared = Vector2.DistanceSquared(target.Center, globalNPC.lastPosition + npc.Size / 2f);
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
        private static void SpeedUpNPC(NPC npc, float mult, float maxMagnitude)
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
        public static void OnWOFKill(NPC npc, BossRushModeBoss globalNPC)
        {
            var bossRushSystem = ModContent.GetInstance<BossRushSystem>();
            foreach (Player p in bossRushSystem.Players)
            {
                p.Teleport(p.GetModPlayer<BossRushPlayer>().previousPos, TeleportationStyleID.TeleportationPotion);
            }
        }
        // ts pmo icl
        public override void OnKill(NPC npc)
        {
            if (!BossRushSystem.IsBossRushMode)
            {
                return;
            }
            if (BossRushSystem.CurrentBoss == npc.type)
            {
                if (BossRushSystem.CurrentBoss.OnKill != null) BossRushSystem.CurrentBoss.OnKill(npc, this);
            }
            bool isRelevantBoss = BossRushSystem.allBosses.Contains(npc.type)
                || npc.type == NPCID.Spazmatism
                || npc.type == NPCID.EaterofWorldsBody
                || npc.type == NPCID.EaterofWorldsTail
                || npc.type == NPCID.EaterofWorldsHead;
            if (isRelevantBoss && !BossRushSystem.HasCurrentBossAlive())
            {
                BossRushSystem.SummonNextBoss();
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
