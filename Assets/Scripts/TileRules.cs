using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  TO ADD A NEW TERRAIN TYPE: 
//  FIRST ADD IT TO THE TILE ENUM, 
//  THEN ADD A NEW STATIC RULES METHOD, 
//  THEN ADD A NEW CASE TO THE SWITCH 
public static class TileRules
{
    public static TileRuleSet ruleSet;
    public static bool debug = false;
    public static bool useInspectorRules = false;

    public static Dictionary<Tile.TileTypes, Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>>> inspectorRules;
    public static Dictionary<Tile.TileTypes, List<TileRuleSet.RuleCondition>> AllWeatherInspectorRules;
    public static Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>> AllTileInspectorRules;
    public static List<TileRuleSet.RuleCondition> UniversalInspectorRules;


    public static void SetRuleSet(TileRuleSet newRuleSet){
        if(newRuleSet == null){
            Debug.Log("ruleset was null");
            return;
        }
        ruleSet = newRuleSet;
        Debug.Log("new rules set");
        useInspectorRules = true;
        inspectorRules = new Dictionary<Tile.TileTypes, Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>>>();
        AllWeatherInspectorRules = new Dictionary<Tile.TileTypes, List<TileRuleSet.RuleCondition>>();
        AllTileInspectorRules = new Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>>();
        TileRuleSet.AllRules rulesToSet = ruleSet.getRuleset();

        if(rulesToSet.tileRules != null && rulesToSet.tileRules.Count > 0){
            foreach(TileRuleSet.Ruleset rs in rulesToSet.tileRules){
                AllWeatherInspectorRules.Add(rs.tileType, new List<TileRuleSet.RuleCondition>());
                foreach(TileRuleSet.RuleCondition wrc in rs.nonWeatherConditions){
                    AllWeatherInspectorRules[rs.tileType].Add(wrc);
                }
                inspectorRules.Add(rs.tileType, new Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>>());
                foreach(TileRuleSet.Rule r in rs.rules){
                    inspectorRules[rs.tileType].Add(r.weatherType,new List<TileRuleSet.RuleCondition>());
                    foreach(TileRuleSet.RuleCondition rc in r.ruleConditions){
                        inspectorRules[rs.tileType][r.weatherType].Add(rc);
                    }
                }
            }
            foreach(TileRuleSet.Rule tr in rulesToSet.allTilesRules.rules){
                AllTileInspectorRules[tr.weatherType] = new List<TileRuleSet.RuleCondition>();
                foreach(TileRuleSet.RuleCondition trc in tr.ruleConditions){
                    AllTileInspectorRules[tr.weatherType].Add(trc);
                }
            }
            UniversalInspectorRules = new List<TileRuleSet.RuleCondition>();
            foreach(TileRuleSet.RuleCondition urc in rulesToSet.allTilesRules.nonWeatherConditions){
                UniversalInspectorRules.Add(urc);
            }
        }
    }


    public static bool CheckConditional(int firstNumber, TileRuleSet.Operators conditional, int secondNumber){
        switch(conditional){
            case TileRuleSet.Operators.Greater:
                return firstNumber > secondNumber;
            case TileRuleSet.Operators.GreaterEquals:
                return firstNumber >= secondNumber;
            case TileRuleSet.Operators.Equals:
                return firstNumber == secondNumber;
            case TileRuleSet.Operators.Less:
                return firstNumber < secondNumber;
            case TileRuleSet.Operators.LessEquals:
                return firstNumber <= secondNumber;
            case TileRuleSet.Operators.NotEquals:
                return firstNumber != secondNumber;
            case TileRuleSet.Operators.Always:
            //Debug.Log("always conditional");
                return true;
            default:
                return false;
        }
    }

    public static Tile.TileTypes GetNewTileInspectorRules(Dictionary<Tile.TileTypes, int> adjacentCounts, WeatherManager.WeatherTypes weather, Tile.TileTypes currentType){
        //first, check all universal tile rules
        foreach(TileRuleSet.RuleCondition urc in UniversalInspectorRules){
            //Debug.Log("Universal rule");
            int count = 0;
            foreach(Tile.TileTypes type in urc.TilesToCheck){
                count += adjacentCounts[type];
            }
            if(CheckConditional(count, urc.conditional, urc.NumTiles)){
                return urc.goalTile;
            }
        }
        //then, check all rules that affect all tiles with the current weather
        List<TileRuleSet.RuleCondition> allTileConditions;
        if(AllTileInspectorRules.TryGetValue(weather, out allTileConditions)){
            foreach(TileRuleSet.RuleCondition trc in allTileConditions){
                int count = 0;
                foreach(Tile.TileTypes type in trc.TilesToCheck){
                    count += adjacentCounts[type];
                }
                if(CheckConditional(count, trc.conditional, trc.NumTiles)){
                    return trc.goalTile;
                }
            }
        }
        //then, check all rules that affect the current tile regardless of weather
        List<TileRuleSet.RuleCondition> allWeatherConditions;
        if(AllWeatherInspectorRules.TryGetValue(currentType, out allWeatherConditions)){
            foreach(TileRuleSet.RuleCondition wrc in allWeatherConditions){
                int count = 0;
                foreach(Tile.TileTypes type in wrc.TilesToCheck){
                    count += adjacentCounts[type];
                }
                if(CheckConditional(count, wrc.conditional, wrc.NumTiles)){
                    return wrc.goalTile;
                }
            }
        }
        //then, check the rules for the current tile and weather combo
        Dictionary<WeatherManager.WeatherTypes, List<TileRuleSet.RuleCondition>> dict;
        List<TileRuleSet.RuleCondition> conditions;
        if(inspectorRules.TryGetValue(currentType, out dict) && dict.TryGetValue(weather, out conditions)){
            foreach(TileRuleSet.RuleCondition rc in conditions){
                int count = 0;
                foreach(Tile.TileTypes type in rc.TilesToCheck){
                    count += adjacentCounts[type];
                }
                if(CheckConditional(count, rc.conditional, rc.NumTiles)){
                    return rc.goalTile;
                }
            }
        }
        return currentType;
    }

