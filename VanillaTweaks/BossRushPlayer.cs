using BossRush.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;


namespace BossRush.VanillaTweaks;

public class BossRushPlayer : ModPlayer
{
    // the previous position of the player before it waas teleported to the WOF
    public Vector2 previousPos;
    private bool diedDuringBossRush;
    private SoundStyle reviveSFX;
    public override void OnEnterWorld()
    {
        reviveSFX = new SoundStyle("BossRush/Assets/Sound/revive", SoundType.Sound);
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        base.Kill(damage, hitDirection, pvp, damageSource);
        BossRushSystem bossRushSystem = ModContent.GetInstance<BossRushSystem>();
        if (bossRushSystem.IsBossRushMode)
        {
            diedDuringBossRush = true;
            bossRushSystem.EndBossRush(false);
            Player.respawnTimer = 1;
        }}



    public override void OnRespawn()
    {
        BossRushSystem bossRushSystem = ModContent.GetInstance<BossRushSystem>();
        
        bool shouldPlayReviveSound = diedDuringBossRush && !bossRushSystem.IsBossRushMode;
        if (shouldPlayReviveSound && !Main.dedServ && Main.myPlayer == Player.whoAmI)
        {
            SoundEngine.PlaySound(reviveSFX);
        }
        diedDuringBossRush = false;
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