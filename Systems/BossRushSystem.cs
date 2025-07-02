using System;
using System.Linq;
using System.Security.Principal;
using BossRush.utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush.Systems
{
    public class BossRushSystem : ModSystem
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
        public readonly long[] allBosses = [];
        public bool isBossRushMode { get; private set; } = false;
        private Player _playerWhoSummoned;
        private Vector2 whereSummoned;
        private const int farthestPlayerCanBeAwayToParticipate = 10000;
        private long currentBossIndex = 0;

        // can't initalize the allBosses array immediatly because .AddRange() doesn't return an instance of the list.
        BossRushSystem()
        {
            var temp = allBosses.ToList();
            temp.AddRange(preharmodeBosses);
            temp.AddRange(hardmodeBosses);
            allBosses = temp.ToArray();
        }

        public void StartBossRush(Player player)
        {
            if (isBossRushMode)
            {
                Main.NewText("What the hell are you doing??!?");
                player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name}"), 99999, 2, true);
                isBossRushMode = false;
                return;
            }
            isBossRushMode = true;
            _playerWhoSummoned = player;
            whereSummoned = _playerWhoSummoned.Center;
            SummonBoss(allBosses[currentBossIndex]);
        }
        public void SummonNextBoss()
        {
            currentBossIndex++;
            SummonBoss(allBosses[currentBossIndex]);
        }
        private void SummonBoss(long bossID)
        {
            SoundEngine.PlaySound(SoundID.Roar);
            isBossRushMode = true;
            int[] validPlayerIndexes = GetPlayerIndexesWithinBossRushRange();
            if (!validPlayerIndexes.Any())
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
#pragma warning disable CS8632
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
#pragma warning restore CS8632
    }
}
