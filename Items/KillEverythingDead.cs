using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace BossRush.Items
{
    public class KillEverythingDead : ModItem
    {
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.BossRush.hjson file.

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.Zenith);
            Item.damage = 1900;
        }



    }
}
