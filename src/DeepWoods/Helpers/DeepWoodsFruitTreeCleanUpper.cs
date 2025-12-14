using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;

namespace DeepWoodsMod
{
    internal class DeepWoodsFruitTreeCleanUpper
    {
        private static void ProcessGameLocation(GameLocation location)
        {
            ModEntry.Log("DeepWoodsFruitTreeCleanUpper.ProcessGameLocation: " + location.Name, StardewModdingAPI.LogLevel.Trace);

            foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs.ToList())
            {
                if (pair.Value is SeedLessFruitTree fruitTree)
                {
                    location.terrainFeatures.Remove(pair.Key);
                    var fruitTreeData = fruitTree.GetData();
                    if (fruitTreeData?.Fruit != null)
                    {
                        var fruitData = fruitTreeData.Fruit.FirstOrDefault();
                        if (fruitData != null)
                        {
                            location.objects.Add(pair.Key, new Object(fruitData.ItemId, 1, false, -1, Object.lowQuality)
                            {
                                IsSpawnedObject = true,
                                CanBeSetDown = false,
                                CanBeGrabbed = false,
                                TileLocation = pair.Key,
                            });
                        }
                    }

                }
            }
        }

        public static void CleanUpBeforeSave()
        {
            if (!Game1.IsMasterGame)
                return;

            ModEntry.Log("DeepWoodsFruitTreeCleanUpper.CleanUpBeforeSave()", StardewModdingAPI.LogLevel.Trace);

            foreach (var location in Game1.locations)
            {
                if (location is not DeepWoods)
                {
                    ProcessGameLocation(location);
                }
            }
        }
    }
}
