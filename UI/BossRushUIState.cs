using System;
using System.Net.PeerToPeer.Collaboration;
using BossRush.Systems;
using BossRush.utils;
using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BossRush.UI
{
    public class BossRushUIState : UIState
    {
        public Text text;
        private BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();

        public override void OnInitialize()
        {
            text = new Text(
                () => "",
                new Vector2(Main.screenWidth, Main.screenHeight) / 2,
                Vector2.One * 2,
                0,
                SpriteEffects.None,
                FontAssets.MouseText.Value
            );
            Append(text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (ModContent.GetInstance<BossRushSystem>().Phase == BossRushPhase.Build)
            {
                text.textSupplier = () =>
                {
                    TimeSpan seconds = ModContent.GetInstance<BossRushSystem>().buildSecondsRemaining;
                    return $"{seconds.Minutes}:{(seconds.Seconds.ToString().Length == 1 ? '0' + seconds.Seconds : seconds.Seconds)}";
                };
            }
            else
            {
                text.textSupplier = () => "";
            }
        }
    }
}
