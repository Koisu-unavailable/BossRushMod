using log4net;
using Terraria;
using Terraria.ModLoader;

namespace BossRush
{
    public class BossRush : Mod
    {
        public static ILog logger => ModContent.GetInstance<BossRush>().Logger;
    }
}
