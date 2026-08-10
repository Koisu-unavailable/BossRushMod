using BossRush.Systems;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace BossRush.VanillaTweaks;
public class BossRushPlayer : ModPlayer
{
    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        base.Kill(damage, hitDirection, pvp, damageSource);
        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            ModContent.GetInstance<BossRushSystem>().EndBossRush(false);
        }
    }
    public override void PostUpdate()
    {
        base.PostUpdate();
        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            Player.ZoneCrimson = true;
            Player.ZoneCorrupt = true;
        }
    }
}