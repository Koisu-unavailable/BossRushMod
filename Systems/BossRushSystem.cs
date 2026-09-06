using System;
using System.Linq;
using Luminance.Core.Cutscenes;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Terraria.Chat;
using Terraria.Localization;
using BossRush.Cutscenes;
using BossRush.VanillaTweaks.Bosses;

namespace BossRush.Systems
{
    public class BossRushSystem : ModSystem
    {
        public readonly long[] banndedItems =
        [
            ItemID.RodOfHarmony // probably cheating
        ];
        public BossRushPhase Phase { get; private set; }
        private static readonly long[] preharmodeBosses =
        {
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.Deerclops,
            NPCID.QueenBee,
            NPCID.BrainofCthulhu,
            NPCID.EaterofWorldsHead,
            NPCID.SkeletronHead,
            NPCID.WallofFlesh,
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
        public readonly Boss[] allBosses =
        [
            new Boss(){Type = NPCID.KingSlime, Health = 10000, Damage = 300, extraEnemiesToBuff = [NPCID.SlimeSpiked, NPCID.BlueSlime]},
            new Boss(){Type = NPCID.EyeofCthulhu, Health = 15000, Damage = 150, PostAI = BossRushModeBoss.EocPostAI, extraEnemiesToBuff = [NPCID.ServantofCthulhu]},
            new Boss(){Type = NPCID.Deerclops, healthMult = 3, DamageMult = 5, 
                extraProjectilesToBuff = [ProjectileID.DeerclopsIceSpike, ProjectileID.DeerclopsRangedProjectile, ProjectileID.InsanityShadowHostile]},
            new Boss(){Type = NPCID.QueenBee, healthMult = 5, DamageMult = 2, extraProjectilesToBuff = [ProjectileID.QueenBeeStinger], extraEnemiesToBuff = [NPCID.Bee]},
            new Boss(){Type = NPCID.BrainofCthulhu, healthMult = 10, DamageMult = 3, extraEnemiesToBuff = [NPCID.Creeper]},
            new Boss(){Type = NPCID.EaterofWorldsHead, healthMult = 20, DamageMult = 4,
                extraEnemiesToBuff = [NPCID.EaterofSouls, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail, NPCID.VileSpit]},
            new Boss(){Type = NPCID.SkeletronHead, healthMult = 9, DamageMult = 3, 
            extraEnemiesToBuff = [NPCID.DarkCaster, NPCID.SkeletronHand], 
            extraProjectilesToBuff = [ProjectileID.Skull]},
            new Boss(){Type = NPCID.WallofFlesh, healthMult = 10, DamageMult = 3, 
            extraEnemiesToBuff = [NPCID.TheHungry, NPCID.TheHungryII, NPCID.FireImp, NPCID.BurningSphere, NPCID.LeechBody, NPCID.LeechHead, NPCID.LeechTail], 
            extraProjectilesToBuff = [ProjectileID.EyeBeam], PostAI = BossRushModeBoss.WofPostAI, OnKill = BossRushModeBoss.OnWOFKill},
            new Boss(){Type = NPCID.QueenSlimeBoss, healthMult = 10, DamageMult = 2,
            extraEnemiesToBuff = [NPCID.QueenSlimeMinionBlue, NPCID.QueenSlimeMinionPink, NPCID.QueenSlimeMinionPurple],
            extraProjectilesToBuff = [ProjectileID.QueenSlimeMinionBlueSpike, ProjectileID.QueenSlimeMinionPinkBall, ProjectileID.QueenSlimeSmash]},
            new Boss(){Type = NPCID.TheDestroyer, healthMult = 6, DamageMult = 2,
            extraEnemiesToBuff = [NPCID.TheDestroyerBody, NPCID.TheDestroyerTail, NPCID.Probe],
            extraProjectilesToBuff = [ProjectileID.DeathLaser, ProjectileID.PinkLaser]},
            new Boss(){Type = NPCID.SkeletronPrime, healthMult = 6, DamageMult = 2,
            extraEnemiesToBuff = [NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser],
            extraProjectilesToBuff = [ProjectileID.BombSkeletronPrime, ProjectileID.DeathLaser]},
            new Boss(){Type = NPCID.Retinazer, healthMult = 6, DamageMult = 2,
            extraEnemiesToBuff = [NPCID.Spazmatism],
            extraProjectilesToBuff = [ProjectileID.EyeLaser, ProjectileID.CursedFlameHostile, ProjectileID.DeathLaser, ProjectileID.EyeFire]},
            new Boss(){Type = NPCID.Plantera, healthMult = 5, DamageMult = 1,
            extraEnemiesToBuff = [NPCID.PlanterasHook, NPCID.PlanterasTentacle, NPCID.Spore],
            extraProjectilesToBuff = [ProjectileID.Seed, ProjectileID.PoisonSeedPlantera, ProjectileID.ThornBall]},
            NPCID.Golem,
            NPCID.HallowBoss, // empress of light
            NPCID.DukeFishron,
            NPCID.CultistBoss,
            NPCID.MoonLordCore
        ];
        public bool IsBossRushMode { get; private set; } = false;
        public long currentBossIndex { get; private set; } = 0;

        // guard against concurrent/rapid summoning calls which can skip or block bosses
        private bool isSummoning = false;
        public Player[] Players { get; private set; } = [];
        public bool Fighting => Phase != BossRushPhase.Intro && Phase != BossRushPhase.None;

        public Boss CurrentBoss => allBosses[currentBossIndex];

        // stored in BossRushSystem so they can be editing in game with dragon lens
        #region Boss Difficulty Modifiers
        public static float eocSpeedMultiplier = 1.5f;
        public static float eocMaxSpeed = 18f; // I calcuated this to be around the max speed
        public static float eocMaxDistance = 30f * 16f; // 30 tiles converted to screen coordinates
        public static float WOFSpeedMultiplier = 2f;
        public static float WOFMaxSpeed = 10; // calculated heuritically
        #endregion

        public void Reset()
        {
            Main.getGoodWorld = false; // check if was FTW before
            Players = [];
            IsBossRushMode = false;
            currentBossIndex = 0;
            Phase = BossRushPhase.None;
            isSummoning = false;
        }
        public void StartBossRush(Player summoner)
        {
            if (IsBossRushMode || Phase == BossRushPhase.Intro)
            {
                return;
            }

            Phase = BossRushPhase.Intro;
            Players = Main.player
                .Where(p => p.active && p.position.Distance(summoner.position) < 500)
                .ToArray();

            if (Players.Length == 0)
            {
                Players = [summoner];
            }

            IsBossRushMode = true;
            DoIntro();
        }
        public void DoIntro()
        {
            var scene = new BossRushIntro();
            scene.OnCutsceneEnd += StartFight;
            CutsceneManager.QueueCutscene(scene);
        }
        public override void PostUpdateEverything()
        {
            if (!Fighting)
            {
                return;
            }
            else
            {
                Main.dayTime = false;
                Main.time = Main.nightLength - 1;
            }
        }

        private void StartFight()
        {
            if (!IsBossRushMode || Phase == BossRushPhase.Fight1)
            {
                return;
            }
            Phase = BossRushPhase.Fight1;
            Main.getGoodWorld = true;
            currentBossIndex = -1;
            SummonNextBoss();
        }

        public void EndBossRush(bool success)
        {
            // give them a buff or smth
            Main.NewText("The hallucinations disisspate");
            if (success)
            {
                Main.NewText("You Won!");
            }
            Reset();
        }
        public override void OnWorldUnload()
        {
            Reset();
        }
        public bool HasCurrentBossAlive()
        {
            if (!IsBossRushMode || currentBossIndex < 0 || currentBossIndex >= allBosses.Length)
            {
                return false;
            }

            long bossType = allBosses[currentBossIndex];
            bool bossAlive = Main.npc.Any(n => n.active && n.life > 0 && n.type == bossType);

            if (bossType == NPCID.Retinazer)
            {
                bossAlive |= Main.npc.Any(n => n.active && n.life > 0 && n.type == NPCID.Spazmatism);
            }
            else if (bossType == NPCID.EaterofWorldsHead)
            {
                bossAlive |= Main.npc.Any(n => n.active && n.life > 0 && (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail));
            }

            return bossAlive;
        }

        public void SummonNextBoss()
        {
            if (!IsBossRushMode || isSummoning)
                return;

            isSummoning = true;
            if (++currentBossIndex >= allBosses.Length)
            {
                EndBossRush(true);
                isSummoning = false;
                return;
            }

            if (!SummonBoss(allBosses[currentBossIndex]))
            {
                currentBossIndex--;
                isSummoning = false;
                return;
            }

            if (allBosses[currentBossIndex] == NPCID.Retinazer)
            {
                SummonBoss(NPCID.Spazmatism);
            }
            isSummoning = false;
        }

        private int SpawnBossNearPlayer(Player chosenPlayer, int bossType)
        {
            int spawnX = (int)(chosenPlayer.position.X + Main.rand.Next(-300, 301) + 150);
            int spawnY = (int)(chosenPlayer.position.Y - 600f);
            if (bossType == NPCID.WallofFlesh)
            {
                spawnX = (int)chosenPlayer.position.X;
                spawnY = (Main.UnderworldLayer + 10) * 16; // spawn where the underworld is usually opened up
            }
            int spawnedIndex = NPC.NewNPC(new EntitySource_BossSpawn(chosenPlayer), spawnX, spawnY, bossType, 1);

            Main.NewText($"SummonBoss: NewNPC returned index={spawnedIndex} for bossID={bossType}");

            if (spawnedIndex < 0)
            {
                Main.NewText($"SummonBoss: failed to create boss {bossType} near player {chosenPlayer.whoAmI}");
                return -1;
            }

            NPC spawnedBoss = Main.npc[spawnedIndex];
            if (!spawnedBoss.active || spawnedBoss.type != bossType)
            {
                Main.NewText($"SummonBoss: created NPC index={spawnedIndex}, but it was not active or had the wrong type for boss {bossType}");
                return -1;
            }

            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasAwoken", spawnedBoss.TypeName), new Color(175, 75, 255));
            }
            else if (Main.dedServ)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", [spawnedBoss.GetTypeNetName()]), new Color(175, 75, 255));
            }

