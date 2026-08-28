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
        public Player[] Players { private get; set; } = [];
        public bool Fighting => Phase != BossRushPhase.Intro && Phase != BossRushPhase.None;

        // /// <summary>
        // /// This is the ID of the NPC that when killed will summon the next boss.
        // /// E.g. The orginal head of the eate
        // /// </summary>
        // public int currentBossID; 

        // stored in BossRushSystem so they can be editing in game with dragon less
        #region Boss Difficulty Modifier
        public static float eocSpeedMultiplier = 1.5f;
        public static float eocMaxSpeed = 18f; // I calcuated this to be around the max speed
        public static float eocMaxDistance = 30f * 16f; // 30 tiles converted to screen coordinates
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
        public void SummonNextBoss()
        {
            if (!IsBossRushMode || isSummoning)
                return;

            isSummoning = true;
            try
            {
                if (++currentBossIndex >= allBosses.Length)
                {
                    EndBossRush(true);
                    return;
                }

                SummonBoss(allBosses[currentBossIndex]);
                if (allBosses[currentBossIndex] == NPCID.Retinazer)
                {
                    SummonBoss(NPCID.Spazmatism);
                }
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
            if (bossID == NPCID.Golem || bossID == NPCID.SkeletronHead || bossID == NPCID.DukeFishron)
            {
                Main.NewText("SummonBoss: special-case Golem spawn (bypassing temple requirement)");

                int shitBoss = NPC.NewNPC(new EntitySource_BossSpawn(chosenPlayer), (int)(chosenPlayer.position.X + Main.rand.Next(-100, 101)), (int)(chosenPlayer.position.Y - 600f), (int)bossID, 1);
                Main.NewText($"SummonBoss: Golem/Skeletron/Duke NewNPC returned index={shitBoss}");

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
