using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulePanelUI : MonoBehaviour
{
    [SerializeField, Tooltip("the rulecondition for this panel")]
    private TileRuleSet.RuleCondition thisRC;


    public void setRC(TileRuleSet.RuleCondition newRC){
        thisRC = newRC;
    }

    public TileRuleSet.RuleCondition getRC(){
        return thisRC;
    }

    public void AddTileType(Tile.TileTypes tileType){
        if(!thisRC.TilesToCheck.Contains(tileType)){
            thisRC.TilesToCheck.Add(tileType);
        }
    }

    public void RemoveTileType(Tile.TileTypes tileType){
        if(thisRC.TilesToCheck.Contains(tileType)){
            thisRC.TilesToCheck.Remove(tileType);
        }
    }

    public void SetNumTile(int num){
        thisRC.NumTiles = num;
    }

    public void SetOperator(TileRuleSet.Operators thisOperator){
        thisRC.conditional = thisOperator;
    }
    public void SetGoalTile(Tile.TileTypes tileType){
        thisRC.goalTile = tileType;
    }
}
