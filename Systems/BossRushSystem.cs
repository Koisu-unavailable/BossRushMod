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
        public long currentBossIndex { get; private set; } = 0;
        // guard against concurrent/rapid summoning calls which can skip or block bosses
        private bool isSummoning = false;
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
            if (success)
            {
                Main.NewText("You Won!");
            }
            Reset();
        }

        public void SummonNextBoss()
        {
            // Prevent overlapping calls which could increment the index multiple times
            if (isSummoning)
            {
                Main.NewText($"SummonNextBoss: call skipped because isSummoning=true (currentIndex={currentBossIndex})");
                return;
            }
            isSummoning = true;
            try
            {
                Main.NewText($"SummonNextBoss: currentIndex={currentBossIndex}, computing nextIndex");
                long nextIndex = currentBossIndex + 1;
                if (nextIndex >= allBosses.Length)
                {
                    Main.NewText($"SummonNextBoss: nextIndex={nextIndex} >= allBosses.Length={allBosses.Length}; ending BossRush");
                    EndBossRush(true);
                    return;
                }

                currentBossIndex = nextIndex;

                long nextBoss = allBosses[currentBossIndex];
                Main.NewText($"SummonNextBoss: nextIndex={nextIndex}, nextBossID={nextBoss}");

                if (nextBoss == NPCID.Retinazer)
                {
                    Main.NewText($"SummonNextBoss: Summoning Retinazer (ID={nextBoss}) and Spazmatism");
                    SummonBoss(nextBoss);
                    SummonBoss(NPCID.Spazmatism);
                    return;
                }

                SummonBoss(nextBoss);
            }
            finally
            {
                isSummoning = false;
            }
        }

        private void SummonBoss(long bossID)
        {
            Main.NewText($"SummonBoss: requested bossID={bossID}");
            SoundEngine.PlaySound(SoundID.Roar);

            if (Players == null || Players.Length == 0)
            {
                Main.NewText($"SummonBoss: no available players to spawn boss {bossID}");
                return;
            }

            Player chosenPlayer = Players[Random.Shared.Next(Players.Length)];
            Main.NewText($"SummonBoss: chosenPlayer whoAmI={chosenPlayer.whoAmI} at position={chosenPlayer.position}");
            // stolen staright from calamity source for special cases
            if (bossID == NPCID.Golem || bossID == NPCID.Skeleton || bossID == NPCID.DukeFishron)
            {
                Main.NewText("SummonBoss: special-case Golem spawn (bypassing temple requirement)");

                int shitBoss = NPC.NewNPC(new EntitySource_BossSpawn(chosenPlayer), (int)(chosenPlayer.position.X + Main.rand.Next(-100, 101)), (int)(chosenPlayer.position.Y - 600f), (int)bossID, 1);
                Main.NewText($"SummonBoss: Golem/Skeletron NewNPC returned index={shitBoss}");

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue("Announcement.HasAwoken", Main.npc[shitBoss].TypeName), new Color(175, 75, 255));
                }
                else if (Main.dedServ)
                {
                    ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", [Main.npc[shitBoss].GetTypeNetName()]), new Color(175, 75, 255));
                }
                return;
            }


            Main.NewText($"SummonBoss: using SpawnOnPlayer for bossID={bossID} on player {chosenPlayer.whoAmI}");
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
