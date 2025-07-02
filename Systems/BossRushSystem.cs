using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;


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
        private readonly long[] allBosses;
        public static bool isBossRushMode = false;
        private Player _playerWhoSummoned;

        // can't initalize the allBosses array immediatly because .AddRange() doesn't return an instance of the list.
        BossRushSystem(Player player)
        {
            var temp = allBosses.ToList();
            temp.AddRange(preharmodeBosses);
            temp.AddRange(hardmodeBosses);
            allBosses = temp.ToArray();
            _playerWhoSummoned = player;
        }


        public static void StartBossRush(Player player)
        {
            if (isBossRushMode)
            {
                Main.NewText("What the hell are you doing??!?");
                player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name}"), 99999, 2, true);
                BossRushSystem.isBossRushMode = false;
                return;
            }
            
        }
        private void SummonBoss(Player player, int bossID)
        {
            SoundEngine.PlaySound(SoundID.Roar);
            isBossRushMode = true;
            IEntitySource entitySource = new EntitySource_Parent(player);
            int[] validPlayerIndexes = GetPlayerIndexesWithinBossRushRange();
            int chosesPlayerIndex = validPlayerIndexes[Random.Shared.Next(validPlayerIndexes.Length)];
            NPC.SpawnBoss((int)player.position.X + 300, (int)player.position.Y + 300, bossID, chosesPlayerIndex);
            
        }
        private int[] GetPlayerIndexesWithinBossRushRange() {
            return null;
        }
    }
}