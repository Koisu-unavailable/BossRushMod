using BossRush.Subworlds;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
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
            SubworldSystem.Enter<BossArenaSubworld>();
            return true;
        }
    }
}
