using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace BossRush.Subworlds;

public class GenArenaPass : GenPass
{
    public GenArenaPass()
        : base("Terrain", 1) { }

    protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
    {
        progress.Message = "Generating arena"; // Sets the text displayed for this pass
        Main.worldSurface = Main.maxTilesY - 42; // Hides the underground layer just out of bounds
        Main.rockLayer = Main.maxTilesY; // Hides the cavern layer way out of bounds
        for (int i = 0; i < Main.maxTilesX; i++)
        {
            for (int j = 0; j < Main.maxTilesY; j++)
            {
                progress.Set((j + i * Main.maxTilesY) / (float)(Main.maxTilesX * Main.maxTilesY)); // Controls the progress bar, should only be set between 0f and 1f
                Tile tile = Main.tile[i, j];
                tile.HasTile = true;
                tile.TileType = TileID.Dirt;
            }
        }
        var structure = StructureHelper.API.Generator.GetStructureData(
            "Assets/horridArena",
            ModContent.GetInstance<BossRush>()
        );
        
        var pos = new Point16(Main.spawnTileX - structure.width /2 , Main.spawnTileY - structure.height / 2);
        StructureHelper.API.Generator.GenerateFromData(structure, pos);
    }
}
