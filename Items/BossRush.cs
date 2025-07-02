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
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.BossRush.hjson file.

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
            if (BossRushSystem.isBossRushMode)
            {
                Main.NewText("What the hell are you doing??!?");
                player.KillMe(
                    PlayerDeathReason.ByCustomReason(
                        $"{player.name}"
                    ),
                    99999,
                    2,
                    true
                );
                BossRushSystem.isBossRushMode = false;
                return true;
            }
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            Main.NewText("Welcome to your final challenge");
            BossRushSystem.isBossRushMode = true;
            IEntitySource entitySource = new EntitySource_Parent(player);
            NPC.NewNPC(
                entitySource,
                (int)player.position.X + 300,
                (int)player.position.Y + 300,
                NPCID.KingSlime
            );

            return true;
        }
    }
}
