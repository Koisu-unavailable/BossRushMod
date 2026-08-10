using System;
using Luminance.Core.Cutscenes;
using Luminance.Core.Graphics;
using Terraria;
using Terraria.Localization;

public class BossRushIntro : Cutscene
{
    public override int CutsceneLength => 60 * 1;
    public event Action OnCutsceneEnd;
    public override void OnBegin()
    {
        Main.NewText(Language.GetText("Mods.BossRush.Misc.Dizzy").ToString());
    }
    override public void Update()
    {
        Main.blockInput = true;
        Main.NewText("funking it up");
    }
    public override void OnEnd()
    {
        Main.blockInput = false;
        OnCutsceneEnd?.Invoke();
    }
}