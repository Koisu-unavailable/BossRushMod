using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace BossRush
{
    public class Utils
    {
        /// <summary>
        /// Attempts to find a tile surrounded by empty, liquid-free tiles within the search area.
        /// Upon failure, it returns the supplied center clamped to safe world bounds.
        /// </summary>
        /// <param name="searchArea">World-pixel rectangle to search in</param>
        /// <param name="center">Fallback world-pixel position</param>
        /// <param name="safeTilesOnEachSide">Number of surrounding tiles to validate on each side of a candidate</param>
        /// <returns>A world-pixel position to teleport the player to</returns>
        public static Vector2 FindValidTpPos(Rectangle searchArea, Vector2 center, int safeTilesOnEachSide = 1)
        {
            safeTilesOnEachSide = Math.Max(0, safeTilesOnEachSide);
            int minTileX = Math.Max(safeTilesOnEachSide, searchArea.Left / 16);
            int maxTileX = Math.Min(Main.maxTilesX - safeTilesOnEachSide - 1, (searchArea.Right - 1) / 16);
            int minTileY = Math.Max(safeTilesOnEachSide, searchArea.Top / 16);
            int maxTileY = Math.Min(Main.maxTilesY - safeTilesOnEachSide - 1, (searchArea.Bottom - 1) / 16);

            for (int tileX = minTileX; tileX <= maxTileX; tileX++)
            {
                for (int tileY = minTileY; tileY <= maxTileY; tileY++)
                {
                    bool valid = true;
                    for (int offsetX = -safeTilesOnEachSide; offsetX <= safeTilesOnEachSide && valid; offsetX++)
                    {
                        for (int offsetY = -safeTilesOnEachSide; offsetY <= safeTilesOnEachSide; offsetY++)
                        {
                            Tile tile = Main.tile[tileX + offsetX, tileY + offsetY];
                            if (tile.HasTile || tile.LiquidAmount > 0)
                            {
                                valid = false;
                                break;
                            }
                        }
                    }

                    if (valid)
                    {
                        return new Vector2(tileX * 16, tileY * 16);
                    }
                }
            }

            int minWorldX = safeTilesOnEachSide * 16;
            int minWorldY = safeTilesOnEachSide * 16;
            int maxWorldX = Math.Max(minWorldX, (Main.maxTilesX - safeTilesOnEachSide - 1) * 16);
            int maxWorldY = Math.Max(minWorldY, (Main.maxTilesY - safeTilesOnEachSide - 1) * 16);
            return new Vector2(
                MathHelper.Clamp(center.X, minWorldX, maxWorldX),
                MathHelper.Clamp(center.Y, minWorldY, maxWorldY));
        }

        public static bool IsValidTeleportPosition(Vector2 position, Vector2 entitySize)
        {
            return position.X >= 0
                && position.Y >= 0
                && position.X + entitySize.X < Main.maxTilesX * 16
                && position.Y + entitySize.Y < Main.maxTilesY * 16;
        }
    }
}
