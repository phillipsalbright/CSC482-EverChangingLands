using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileRuleSet : MonoBehaviour
{
    public enum Operators{
        Greater,GreaterEquals,Equals,LessEquals,Less,NotEquals,Always
    }

    [Serializable]
    public struct AllRules{
        [Tooltip("the rules to run for all tiles, regardless of tile type")]
        public AllTilesRuleset allTilesRules;
        [Tooltip("the rules to run for specific tile types")]
        public List<Ruleset> tileRules;
    }

    [Serializable]
    public struct AllTilesRuleset{
        [Tooltip("the set of rule conditions that run regardless of weather")]
        public List<RuleCondition> nonWeatherConditions;
        [Tooltip("the set of rules for this ruleset")]
        public List<Rule> rules;
    }

    [Serializable]
    public struct Ruleset{
        [Tooltip("the starting tile type for this ruleset")]
        public Tile.TileTypes tileType;
        [Tooltip("the set of rule conditions that run regardless of weather")]
        public List<RuleCondition> nonWeatherConditions;
        [Tooltip("the set of rules for this ruleset")]
        public List<Rule> rules;
    }

    [Serializable]
    public struct Rule{
        [Tooltip("the weather type for this rule")]
        public WeatherManager.WeatherTypes weatherType;
        [Tooltip("The set of all rule conditions for this weather type. \n Must be put in order of priority")]
        public List<RuleCondition> ruleConditions;
    }

    [Serializable]
    public struct RuleCondition{
        [Tooltip("All of the tile types to count for this rule condition.\nFor example, only selecting Forest will only count the number of surrounding forest tile,\n While selecting Forest and Grass will sum the number of surrounding forest tiles and the number of surrounding grass tiles.")]
        public List<Tile.TileTypes> TilesToCheck;
        [Tooltip("The operator to use for this Rule condition.")]
        public Operators conditional;
        [Tooltip("The number to compare the tile counts against"), Range(0,4)]
        public int NumTiles;
        [Tooltip("The tile that will be changed into if this condition is met")]
        public Tile.TileTypes goalTile;
    }

    [SerializeField]
    private AllRules currentRuleset;
    [SerializeField]
    private String rsName;

    private Dictionary<Operators, string> operatorMap;

    void Awake(){
        operatorMap = new Dictionary<Operators, string>();
        operatorMap.Add(Operators.Greater, "more than ");
        operatorMap.Add(Operators.GreaterEquals, "at least ");
        operatorMap.Add(Operators.Equals, "exactly ");
        operatorMap.Add(Operators.LessEquals, "at most ");
        operatorMap.Add(Operators.Less, "less than ");
        operatorMap.Add(Operators.NotEquals, "not ");
    }
    void Start(){
        

        //Debug.Log(toString());
    }

    public void SetupFromRSS(RuleSetSave rss){
        SetRuleSet(rss.getRuleset());
        setRSName(rss.getRSName());
    }

    public String getRSName(){
        return rsName;
    }
    public void setRSName(String newName){
        rsName = newName;
    }

    public void SetRuleSet(AllRules ar){
        currentRuleset = ar;
    }

    public AllRules getRuleset(){
        return currentRuleset;
    }

    //returns a string of the entire current ruleset
    public String toString(){
        String result = "RULESET: " + rsName + "\n\n";
        result += allTileRulesString(currentRuleset.allTilesRules);
        foreach(Ruleset rs in currentRuleset.tileRules){
            result += ruleSetString(rs);
        }
        return result;
    }

    //returns all rules that will run under the current weather conditions
    public String toStringForWeather(WeatherManager.WeatherTypes thisWeather){
        String result = "RULESET:\n\n";
        result += allTileRulesStringForWeather(currentRuleset.allTilesRules, thisWeather);
        foreach(Ruleset rs in currentRuleset.tileRules){
            result += ruleSetStringForWeather(rs, thisWeather);
        }
        return result;
    }

    //turns the ruleset of rules that apply to all tile types into a string
    public String allTileRulesString(AllTilesRuleset atrs){
        String result = "For all tiles, check the following rules:\n";
        result += "For all weather conditions, check the following rules:\n";
        foreach(RuleCondition rc in atrs.nonWeatherConditions){
            result += ruleConditionString(rc);
        }
        result += "\n";
        foreach(Rule r in atrs.rules){
            result += ruleString(r);
        }
        result += "\n";
        return result;
    }

    //turns the ruleset of rules that apply to all tile types for a given weatherinto a string
    public String allTileRulesStringForWeather(AllTilesRuleset atrs, WeatherManager.WeatherTypes thisWeather){
        String result = "For all tiles, check the following rules:\n";
        foreach(RuleCondition rc in atrs.nonWeatherConditions){
            result += ruleConditionString(rc);
        }
        result += "\n";
        foreach(Rule r in atrs.rules){
            if(r.weatherType == thisWeather){
                result += ruleStringForWeather(r);
            }
        }
        result += "\n";
        return result;
    }

    //turns a ruleset into a string
    public String ruleSetString(Ruleset rs){
        String result = "For Tile type " + TileManager.Instance.getTileNameString(rs.tileType) + ", check the following rules:\n";
        result += "For all weather conditions, check the following rules:\n";
        foreach(RuleCondition rc in rs.nonWeatherConditions){
            result += ruleConditionString(rc);
        }
        foreach(Rule r in rs.rules){
            result += ruleString(r);
        }
        result += "\n";
        return result;
    }

    //turns a ruleset into a string of rules that run on a given weather
    public String ruleSetStringForWeather(Ruleset rs, WeatherManager.WeatherTypes thisWeather){
        String result = "For Tile type " + TileManager.Instance.getTileNameString(rs.tileType) + ", check the following rules:\n";
        foreach(RuleCondition rc in rs.nonWeatherConditions){
            result += ruleConditionString(rc);
        }
        foreach(Rule r in rs.rules){
            if(r.weatherType == thisWeather){
                result += ruleStringForWeather(r);
            }
        }
        result += "\n";
        return result;
    }

    //turns a rule condition into a string
    public String ruleConditionString(RuleCondition rc){
        string result = "";
        if(rc.conditional == Operators.Always){
            result = "Always ";
        }
        else{
            result = "If count of adjacent ";
            for( int i = 0; i < rc.TilesToCheck.Count; i++){
                result += TileManager.Instance.getTileNameString(rc.TilesToCheck[i]);
                if(i == rc.TilesToCheck.Count - 2){
                    result += ", and ";
                }
                else if(i < rc.TilesToCheck.Count - 1){
                    result += ", ";
                }         
            }
            result += " tiles is " + operatorMap[rc.conditional] + rc.NumTiles + ",";
           
            
        }
        result += "change to " + TileManager.Instance.getTileNameString(rc.goalTile) + ".\n";
        return result;
    }

    //turns a rule into a string
    public String ruleString(Rule r){
        string result = "When weather is " + WeatherManager.Instance.getWeatherNameString(r.weatherType) + ", check the following conditions:\n";
        foreach(RuleCondition rc in r.ruleConditions){
            result += ruleConditionString(rc);
        }
        result += "\n";
        return result;
    }   

    //turns a rule into a string for a given weather type
    public String ruleStringForWeather(Rule r){
        string result = "";
        foreach(RuleCondition rc in r.ruleConditions){
            result += ruleConditionString(rc);
        }
        result += "\n";
        return result;
    }   
}
