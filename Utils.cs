using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush
{
    public class Utils
    {
        /// <summary>
        /// Attempts to find a valid tile to teleport a player to. Upon failure (i.e. centre is a tile and completely surronded by tiles), it will return the centre of the circle
        /// </summary>
        /// <param name="radius">Radius of the circle to search in</param>
        /// <param name="center">Point representing a valid tile</param>
        /// <returns>A tile to tp the player to</returns>
        public static Vector2 FindValidTpPos(int radius, Vector2 center)
        {
            // ngl this algorithim probably ineffcient asl
            // equation for a circle
            var isValid = (int x, int y) => Math.Pow(x - center.X, 2) + Math.Pow(y - center.Y,2) < Math.Pow(radius, 2);
            for (int x = 0; x < Main.tile.Width; x++)
            {
                for (int y = 0; y < Main.tile.Height; y++)
                {
                    if (isValid(x, y) && !Main.tile[x,y].HasUnactuatedTile)
                    {
                        return new Vector2(x,y);
                    }
                }
            }
            return center; // cooked
        }
    }
}