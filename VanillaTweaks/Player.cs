using BossRush.Systems;
using Terraria.DataStructures;
using Terraria.ModLoader;

public class LeaveOnDeath : ModPlayer
{
    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        base.Kill(damage, hitDirection, pvp, damageSource);
        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            ModContent.GetInstance<BossRushSystem>().EndBossRush();
        }
    }
}