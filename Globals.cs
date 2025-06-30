using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BossRush
{
    public static class Globals
    {
        public static bool isBossRushMode = false;
        private static string[] bossRushQuotes =
        [
            "You did well",
            "Other text",
            "SIOJ",
            "FOobar",
            "Bar foo"
        ];
        public static string rdmBossRushQuote
        {
            get { return bossRushQuotes[new Random().Next(0, bossRushQuotes.Length)]; }
        }
    }
}
