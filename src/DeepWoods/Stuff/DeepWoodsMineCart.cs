
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod.Stuff
{
    public class DeepWoodsMineCart
    {
        public static Vector2 MineCartLocation => new Vector2(Settings.Map.RootLevelEnterLocation.X - Settings.Map.ExitRadius - 2 - 6, Settings.Map.RootLevelEnterLocation.Y + 5);

        public static string DeepWoodsMineCartId => "maxvollmer.deepwoods.DeepWoodsMineCart";

        public static void AddDeepWoodsMineCart(DeepWoods location)
        {
            AddMinecartTile(location, MineCartLocation, 0, -1, 933);    // top of minecart
            AddMinecartTile(location, MineCartLocation, 0, 0, 958);     // bottom of minecart
            AddMinecartTile(location, MineCartLocation, 1, 0, 1080);    // left of minecart controls
            AddMinecartTile(location, MineCartLocation, 2, 0, 1081);    // right of minecart controls

            // TODO: Steam
            //AddSteamTile(location, tileLocation, 2, 0, 1081);       // right of minecart controls

            for (int i = 0; i < MineCartLocation.Y; i++)
            {
                AddTrackTile(location, MineCartLocation, 0, -i, 1076);  // track
            }
            AddTrackTile(location, MineCartLocation, 0, 1, 1075);       // end of track
        }

        private static Tile GetOrCreateTile(DeepWoods location, string layerName, int tileX, int tileY, int tileIndex)
        {
            var layer = location.map.GetLayer(layerName);
            var tile = layer.Tiles[tileX, tileY];
            if (tile == null)
            {
                tile = new StaticTile(layer, location.map.GetTileSheet(DeepWoodsGlobals.DEFAULT_OUTDOOR_TILESHEET_ID), BlendMode.Alpha, tileIndex);
                layer.Tiles[tileX, tileY] = tile;
            }
            return tile;
        }

        private static void AddTrackTile(DeepWoods location, Vector2 tileLocation, int x, int y, int tileIndex)
        {
            int tileX = (int)(tileLocation.X + x);
            int tileY = (int)(tileLocation.Y + y);
            var tile = GetOrCreateTile(location, "Back", tileX, tileY, tileIndex);
            tile.TileIndex = tileIndex;
        }

        private static void AddMinecartTile(DeepWoods location, Vector2 tileLocation, int x, int y, int tileIndex)
        {
            int tileX = (int)(tileLocation.X + x);
            int tileY = (int)(tileLocation.Y + y);
            var tile = GetOrCreateTile(location, "Buildings", tileX, tileY, tileIndex);
            tile.Properties.Remove("Action");
            tile.Properties.Add("Action", $"MinecartTransport Default {DeepWoodsMineCartId}");
            tile.TileIndex = tileIndex;
        }
    }
}