            return spawnedIndex;
        }

        private bool SummonBoss(long bossID)
        {
            Main.NewText($"SummonBoss: requested bossID={bossID}");
            SoundEngine.PlaySound(SoundID.Roar);

            if (Players == null || Players.Length == 0)
            {
                Main.NewText($"SummonBoss: no available players to spawn boss {bossID}");
                return false;
            }

            Player chosenPlayer = Players[Random.Shared.Next(Players.Length)];
            Main.NewText($"SummonBoss: chosenPlayer whoAmI={chosenPlayer.whoAmI} at position={chosenPlayer.position}");

            if (!chosenPlayer.active || chosenPlayer.dead)
            {
                Main.NewText($"SummonBoss: invalid chosen player {chosenPlayer.whoAmI} for boss {bossID}; picking another player");
                chosenPlayer = Players.FirstOrDefault(player => player.active && !player.dead) ?? chosenPlayer;
            }

            if (!chosenPlayer.active || chosenPlayer.dead)
            {
                Main.NewText($"SummonBoss: no valid player available to spawn boss {bossID}");
                return false;
            }

            return SpawnBossNearPlayer(chosenPlayer, (int)bossID) >= 0;
        }
    }

    public enum BossRushPhase
    {
        None,
        Intro,
        Fight1
    }
}
