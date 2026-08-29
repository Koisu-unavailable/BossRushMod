using BossRush.Systems;
using Terraria;
using Terraria.ModLoader;

public class BossRushMusic : ModSceneEffect
{
    private BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();
    public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<BossRushBackGroundStyle>();
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

    
    public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sound/BAKA");

    public override bool IsSceneEffectActive(Player player)
    {
        return BossRushSystem.IsBossRushMode;
    }
}