using BossRush.Systems;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Terraria;
using Terraria.ModLoader;

public class BossRushProjectile : GlobalProjectile
{
    private BossRushSystem BossRushSystem => ModContent.GetInstance<BossRushSystem>();
    public override void SetDefaults(Projectile entity)
    {
        if (BossRushSystem.IsBossRushMode)
        {
            if (entity.hostile)
            {
                entity.damage = (int)(entity.damage * (BossRushSystem.CurrentBoss.DamageMult ?? 1));
            }
        }
    }
    public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
    {
        if (BossRushSystem.IsBossRushMode)
        {
            if (projectile.hostile)
            {
                modifiers.IncomingDamageMultiplier *= BossRushSystem.CurrentBoss.DamageMult ?? 1;
            }
        }
    }
}