using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

public class BossRushBackGroundStyle : ModSurfaceBackgroundStyle
{
    private Asset<Texture2D> background;
    public override void Load()
    {
        base.Load();
        background = Mod.Assets.Request<Texture2D>("Assets/background");
    }
    public override void ModifyFarFades(float[] fades, float transitionSpeed)
    {
        return; 
    }
    public override bool PreDrawCloseBackground(SpriteBatch spriteBatch)
    {
        var screenRec = new Rectangle(0,0, Main.screenWidth, Main.screenHeight);
        spriteBatch.Draw(background.Value, new Microsoft.Xna.Framework.Vector2(0,0), Color.White);
        return false;
    }
}