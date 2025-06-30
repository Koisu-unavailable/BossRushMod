using System.Numerics;
using Terraria;
using Terraria.Chat.Commands;
using MXF = Microsoft.Xna.Framework;

namespace BossRush
{
    public interface BossSummonManagment
    {
        public static int[] findValidBossSpawnPos(Player player)
        {
            //Scan in A 500 block circle around player for area that boss can spawn
            MXF.Vector2 playerPos = player.position;
            for (int i = 0; i <= 360; i++) { }
            return new int[] { 1, 2 };
        }

        // TODO: THIS IS NOT SUPPOSED TO BE NULLABLE
        private static MXF.Vector2? rotateVector(MXF.Vector2 vector)
        {
            return null;
        }
    }
}
