using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace BossRush.utils;

public static class PlayerUtils
{
    /// <summary>
    /// Distance from the player where bosses cannot be spawned
    /// </summary>
    private static readonly Vector2 SafeZone = new(20, 20);

    // TODO: Fix BEFORE release
    public static Vector2 GetValidBossSpawnPostion(this Player player)
    {
        bool foundPos = false;
        Vector2 spawnPos =
            player.Center.ToTileCoordinates().ToVector2()
            + SafeZone
            + new Vector2(Random.Shared.Next((int)SafeZone.X * 2));
        int timesSearched = 1;
        while (!foundPos)
        {
            spawnPos =
                player.Center.ToTileCoordinates().ToVector2()
                + SafeZone
                + new Vector2(Random.Shared.Next((int)SafeZone.X * timesSearched));
            ;
            if (!Main.tile[spawnPos.ToPoint()].HasTile)
            {
                foundPos = true;
            }
            ++timesSearched;
        }
        return spawnPos;
    }

}
