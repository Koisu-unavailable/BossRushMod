using System;
using System.Reflection;
using log4net.Repository.Hierarchy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil.Cil;
using ReLogic.Graphics;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace BossRush.UI
{
    public class Text(
        Func<string> textSupplier,
        Vector2 pos,
        Vector2 scale,
        float rotation,
        SpriteEffects spriteEffects,
        DynamicSpriteFont font
    ) : UIElement
    {
        Color color = Color.White;

        public Func<string> textSupplier = textSupplier;
        private string text => textSupplier();
        private readonly Vector2 pos = pos;
        private Vector2 scale = scale;

        private readonly float rotation = rotation;
        private readonly SpriteEffects spriteEffects = spriteEffects;
        private readonly DynamicSpriteFont font = font;
        private static bool LoggedError = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            // access the draw method of the DynamicSpriteFont because it's internal
            var drawMethod = typeof(DynamicSpriteFont).GetMethod(
                "InternalDraw",
                BindingFlags.NonPublic | BindingFlags.Instance
            );
            if (drawMethod == null)
            {
                if (!LoggedError)
                {
                    BossRush.logger.Error(
                        $"Could not find InternalDraw method of {nameof(DynamicSpriteFont)}"
                    );
                    LoggedError = true;
                }
                return;
            }
            // internal void InternalDraw(string text, SpriteBatch spriteBatch, Vector2 startPosition, Color color, float rotation, Vector2 origin, ref Vector2 scale, SpriteEffects spriteEffects, float depth)
            drawMethod.Invoke(
                font,
                [
                    text,
                    spriteBatch,
                    pos,
                    color,
                    rotation,
                    font.MeasureString(text) * Vector2.Zero + new Vector2(0, 0.1f),
                    scale,
                    SpriteEffects.None,
                    1
                ]
            );
        }
    }
}
