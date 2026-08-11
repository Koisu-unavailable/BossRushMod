using System;
using System.Linq;
using Luminance.Core.Cutscenes;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
        public readonly long[] allBosses =
        [
            NPCID.KingSlime,
            NPCID.EyeofCthulhu,
            NPCID.Deerclops,
            NPCID.QueenBee,
            NPCID.BrainofCthulhu,
            NPCID.EaterofWorldsHead,
            NPCID.SkeletronHead,
            NPCID.WallofFlesh,
            NPCID.QueenSlimeBoss,
            NPCID.TheDestroyer,
            NPCID.SkeletronPrime,
            NPCID.Retinazer,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.HallowBoss, // empress of light
            NPCID.DukeFishron,
            NPCID.CultistBoss,
            NPCID.MoonLordCore
        ];
        public bool IsBossRushMode { get; private set; } = false;
        public long currentBossIndex {get; private set;} = 0;
        //this is for debug editing
        public static float eocSpeedMultiplier = 1.5f;

        // this is in 24 hour time
        public TimeSpan buildSecondsRemaining { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        private const int PREPARATION_SECONDS = 5; // five minutes
        public Player[] Players { private get; set; } = [];
        public bool Fighting => Phase != BossRushPhase.Intro && Phase != BossRushPhase.None;
        // /// <summary>
        // /// This is the ID of the NPC that when killed will summon the next boss.
        // /// E.g. The orginal head of the eate
        // /// </summary>
        // public int currentBossID 
        public void Reset()
        {
            Main.getGoodWorld = false;
            Players = [];
            IsBossRushMode = false;
            currentBossIndex = 0;
            Phase = BossRushPhase.None;
        }
        public void StartBossRush(Player summoner)
        {
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
            if (Fighting)
            {
                Main.dayTime = false;
                Main.time = Main.nightLength - 1;
            }
        }

        private void StartFight()
        {
            Phase = BossRushPhase.Fight1;
            Main.getGoodWorld = true;
            SummonBoss(allBosses[0]);
        }

        public void EndBossRush(bool success)
        {
            // give them a buff or smth
            Main.NewText("The hallucinations disisspate");
            if (success){
                Main.NewText("You Won!");
            }
            Reset();
        }

        public void SummonNextBoss()
        {
            currentBossIndex++;
            if (currentBossIndex > allBosses.Length){
                EndBossRush(true);
                return;
            }
            if (allBosses[currentBossIndex] == NPCID.Retinazer)
            {
                SummonBoss(allBosses[currentBossIndex]);
                SummonBoss(NPCID.Spazmatism);
                return;
            }
            SummonBoss(allBosses[currentBossIndex]);
        }

        private void SummonBoss(long bossID)
        {
            SoundEngine.PlaySound(SoundID.Roar);
            IsBossRushMode = true;

            if (Players.Length == 0)
            {
                return;
            }

            Player chosenPlayer = Players[Random.Shared.Next(Players.Length)];
            NPC.SpawnOnPlayer(chosenPlayer.whoAmI, (int)bossID);
            
        }
    }

    public enum BossRushPhase
    {
        None,
        Intro,
        Fight1
    }
}
