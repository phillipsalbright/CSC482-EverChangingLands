using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelUI : MonoBehaviour
{
    [SerializeField, Tooltip("the rulecondition for this panel")]
    private TileRuleSet.RuleCondition thisRC;
    [SerializeField, Tooltip("the inputfield for number of tiles requirement")]
    private TMPro.TMP_InputField numTilesInput;
    [SerializeField, Tooltip("the dropdown for the operators")]
    private TMPro.TMP_Dropdown operatorDropDown;
    [SerializeField, Tooltip("the dropdown for goal tile")]
    private TMPro.TMP_Dropdown goalTileDropDown;
    [SerializeField, Tooltip("the toggles for tiles to check")]
    private List<Toggle> tilesToCheckToggles;

    public void Setup(){
        if(tilesToCheckToggles == null){
            tilesToCheckToggles = new List<Toggle>();
        }
        numTilesInput.text = thisRC.NumTiles.ToString();
        
        switch (thisRC.goalTile)
        {
            case Tile.TileTypes.DeepWater:
                goalTileDropDown.value = 0;
                break;
            case Tile.TileTypes.Water:
                goalTileDropDown.value = 1;
                break;
            case Tile.TileTypes.Forest:
                goalTileDropDown.value = 2;
                break;
            case Tile.TileTypes.Grass:
                goalTileDropDown.value = 3;
                break;
            case Tile.TileTypes.Desert:
                goalTileDropDown.value = 4;
                break;
            case Tile.TileTypes.Dirt:
                goalTileDropDown.value = 5;
                break;
            case Tile.TileTypes.SnowPeak:
                goalTileDropDown.value = 6;
                break;
            case Tile.TileTypes.Mountain:
                goalTileDropDown.value = 7;
                break;
            case Tile.TileTypes.Rocks:
                goalTileDropDown.value = 8;
                break;
            case Tile.TileTypes.Fire:
                goalTileDropDown.value = 9;
                break;
            default:
                goalTileDropDown.value = 0;
                break;
        }

        switch(thisRC.conditional){
            case TileRuleSet.Operators.Always:
                operatorDropDown.value = 0;
                break;
            case TileRuleSet.Operators.Greater:
                operatorDropDown.value = 1;
                break;
            case TileRuleSet.Operators.Less:
                operatorDropDown.value = 2;
                break;
            case TileRuleSet.Operators.LessEquals:
                operatorDropDown.value = 3;
                break;
            case TileRuleSet.Operators.GreaterEquals:
                operatorDropDown.value = 4;
                break;
            case TileRuleSet.Operators.Equals:
                operatorDropDown.value = 5;
                break;
            case TileRuleSet.Operators.NotEquals:
                operatorDropDown.value = 6;
                break;
            default:
                operatorDropDown.value = 0;
                break;
        }

        foreach(Tile.TileTypes thisTileTypes in thisRC.TilesToCheck){
            switch (thisTileTypes)
            {
                case Tile.TileTypes.DeepWater:
                    tilesToCheckToggles[0].isOn = true;
                    break;
                case Tile.TileTypes.Water:
                    tilesToCheckToggles[1].isOn = true;
                    break;
                case Tile.TileTypes.Forest:
                    tilesToCheckToggles[2].isOn = true;
                    break;
                case Tile.TileTypes.Grass:
                    tilesToCheckToggles[3].isOn = true;
                    break;
                case Tile.TileTypes.Desert:
                    tilesToCheckToggles[4].isOn = true;
                    break;
                case Tile.TileTypes.Dirt:
                    tilesToCheckToggles[5].isOn = true;
                    break;
                case Tile.TileTypes.SnowPeak:
                    tilesToCheckToggles[6].isOn = true;
                    break;
                case Tile.TileTypes.Mountain:
                    tilesToCheckToggles[7].isOn = true;
                    break;
                case Tile.TileTypes.Rocks:
                    tilesToCheckToggles[8].isOn = true;
                    break;
                case Tile.TileTypes.Fire:
                    tilesToCheckToggles[9].isOn = true;
                    break;
            }
        }
        
    }

    public void setRC(TileRuleSet.RuleCondition newRC){
        thisRC = newRC;
    }

    public TileRuleSet.RuleCondition getRC(){
        return thisRC;
    }

    public void ToggleWater(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Water);
        }
        else{
            RemoveTileType(Tile.TileTypes.Water);
        }
    }

    public void ToggleDeepWater(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.DeepWater);
        }
        else{
            RemoveTileType(Tile.TileTypes.DeepWater);
        }
    }

    public void ToggleDesert(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Desert);
        }
        else{
            RemoveTileType(Tile.TileTypes.Desert);
        }
    }

    public void ToggleDirt(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Dirt);
        }
        else{
            RemoveTileType(Tile.TileTypes.Dirt);
        }
    }

    public void ToggleForest(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Forest);
        }
        else{
            RemoveTileType(Tile.TileTypes.Forest);
        }
    }

    public void ToggleGrass(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Grass);
        }
        else{
            RemoveTileType(Tile.TileTypes.Grass);
        }
    }

    public void TogglePeak(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.SnowPeak);
        }
        else{
            RemoveTileType(Tile.TileTypes.SnowPeak);
        }
    }

    public void ToggleMountain(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Mountain);
        }
        else{
            RemoveTileType(Tile.TileTypes.Mountain);
        }
    }

    public void ToggleRocks(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Rocks);
        }
        else{
            RemoveTileType(Tile.TileTypes.Rocks);
        }
    }

    public void ToggleMud(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Mud);
        }
        else{
            RemoveTileType(Tile.TileTypes.Mud);
        }
    }
    public void ToggleFire(Toggle toggle){
        if(toggle.isOn){
            AddTileType(Tile.TileTypes.Fire);
        }
        else{
            RemoveTileType(Tile.TileTypes.Fire);
        }
    }

    public void OperatorDropDownChange(TMPro.TMP_Dropdown dropdown){
        int loc = dropdown.value;
        switch(loc){
            case 0:
                SetOperator(TileRuleSet.Operators.Always);
                break;
            case 1:
                SetOperator(TileRuleSet.Operators.Greater);
                break;
            case 2:
                SetOperator(TileRuleSet.Operators.Less);
                break;
            case 3:
                SetOperator(TileRuleSet.Operators.LessEquals);
                break;
            case 4:
                SetOperator(TileRuleSet.Operators.GreaterEquals);
                break;
            case 5:
                SetOperator(TileRuleSet.Operators.Equals);
                break;
            case 6:
                SetOperator(TileRuleSet.Operators.NotEquals);
                break;
        }
    }

    public void GoalTileDropDownChange(TMPro.TMP_Dropdown dropdown){
        int loc = dropdown.value;
        switch(loc){
            case 0:
                SetGoalTile(Tile.TileTypes.DeepWater);
                break;
            case 1:
                SetGoalTile(Tile.TileTypes.Water);
                break;
            case 2:
                SetGoalTile(Tile.TileTypes.Forest);
                break;
            case 3:
                SetGoalTile(Tile.TileTypes.Grass);
                break;
            case 4:
                SetGoalTile(Tile.TileTypes.Desert);
                break;
            case 5:
                SetGoalTile(Tile.TileTypes.Dirt);
                break;
            case 6:
                SetGoalTile(Tile.TileTypes.SnowPeak);
                break;
            case 7:
                SetGoalTile(Tile.TileTypes.Mountain);
                break;
            case 8:
                SetGoalTile(Tile.TileTypes.Rocks);
                break;
            case 9:
                SetGoalTile(Tile.TileTypes.Fire);
                break;
        }
    }

    public void ChangeNumTile(TMPro.TMP_InputField text){
        int result = 0;
        try{
            result = int.Parse(text.text);
        }
        catch(System.Exception e){
            return;
        }
        if(result < 0){
            result = 0;
            text.text = 0.ToString();
        }
        if(result > 4){
            result = 4;
            text.text = 4.ToString();
        }
        SetNumTile(result);
        
    }

    public void AddTileType(Tile.TileTypes tileType){
        if(!thisRC.TilesToCheck.Contains(tileType)){
            thisRC.TilesToCheck.Add(tileType);
        }
        RuleCreationUIManager.Instance.UpdateRuleCondition(gameObject);
    }

    public void RemoveTileType(Tile.TileTypes tileType){
        if(thisRC.TilesToCheck.Contains(tileType)){
            thisRC.TilesToCheck.Remove(tileType);
        }
        RuleCreationUIManager.Instance.UpdateRuleCondition(gameObject);
    }

    public void SetNumTile(int num){
        thisRC.NumTiles = num;
        RuleCreationUIManager.Instance.UpdateRuleCondition(gameObject);
    }

    public void SetOperator(TileRuleSet.Operators thisOperator){
        thisRC.conditional = thisOperator;
        RuleCreationUIManager.Instance.UpdateRuleCondition(gameObject);
    }
    public void SetGoalTile(Tile.TileTypes tileType){
        thisRC.goalTile = tileType;
        RuleCreationUIManager.Instance.UpdateRuleCondition(gameObject);
    }

    public void DeleteThis(){
        RuleCreationUIManager.Instance.DeleteRuleCondition(gameObject);
    }
}
