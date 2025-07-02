using BossRush.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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
            ModContent.GetInstance<BossRushSystem>().StartBossRush(player);
            return true;
        }
    }
}
