using log4net;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace BossRush
{
    public class BossRush : Mod
    {
        public static ILog logger => ModContent.GetInstance<BossRush>().Logger;
        public static readonly IAudioTrack Music = MusicLoader.GetMusic("BossRush/Assets/Music/BAKA");

    }
}
