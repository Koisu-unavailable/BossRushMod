using BossRush.Systems;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

public class BossRushBackGroundStyle : ModSurfaceBackgroundStyle
{
    private Asset<Texture2D> background;
    private Asset<Effect> effect;
    private ManagedRenderTarget target;


    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        return;
    }
    public override void Load()
    {
        base.Load();

        background = Mod.Assets.Request<Texture2D>("Assets/space");
        effect = Mod.Assets.Request<Effect>("Assets/Shaders/AutoloadedEffects/Filters/blink");

        target = new ManagedRenderTarget(true, (width, height) =>
            new RenderTarget2D(
                Main.instance.GraphicsDevice,
                width,
                height));

        RenderTargetManager.RenderTargetUpdateLoopEvent += UpdateTarget;
    }

    private void UpdateTarget()
    {
        GraphicsDevice gd = Main.instance.GraphicsDevice;
        gd.SetRenderTarget(target);
        gd.Clear(Color.Transparent);

        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            effect.Value.Parameters["colour"].SetValue(new Vector4(1f, 0.5f, 0.75f, 1f));

            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect.Value);

            Main.spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle(0, 0, target.Target.Width, target.Target.Height),
                Color.White);

            Main.spriteBatch.End();
        }
        else
        {
            gd.Clear(Color.Transparent);
        }
        gd.SetRenderTarget(null);
    }

    public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
    {
        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            spriteBatch.Draw(
                target,
                new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                Color.White);
        }

        // We drew our replacement background, so skip the vanilla close background.
        return false;
    }
}