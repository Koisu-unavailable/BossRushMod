using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BossRush.Subworlds;
using BossRush.Systems;
using Humanizer;
using SubworldLibrary;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BossRush.VanillaTweaks.Bosses
{
    public class Player : ModPlayer
    {
        // public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        // {
        //     ModContent.GetInstance<BossRushSystem>().playersToSendAway.Enqueue(Player);
        //     BossRush.logger.Debug($"Queue is {ModContent.GetInstance<BossRushSystem>().playersToSendAway.ToArray().Humanize()}");
        // }
        public override void PostUpdate()
        {
            Player.ZoneCrimson = true;
        }

    }   
}