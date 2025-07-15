using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BossRush.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using SubworldLibrary;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace BossRush.Subworlds
{
    public class BossArenaSubworld : Subworld
    {
        public override int Width => 1000;
        public override int Height => 1000;
        public override List<GenPass> Tasks => [new GenArenaPass()];
        private static BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();

        public override void Update()
        {
            if (BossRushSystem.Phase == BossRushPhase.Build)
            {
                BossRushSystem.UpdateTimeRemaining();
            }
            Main.dayTime = false;
            Main.time = 2;
            BossRushSystem.Players = Main.player;
        }

        public override void DrawMenu(GameTime gameTime)
        {
            base.DrawMenu(gameTime);
            Asset<Texture2D> background = Mod.Assets.Request<Texture2D>("Assets/background");
            Main.spriteBatch.Draw(
                background.Value,
                new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                Color.White
            );
        }

        public override void OnLoad()
        {
            BossRushSystem.StartBossRush();
            BossRushSystem.PhaseChangeHandler += (
                phase
            ) => { // TODO: Implement??
            };
        }
    }
}
