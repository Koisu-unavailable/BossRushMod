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
        public override bool NoPlayerSaving => false;

        public override void Update()
        {
            BossRush.logger.Debug("UPDATED");
            BossRushSystem.UpdateTimeRemaining();
            

        }

        public override void OnLoad()
        {
            BossRush.logger.Debug("LOADED");
            Main.dayTime = true;
            Main.time = 0;
            BossRushSystem.StartBossRush();
        }
        
    }
}
