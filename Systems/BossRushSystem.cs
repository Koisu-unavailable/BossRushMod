using System;
using System.Linq;
using BossRush.utils;
using Microsoft.Xna.Framework;
using SubworldLibrary;
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
        public readonly long[] allBosses = [];
        public bool IsBossRushMode { get; private set; } = false;
        private Player _playerWhoSummoned;
        private Vector2 whereSummoned;
        private const int farthestPlayerCanBeAwayToParticipate = 10000;
        private long currentBossIndex = 0;

        // this is in 24 hour time
        public TimeSpan buildSecondsRemaining { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        private const int PREPARATION_SECONDS = 5 * 60; // five minutes

        // can't initalize the allBosses array immediatly because .AddRange() doesn't return an instance of the list.
        BossRushSystem()
        {
            var temp = allBosses.ToList();
            temp.AddRange(preharmodeBosses);
            temp.AddRange(hardmodeBosses);
            allBosses = temp.ToArray();
        }

        public void UpdateTimeRemaining()
        {
            
            BossRush.logger.Debug($"Starttime: {StartTime.TimeOfDay}, End time: {EndTime.TimeOfDay}, Now: {DateTime.Now.TimeOfDay.Seconds}");

            
            buildSecondsRemaining = (EndTime.TimeOfDay - DateTime.Now.TimeOfDay);

            

            BossRush.logger.Debug(buildSecondsRemaining);
        }

        public void StartBossRush()
        {
            if (IsBossRushMode)
            {
                throw new Exception("Boss rush already started");
            }
            Phase = BossRushPhase.Build;
            IsBossRushMode = true;
            _playerWhoSummoned = Main.player[0];
            whereSummoned = _playerWhoSummoned.Center;
            buildSecondsRemaining = new TimeSpan(0, 5, 0);
            StartTime = DateTime.Now;
            EndTime = EndTime.Add(
                StartTime.TimeOfDay.Add(
                    new TimeSpan(seconds: PREPARATION_SECONDS, minutes: 0, hours: 0)
                )
            );
        }

        public void AdvancePhase()
        {
            switch (Phase)
            {
                case BossRushPhase.Build:
                    StartFight();
                    break;
                case BossRushPhase.Fight:
                    EndBossRush();
                    break;
            }
        }

        private void StartFight()
        {
            SummonBoss(allBosses[0]);
        }

        private void EndBossRush()
        {
            throw new NotImplementedException();
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
            int[] validPlayerIndexes = GetPlayerIndexesWithinBossRushRange();
            if (validPlayerIndexes.Length == 0)
            {
                throw new NoPlayersInRangeException();
            }
            int chosesPlayerIndex = validPlayerIndexes[
                Random.Shared.Next(validPlayerIndexes.Length)
            ];
            Player player = Main.player[chosesPlayerIndex];
            NPC.SpawnBoss(
                (int)player.GetValidBossSpawnPostion().ToWorldCoordinates().X,
                (int)player.GetValidBossSpawnPostion().ToWorldCoordinates().Y,
                (int)bossID,
                chosesPlayerIndex
            );
        }

        /// <summary>
        /// Get the index of the players within the range of the boss rush event
        /// </summary>
        /// <returns>A list of the indexes or null if none of the players are in the range of the boss rush</returns>
        private int[]? GetPlayerIndexesWithinBossRushRange()
        {
            int[] goodPlayers = [];
            int i = 0; // index
            foreach (Player player in Main.player)
            {
                if (Main.player.Last() == player) // the dummy entry
                {
                    break;
                }
                if (!player.dead && player.active)
                {
                    if (
                        player.position.Distance(whereSummoned)
                        < farthestPlayerCanBeAwayToParticipate
                    )
                    {
                        goodPlayers = goodPlayers.Append(player.whoAmI).ToArray();
                        i++;
                    }
                }
            }
            return goodPlayers;
        }
    }

    public enum BossRushPhase
    {
        None,
        Build,
        Fight
    }
}
