using log4net;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace BossRush
{
    public class BossRush : Mod
    {
        public static ILog logger => ModContent.GetInstance<BossRush>().Logger;
        public override void Load()
        {
            MusicLoader.AddMusic(this, "Assets/Sound/BAKA");
        }


    }
}