    public static Tile.TileTypes GetNewTileType(Tile.TileTypes startingType, Tile[,] adjacentTiles){
        if(debug){
            Debug.Log("check tile of " + startingType);
        }
        //first, check the number of surrounding tiles of each terrain type for use in rule checking.
        Dictionary<Tile.TileTypes, int> terrainCounts =  new Dictionary<Tile.TileTypes, int>();
        foreach(Tile tile in adjacentTiles){
            if (tile == null){
                if(debug){
                    Debug.Log("tile null");
                }
                continue;
            }
            if(debug){
                Debug.Log(tile.GetCurrentTileType());
            }
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
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Rocks)){
            terrainCounts.Add(Tile.TileTypes.Rocks, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.Mountain)){
            terrainCounts.Add(Tile.TileTypes.Mountain, 0);
        }
        if(!terrainCounts.ContainsKey(Tile.TileTypes.SnowPeak)){
            terrainCounts.Add(Tile.TileTypes.SnowPeak, 0);
        }
        if (!terrainCounts.ContainsKey(Tile.TileTypes.Fire))
        {
            terrainCounts.Add(Tile.TileTypes.Fire, 0);
        }

        //get the current weather
        WeatherManager.WeatherTypes currentWeather = WeatherManager.Instance.GetCurrentWeather();
        //now, check which set of rules to run through.
        //the switch statement determines which terrain-specific rules method to call
        if(useInspectorRules){
            return GetNewTileInspectorRules(terrainCounts, currentWeather, startingType);
        }
        else{
            switch(startingType){
                case Tile.TileTypes.Water:
                    return GetNewTileWater(terrainCounts, currentWeather, startingType);
                    
                case Tile.TileTypes.Mud:
                    return GetNewTileMud(terrainCounts, currentWeather, startingType);
                    
                case Tile.TileTypes.Desert:
                    return GetNewTileDesert(terrainCounts, currentWeather, startingType);
                    
                case Tile.TileTypes.Dirt:
                    return GetNewTileDirt(terrainCounts, currentWeather, startingType);
                    
                case Tile.TileTypes.Forest:
                    return GetNewTileForest(terrainCounts, currentWeather, startingType);
                    
                case Tile.TileTypes.Grass:
                    return GetNewTileGrass(terrainCounts, currentWeather, startingType);
                    
                default:
                    return startingType;
            }
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
                else if(adjacentCounts[Tile.TileTypes.Dirt] >= 4){
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
                if(adjacentCounts[Tile.TileTypes.Dirt] + adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] + adjacentCounts[Tile.TileTypes.Water] >= 4){
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
                if(adjacentCounts[Tile.TileTypes.Dirt] + adjacentCounts[Tile.TileTypes.Desert] >= 3){
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
                if(adjacentCounts[Tile.TileTypes.Mud] + adjacentCounts[Tile.TileTypes.Water] >= 3){
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
                    return Tile.TileTypes.Mud;
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
                if(adjacentCounts[Tile.TileTypes.Dirt] + adjacentCounts[Tile.TileTypes.Desert] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Desert] >= 3){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Grass] + adjacentCounts[Tile.TileTypes.Forest] >= 4){
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
                if(adjacentCounts[Tile.TileTypes.Dirt] + adjacentCounts[Tile.TileTypes.Desert] >= 3){
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
                if(adjacentCounts[Tile.TileTypes.Grass] + adjacentCounts[Tile.TileTypes.Forest]  >= 3){
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
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                break;
            case WeatherManager.WeatherTypes.Rain:
                if(adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Grass] >= 4){
                    return Tile.TileTypes.Dirt;
                }
                if(adjacentCounts[Tile.TileTypes.Water] >= 4){
                    return Tile.TileTypes.Mud;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] >= 4){
                    return Tile.TileTypes.Dirt;
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
                if(adjacentCounts[Tile.TileTypes.Grass] + adjacentCounts[Tile.TileTypes.Forest] >= 4){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] + adjacentCounts[Tile.TileTypes.Water] >= 4){
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
                if(adjacentCounts[Tile.TileTypes.Grass] + adjacentCounts[Tile.TileTypes.Forest] >= 3){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Forest] >= 2){
                    return Tile.TileTypes.Grass;
                }
                if(adjacentCounts[Tile.TileTypes.Mud] + adjacentCounts[Tile.TileTypes.Water] >= 3){
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
