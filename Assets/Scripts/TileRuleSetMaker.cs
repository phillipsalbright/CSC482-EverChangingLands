using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TileRuleSetMaker
{
    
    public static bool CreateRuleSet(TileRuleSet.AllRules rules){
        try{
            TileRuleSet trs = new TileRuleSet();
            trs.SetRuleSet(rules);
        }
        catch(Exception e){
            Debug.LogError("failed to create new RuleSet. had error: " + e.Message);
            return false;
        }
        return true;
    }

    public static bool CreateAllRules(TileRuleSet.AllTilesRuleset atr, List<TileRuleSet.Ruleset> rs = null){
        if(rs == null){
            rs = new List<TileRuleSet.Ruleset>();
        }
        try{
            TileRuleSet.AllRules atrs = new TileRuleSet.AllRules{allTilesRules = atr, tileRules = rs};
        }
        catch(Exception e){
            Debug.LogError("failed to create new AllRules. had error: " + e.Message);
            return false;
        }
        return true;
    }
    public static bool CreateAllTilesRuleset(List<TileRuleSet.RuleCondition> nwc = null, List<TileRuleSet.Rule> r = null){
        if(nwc == null){
            nwc = new List<TileRuleSet.RuleCondition>();
        }
        if(r == null){
            r = new List<TileRuleSet.Rule>();
        }
        try{
            TileRuleSet.AllTilesRuleset atrs = new TileRuleSet.AllTilesRuleset{nonWeatherConditions = nwc, rules = r};
        }
        catch(Exception e){
            Debug.LogError("failed to create new AllTilesRuleset. had error: " + e.Message);
            return false;
        }
        return true;
    }
    public static bool CreateRuleset(List<TileRuleSet.RuleCondition> nwc = null, List<TileRuleSet.Rule> r = null, Tile.TileTypes tile = Tile.TileTypes.DeepWater){
        if(nwc == null){
            nwc = new List<TileRuleSet.RuleCondition>();
        }
        if(r == null){
            r = new List<TileRuleSet.Rule>();
        }
        try{
            TileRuleSet.Ruleset rs = new TileRuleSet.Ruleset{tileType = tile, nonWeatherConditions = nwc, rules = r};
        }
        catch(Exception e){
            Debug.LogError("failed to create new Ruleset. had error: " + e.Message);
            return false;
        }
        return true;
    }
    public static bool CreateRule(List<TileRuleSet.RuleCondition> rc = null, WeatherManager.WeatherTypes weather = WeatherManager.WeatherTypes.Sunny){
        if(rc == null){
            rc = new List<TileRuleSet.RuleCondition>();
        }
        try{
            TileRuleSet.Rule r = new TileRuleSet.Rule{weatherType = weather, ruleConditions = rc};
        }
        catch(Exception e){
            Debug.LogError("failed to create new Rule. had error: " + e.Message);
            return false;
        }
        return true;
    }
    public static bool CreateRuleCondition(List<Tile.TileTypes> tilesToCheck = null, TileRuleSet.Operators condition = TileRuleSet.Operators.Always, int NumTiles = 0, Tile.TileTypes goal = Tile.TileTypes.DeepWater){
        try{
            if(tilesToCheck == null){
                tilesToCheck = new List<Tile.TileTypes>();
            }
            TileRuleSet.RuleCondition rc = new TileRuleSet.RuleCondition{TilesToCheck = tilesToCheck, conditional = condition, NumTiles = NumTiles, goalTile=goal};
        }
        catch(Exception e){
            Debug.LogError("failed to create new RuleCondition. had error: " + e.Message);
            return false;
        }
        return true;
    }
}
