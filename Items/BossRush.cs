using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using BossRush.Systems;

namespace BossRush.Items
{
    public class BossRush : ModItem
    {
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.SuspiciousLookingEye);
            Item.useStyle = ItemUseStyleID.DrinkLong;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddCondition(Condition.DownedMoonLord);
            recipe.Register();
        }

        public override bool? UseItem(Player player)
        {
            var bossRushSystem = ModContent.GetInstance<BossRushSystem>();
            if (!bossRushSystem.IsBossRushMode)
            {
                bossRushSystem.StartBossRush(player);
                return true;
            }
            else
            {
                var reason = new PlayerDeathReason
                {
                    CustomReason = Language
                        .GetText("Mods.BossRush.Misc.overdosed")
                        .WithFormatArgs(player.name)
                        .ToNetworkText()
                };
                player.KillMe(reason, 2^30, 0);
                return true;
            }
        }
    }
}
