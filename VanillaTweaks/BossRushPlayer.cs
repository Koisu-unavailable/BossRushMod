using BossRush.Systems;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace BossRush.VanillaTweaks;
public class BossRushPlayer : ModPlayer
{
    // the previous position of the player before it waas teleported to the WOF
    public Vector2 previousPos;
    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        base.Kill(damage, hitDirection, pvp, damageSource);
        if (ModContent.GetInstance<BossRushSystem>().IsBossRushMode)
        {
            ModContent.GetInstance<BossRushSystem>().EndBossRush(false);
            Player.respawnTimer = 30; // half a second
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