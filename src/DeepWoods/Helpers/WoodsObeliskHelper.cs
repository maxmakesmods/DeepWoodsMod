using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Delegates;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using System.Linq;
using static DeepWoodsMod.DeepWoodsGlobals;
using static DeepWoodsMod.DeepWoodsSettings;

namespace DeepWoodsMod
{
    public class WoodsObeliskHelper
    {
        public static BuildingData WoodsObeliskBuildingData

        {
            get
            {


                return new()
                {
                    Name = I18N.WoodsObeliskDisplayName,
                    Description = I18N.WoodsObeliskDescription,
                    MagicalConstruction = true,
                    Builder = "Wizard",
                    BuildCost = Settings.Objects.WoodsObelisk.MoneyRequired,
                    BuildMaterials = Settings.Objects.WoodsObelisk.ItemsRequired.Select(i => new BuildingMaterial()
                    {
                        ItemId = i.Key.ToString(),
                        Amount = i.Value
                    }).ToList(),
                    BuildingType = typeof(WoodsObelisk).AssemblyQualifiedName,
                    Texture = "Buildings\\" + WOODS_OBELISK_BUILDING_NAME,
                    Size = new Point(3, 2),
                    //BuildCondition = WOODS_OBELISK_BUILDING_CONDITION
                };
            }
        }

        // [17:14:58 ERROR game] Can't construct building '', no data found matching that ID.

        public static void SendLetterIfNecessaryAndPossible()
        {
            if (DeepWoodsState.LowestLevelReached >= Settings.Level.MinLevelForWoodsObelisk
                && !Game1.player.hasOrWillReceiveMail(WOODS_OBELISK_WIZARD_MAIL_ID)
                && (Game1.player.mailReceived.Contains("hasPickedUpMagicInk") || Game1.player.hasMagicInk))
            {
                Game1.addMailForTomorrow(WOODS_OBELISK_WIZARD_MAIL_ID);
            }
        }

        public static bool CanBuildWoodsObelisk(string[] query, GameStateQueryContext context)
        {
            return DeepWoodsState.LowestLevelReached >= Settings.Level.MinLevelForWoodsObelisk
                && Game1.player.mailReceived.Contains(WOODS_OBELISK_WIZARD_MAIL_ID);
        }

        /*
        public static void InjectWoodsObeliskIntoGame()
        {
            if (DeepWoodsState.LowestLevelReached >= Settings.Level.MinLevelForWoodsObelisk
                && Game1.player.mailReceived.Contains(WOODS_OBELISK_WIZARD_MAIL_ID)
                && Game1.activeClickableMenu is CarpenterMenu carpenterMenu
                && IsMagical(carpenterMenu)
                && !HasBluePrint(carpenterMenu))
            {
                // Add Woods Obelisk directly after the other obelisks
                int lastObeliskIndex = carpenterMenu.Blueprints.FindLastIndex(bluePrint => bluePrint.DisplayName.Contains("Obelisk"));

                CarpenterMenu.BlueprintEntry woodsObeliskBluePrint = new(
                    index: lastObeliskIndex + 1,
                    id: null,
                    data: WoodsObeliskBuildingData,
                    skinId: null
                    );

                carpenterMenu.Blueprints.Insert(lastObeliskIndex + 1, woodsObeliskBluePrint);
            }
        }

        private static bool IsMagical(CarpenterMenu carpenterMenu)
        {
            return carpenterMenu.Blueprints.Exists(b => b.MagicalConstruction);
        }

        private static bool HasBluePrint(CarpenterMenu carpenterMenu)
        {
            return carpenterMenu.Blueprints.Exists(bluePrint => bluePrint.DisplayName == WOODS_OBELISK_BUILDING_NAME);
        }
        */



        private enum ProcessMethod
        {
            Remove,
            Restore
        }

        private static void ProcessFarm(Farm farm, ProcessMethod method)
        {
            ModEntry.Log("WoodsObelisk.ProcessFarm(" + method + ")", StardewModdingAPI.LogLevel.Trace);

            foreach (Building building in farm.buildings.ToList())
            {
                if (building == null)
                {
                    continue;
                }

                if (method == ProcessMethod.Remove && building is WoodsObelisk)
                {
                    farm.buildings.Remove(building);
                    farm.buildings.Add(Building.CreateInstanceFromId("Earth Obelisk", new (building.tileX.Value, building.tileY.Value)));
                    DeepWoodsState.WoodsObeliskLocations.Add(new XY(building.tileX.Value, building.tileY.Value));
                }
                else if (method == ProcessMethod.Restore
                    && building.buildingType.Value.Equals("Earth Obelisk")
                    && DeepWoodsState.WoodsObeliskLocations.Contains(new XY(building.tileX.Value, building.tileY.Value)))
                {
                    farm.buildings.Remove(building);
                    farm.buildings.Add(new WoodsObelisk(new(building.tileX.Value, building.tileY.Value)));
                }
            }
        }

        public static void RemoveAllFromGame()
        {
            if (!Game1.IsMasterGame)
                return;

            ModEntry.Log("WoodsObelisk.RemoveAllFromGame()", StardewModdingAPI.LogLevel.Trace);

            DeepWoodsState.WoodsObeliskLocations.Clear();
            ProcessFarm(Game1.getFarm(), ProcessMethod.Remove);
        }

        public static void RestoreAllInGame()
        {
            if (!Game1.IsMasterGame)
                return;

            ModEntry.Log("WoodsObelisk.RestoreAllInGame()", StardewModdingAPI.LogLevel.Trace);

            ProcessFarm(Game1.getFarm(), ProcessMethod.Restore);
        }
    }
}
