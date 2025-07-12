using System.Collections.Generic;
using BossRush.UI;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace BossRush.Systems
{
    [Autoload(Side = ModSide.Client)]
    public class BossRushUiSystem : ModSystem
    {
        internal BossRushUIState bossRushUIState;

        private UserInterface bossRushUI;

        public override void Load()
        {
            bossRushUIState = new BossRushUIState();
            bossRushUIState.Activate();
            bossRushUI = new UserInterface();
            bossRushUI.SetState(bossRushUIState);
        }

        public override void UpdateUI(GameTime gameTime)
        {
            bossRushUI?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer =>
                layer.Name.Equals("Vanilla: Mouse Text")
            );
            if (mouseTextIndex != -1)
            {
                layers.Insert(
                    mouseTextIndex,
                    new LegacyGameInterfaceLayer(
                        "Boss Rush: Adds a boss rush",
                        delegate
                        {
                            bossRushUI.Draw(Main.spriteBatch, new GameTime());
                            return true;
                        },
                        InterfaceScaleType.UI
                    )
                );
            }
        }
    }
}
