using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BossRush.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BossRush.Players
{
    public class BossRushPlayer : ModPlayer
    {
        public override void Kill(
            double damage,
            int hitDirection,
            bool pvp,
            PlayerDeathReason damageSource
        )
        {
            if (BossRushSystem.isBossRushMode && Globals.timesDiedToBossRush == 3)
            {
                Main.NewText(
                    $"The boss rush be keeping {Main.PlayerList[damageSource.SourcePlayerIndex].Name} big as hell"
                );
            }
        }
    }
}
