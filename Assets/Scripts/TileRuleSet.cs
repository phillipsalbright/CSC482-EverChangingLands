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

    public AllRules getRuleset(){
        return currentRuleset;
    }
}