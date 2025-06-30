using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
namespace BossRush.Items
{
	public class BossRush : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.BossRush.hjson file.
	
		public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.SuspiciousLookingEye);

			
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
        public override bool? UseItem(Player player)
        {
			if (Globals.isBossRushMode){
				Main.NewText("What the hell are you doing??!?");
				player.KillMe(PlayerDeathReason.ByCustomReason($"{player.name} was slain by being a fucking dumbass"), 99999, 2, true);
				Globals.isBossRushMode = false;
				return true;
			}
			SoundEngine.PlaySound(SoundID.Roar, player.position);
			Main.NewText("Welcome to your final challenge");
			Globals.isBossRushMode = true;
			IEntitySource entitySource = new EntitySource_Parent(player);
			NPC.NewNPC(entitySource,(int)player.position.X+50, (int)player.position.Y+50,NPCID.KingSlime );
			
            return true;
        }
		
    }
}