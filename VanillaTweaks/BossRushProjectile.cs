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
            if (entity.hostile) // TODO: 
            {
                // do something with this
                // On_Projectile.NewProjectileDirect += (On_Projectile.orig_NewProjectileDirect orig, Terraria.DataStructures.IEntitySource spawnSource, Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity, int type, int damage, float knockback, int owner, float ai0, float ai1, float ai2) =>
                // {

                // }
                entity.damage *= BossRushSystem.CurrentBoss.DamageMult ?? 1;
            }
        }
    }

}