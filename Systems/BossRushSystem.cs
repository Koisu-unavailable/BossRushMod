using System;
using System.Collections;
using System.Linq;
using System.Reflection.Emit;
using BossRush.Subworlds;
using BossRush.utils;
using Humanizer;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BossRush.Systems
{
    public class BossRushSystem : ModSystem
    {
        public readonly long[] banndedItems =
        [
            ItemID.EmpressFlightBooster, // soaring insignia
            ItemID.RodOfHarmony
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

        public Queue playersToSendAway = [];

        public void Reset()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            Players = [];
            buildSecondsRemaining = new TimeSpan(0);
            IsBossRushMode = false;
            currentBossIndex = 0;
            playersToSendAway = [];
            Phase = BossRushPhase.None;
        }

        public void UpdateTimeRemaining()
        {
            buildSecondsRemaining = EndTime.TimeOfDay - DateTime.Now.TimeOfDay;
            if (buildSecondsRemaining < TimeSpan.Zero)
            {
                AdvancePhase();
            }
        }

        // public override void PostUpdateEverything()
        // {
        //     BossRush.logger.Debug($"HERE: {playersToSendAway.Count}");
        //     if (playersToSendAway.Count != 0)
        //     {
        //         SubworldSystem.MovePlayerToMainWorld(((Player)playersToSendAway.Dequeue()).whoAmI);
        //         BossRush.logger.Debug(playersToSendAway.ToArray().Humanize());
        //     }



        // }
        public void StartBossRush()
        {
            Phase = BossRushPhase.Build;
            IsBossRushMode = true;
            buildSecondsRemaining = new TimeSpan(0, 0, PREPARATION_SECONDS);
            StartTime = DateTime.Now;
            EndTime = EndTime.Add(
                StartTime.TimeOfDay.Add(
                    new TimeSpan(seconds: PREPARATION_SECONDS, minutes: 0, hours: 0)
                )
            );
        }

        public delegate void OnChangePhase(BossRushPhase newPhase);
        public event OnChangePhase PhaseChangeHandler;

        public void AdvancePhase()
        {
            switch (Phase)
            {
                case BossRushPhase.Build:
                    StartFight();
                    PhaseChangeHandler?.Invoke(BossRushPhase.Fight);
                    break;
                case BossRushPhase.Fight:
                    EndBossRush();
                    PhaseChangeHandler?.Invoke(BossRushPhase.End);
                    break;
            }
        }

        public override void PostUpdateEverything()
        {
            if (Phase == BossRushPhase.Fight)
            {
                foreach (Player p in Players)
                {
                    p.ZoneCrimson = true;
                    
                }
            }
        }

        private void StartFight()
        {
            Phase = BossRushPhase.Fight;
            SummonBoss(allBosses[0]);
        }

        private void EndBossRush()
        {
            Reset();
        }

        public void SummonNextBoss()
        {
            currentBossIndex++;
            SummonBoss(allBosses[currentBossIndex]);
        }

        private void SummonBoss(long bossID)
        {
            SoundEngine.PlaySound(SoundID.Roar);
            IsBossRushMode = true;
            Player chosenPlayer = Players[Random.Shared.Next(Players.Length)];
            NPC.SpawnOnPlayer(chosenPlayer.whoAmI, (int)bossID);
        }
    }

    public enum BossRushPhase
    {
        None,
        Build,
        Fight,
        End
    }
}
