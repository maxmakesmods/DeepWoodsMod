using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;

namespace DeepWoodsMod
{
    public class WoodsObelisk : Building
    {
        public WoodsObelisk()
           : base()
        {
        }

        public WoodsObelisk(Vector2 tile)
            : base(WoodsObeliskHelper.WoodsObeliskBuildingData.BuildingType, tile)
        {
        }

        public override BuildingData GetData()
        {
            return WoodsObeliskHelper.WoodsObeliskBuildingData;
        }



        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            if (!who.IsLocalPlayer)
            {
                return false;
            }

            if (who.isRidingHorse())
            {
                return false;
            }

            if (isTilePassable(tileLocation))
            {
                return false;
            }

            // no clue, copied from game code
            if (who.ActiveObject != null
                && who.ActiveObject.IsFloorPathItem()
                && who.currentLocation != null
                && !who.currentLocation.terrainFeatures.ContainsKey(tileLocation))
            {
                return false;
            }

            Game1.activeClickableMenu = new WoodsObeliskMenu();
            return true;
        }

    }
}
