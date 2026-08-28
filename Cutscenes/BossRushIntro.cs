using System;
using Luminance.Core.Cutscenes;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BossRush.Cutscenes;
public class BossRushIntro : Cutscene
{
    public override int CutsceneLength => 60 * 2;
    public event Action OnCutsceneEnd;
    private ManagedScreenFilter blinkShader;
    private SoundStyle alert;
    public override void Load()
    {
        base.Load();
        // if (!Main.dedServ)
        // {

        //     alert = new SoundStyle("BossRush/Assets/Sound/Alert") {Volume = 2 };
        //     alert.GetSoundEffect(); // force it to load, idkl if required
        // }

    }
    public override void OnBegin()
    {
        // Main.NewText(Language.GetText("Mods.BossRush.Misc.Dizzy").ToString(), new Color(175, 75, 255));
        // Main.NewText(ShaderManager.AutoloadDirectoryFilters);
        // var check = ShaderManager.TryGetFilter("blink", out blinkShader); // shader needs to be compiled
        // Main.NewText(check);
        // if (check)
        // {
        //     blinkShader.TrySetParameter("colour", new Vector4(1, 1,1,1));
        // }
    }
    override public void Update()
    {
        Main.blockInput = true;
        // blinkShader.Activate(); silenety fails if doesn;'t woirk
    }
    public override void OnEnd()
    {
        Main.blockInput = false;
        OnCutsceneEnd?.Invoke();
        // SoundEngine.PlaySound(alert);
    }
}