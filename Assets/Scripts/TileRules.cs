using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO ADD A NEW TERRAIN TYPE: 
//  FIRST ADD IT TO THE TILE ENUM, 
//  THEN ADD A NEW STATIC RULES METHOD, 
//  THEN ADD A NEW CASE TO THE SWITCH 
public static class TileRules
{
    public static Tile.TileTypes GetNewTileType(Tile.TileTypes startingType, Tile[,] adjacentTiles){
        //first, check the number of surrounding tiles of each terrain type for use in rule checking.
        Dictionary<Tile.TileTypes, int> terrainCounts =  new Dictionary<Tile.TileTypes, int>();
        foreach(Tile tile in adjacentTiles){
            Tile.TileTypes currentTileType = tile.GetCurrentTileType();
            bool exists = terrainCounts.ContainsKey(currentTileType);
            if(exists){
                terrainCounts[currentTileType] = terrainCounts[currentTileType] + 1;
            }
            else{
                terrainCounts.Add(currentTileType, 1);
            }
        }

        //make sure all terrain types exist in the count just for simplicity's sake later on
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Desert)){
            terrainCounts.Add(Tile.TileTypes.Desert, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Forest)){
            terrainCounts.Add(Tile.TileTypes.Forest, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Water)){
            terrainCounts.Add(Tile.TileTypes.Water, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.DeepWater)){
            terrainCounts.Add(Tile.TileTypes.DeepWater, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Dirt)){
            terrainCounts.Add(Tile.TileTypes.Dirt, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Grass)){
            terrainCounts.Add(Tile.TileTypes.Grass, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Mud)){
            terrainCounts.Add(Tile.TileTypes.Mud, 0);
        }

        //get the current weather
        WeatherManager.WeatherTypes currentWeather = WeatherManager.Instance.GetCurrentWeather();
        //now, check which set of rules to run through.
        //the switch statement determines which terrain-specific rules method to call
        switch(startingType){
            case Tile.TileTypes.Water:
                return GetNewTileWater(terrainCounts, currentWeather, startingType);
                break;
            case Tile.TileTypes.Mud:
                return GetNewTileMud(terrainCounts, currentWeather, startingType);
                break;
            case Tile.TileTypes.Desert:
                return GetNewTileDesert(terrainCounts, currentWeather, startingType);
                break;
            case Tile.TileTypes.Dirt:
                return GetNewTileDirt(terrainCounts, currentWeather, startingType);
                break;
            case Tile.TileTypes.Forest:
                return GetNewTileForest(terrainCounts, currentWeather, startingType);
                break;
            case Tile.TileTypes.Grass:
                return GetNewTileGrass(terrainCounts, currentWeather, startingType);
                break;
            default:
                return startingType;
        }
    }


    //These are the individual methods for checking rules for each specific tile type

    static Tile.TileTypes GetNewTileForest(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Grass;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                else if(adjacentCounts[Tile.TileTypes.Desert] >= 2){
                    return Tile.TileTypes.Grass;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Grass;
                }
                break;
            default:
                return currentType;
        }
        return currentType;
    }

    static Tile.TileTypes GetNewTileGrass(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Dirt] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 3){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Forest;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Dirt] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 2){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Forest;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 2){
                    return Tile.TileTypes.Forest;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 2){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] >= 3){
                    return Tile.TileTypes.Mud;
                }
                
                break;
            default:
                return currentType;
        }
        return currentType;
    }

    static Tile.TileTypes GetNewTileWater(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Water] < 2){
                    return Tile.TileTypes.Mud;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Water] < 1){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] < 3){
                    return Tile.TileTypes.Mud;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Water] < 1){
                    return Tile.TileTypes.Dirt;
                }
                break;
            default:
                return currentType;
        }
        return currentType;
    }

    static Tile.TileTypes GetNewTileMud(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Water] >= 3){
                    return Tile.TileTypes.Water;
                }
                if(adjacentCounts[Tile.TileTypes.Dirt] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Grass] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Grass;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Water;
                }
                if(adjacentCounts[Tile.TileTypes.Dirt] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 2){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Grass;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Water] >= 2){
                    return Tile.TileTypes.Water;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Grass] >= 3){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 2){
                    return Tile.TileTypes.Grass;
                }
                break;
            default:
                return currentType;
        }
        return currentType;
    }

    static Tile.TileTypes GetNewTileDesert(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 2){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 2){
                    return Tile.TileTypes.Dirt;
                }
                break;
            default:
                return currentType;
        }
        return currentType;
    }

    static Tile.TileTypes GetNewTileDirt(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        if(adjacentCounts[Tile.TileTypes.DeepWater] > 0){
            return Tile.TileTypes.Water;
        }
        switch(weather){
            case WeatherManager.WeatherTypes.Sunny:
                if(adjacentCounts[Tile.TileTypes.Grass] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 3){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Desert;
                }
                break;
            case WeatherManager.WeatherTypes.Drought:
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 2){
                    return Tile.TileTypes.Desert;
                }
                break;
                //FIGURE OUT WHAT HAPPENS WITH 2 WATER 2 FOREST
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Grass] >= 3){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 2){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] >= 3){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 2){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Desert;
                }
                
                break;
            default:
                return currentType;
        }
        return currentType;
    }
}
