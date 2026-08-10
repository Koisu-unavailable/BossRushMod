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
            NPCID.Spazmatism,
            NPCID.Plantera,
            NPCID.Golem,
            NPCID.HallowBoss, // empress of light
            NPCID.DukeFishron,
            NPCID.CultistBoss,
            NPCID.MoonLordCore
        ];
        public bool IsBossRushMode { get; private set; } = false;
        private long currentBossIndex = 0;

        // this is in 24 hour time
        public TimeSpan buildSecondsRemaining { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        private const int PREPARATION_SECONDS = 5; // five minutes
        public Player[] Players { private get; set; } = [];
        public bool fighting => Phase != BossRushPhase.Intro && Phase != BossRushPhase.None;
        public void Reset()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            Players = [];
            buildSecondsRemaining = new TimeSpan(0);
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
            if (!fighting)
            {
                foreach (Player p in Players)
                {
                    if (allBosses[currentBossIndex] == NPCID.BrainofCthulhu){
                        p.ZoneCrimson = true;
                    }
                    else if (allBosses[currentBossIndex] == NPCID.EaterofWorldsHead){
                        p.ZoneCorrupt = true;
                    }
                    else {
                        p.ZoneCrimson = false;
                        p.ZoneCorrupt = false;
                    }
                }
                return;
            }
            if (fighting)
            {
                Main.dayTime = false;
                Main.time = Main.nightLength - 1;
            }
        }

        private void StartFight()
        {
            Phase = BossRushPhase.Fight1;

            SummonBoss(allBosses[0]);
        }

        public void EndBossRush()
        {
            // give them a buff or smth
            Main.NewText("The hallucinations disisspate");
            Reset();
        }

        public void SummonNextBoss()
        {
            currentBossIndex++;
            SummonBoss(allBosses[currentBossIndex]);
        }

        private void SummonBoss(long bossID)
        {
            // SoundEngine.PlaySound(SoundID.Roar);
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
