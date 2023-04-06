using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleCreationUIManager : Singleton<RuleCreationUIManager>
{
    [Header("ui pieces")]
    [SerializeField, Tooltip("The canvas to be used for the ruleset creation")]
    private GameObject rulesetCanvas;
    [SerializeField, Tooltip("the content holder for the rulesets")]
    private GameObject contentHolder;
    [SerializeField, Tooltip("the rulecondition panel prefab")]
    private GameObject rcPanelPrefab;
    [SerializeField, Tooltip("the rulecondition panels currently being displayed")]
    private List<GameObject> rcPanels;
    [SerializeField, Tooltip("the error text area")]
    private TMPro.TextMeshProUGUI errorText;
    [Header("other settings")]
    [SerializeField, Tooltip("Current list of ruleconditions")]
    private List<TileRuleSet.RuleCondition> currentRCs;
    [SerializeField, Tooltip("the ruleset that is being created/edited in this scene")]
    private TileRuleSet.AllRules trs;
    [SerializeField, Tooltip("the ruleset's name")]
    private string ruleName;
    [SerializeField, Tooltip("a list of all tiletypes for use in creating the ruleset")]
    private List<Tile.TileTypes> tileTypes;
    [SerializeField, Tooltip("a list of all weather types for user in creation")]
    private List<WeatherManager.WeatherTypes> weatherTypes;
    [SerializeField, Tooltip("if the current tile type is allTiles")]
    private bool allTiles = true;
    [SerializeField, Tooltip("the currently selected tile type")]
    private Tile.TileTypes currentTileType = Tile.TileTypes.DeepWater;
    [SerializeField, Tooltip("if the current weather type is nonweather conditions")]
    private bool nonWeather = true;
    [SerializeField, Tooltip("the currently selected weather type")]
    private WeatherManager.WeatherTypes currentWeatherType = WeatherManager.WeatherTypes.Sunny;
    

    // Update is called once per frame
    void Update()
    {
        
    }


    public void StartCreationScene(){
        allTiles = true;
        nonWeather = true;;
        currentTileType = Tile.TileTypes.DeepWater;
        currentWeatherType = WeatherManager.WeatherTypes.Sunny;
        errorText.text = "";
        rulesetCanvas.SetActive(true);
        
    }

    public void StartEditingRuleSet(TileRuleSet ruleSet, string rsName = null){
        StartCreationScene();
        if(rsName == null){
            rsName = ruleSet.getRSName();
        }
        ruleName = rsName;
        trs = ruleSet.getRuleset();
    }

    public void StartCreatingRuleSet(){
        
        trs = new TileRuleSet.AllRules();
        trs.allTilesRules = new TileRuleSet.AllTilesRuleset();
        trs.allTilesRules.nonWeatherConditions = new List<TileRuleSet.RuleCondition>();
        trs.allTilesRules.rules = new List<TileRuleSet.Rule>();
        foreach(WeatherManager.WeatherTypes weather in weatherTypes){
            TileRuleSet.Rule thisRule = new TileRuleSet.Rule();
            thisRule.weatherType = weather;
            thisRule.ruleConditions = new List<TileRuleSet.RuleCondition>();
            trs.allTilesRules.rules.Add(thisRule);
        }
        trs.tileRules = new List<TileRuleSet.Ruleset>();
        foreach(Tile.TileTypes tileType in tileTypes){
            TileRuleSet.Ruleset thisRS = new TileRuleSet.Ruleset();
            thisRS.tileType = tileType;
            thisRS.nonWeatherConditions = new List<TileRuleSet.RuleCondition>();
            thisRS.rules = new List<TileRuleSet.Rule>();
            foreach(WeatherManager.WeatherTypes weather in weatherTypes){
                TileRuleSet.Rule thisRule = new TileRuleSet.Rule();
                thisRule.weatherType = weather;
                thisRule.ruleConditions = new List<TileRuleSet.RuleCondition>();
                thisRS.rules.Add(thisRule);
            }
        }
        StartCreationScene();
        RefreshRuleConditionsView();
        

    }

    public void RefreshRuleConditionsView(){
        ClearRuleConditions();
        if(allTiles){
            if(nonWeather){
                currentRCs = trs.allTilesRules.nonWeatherConditions;
                DisplayRuleConditions(currentRCs);
            }
            else{
                foreach(TileRuleSet.Rule r in trs.allTilesRules.rules){
                    if(r.weatherType == currentWeatherType){
                        currentRCs = r.ruleConditions;
                        DisplayRuleConditions(currentRCs);
                    }
                }
            }
        }
        else{
            foreach(TileRuleSet.Ruleset rs in trs.tileRules){
                if(rs.tileType == currentTileType){
                    if(nonWeather){
                        currentRCs = rs.nonWeatherConditions;
                        DisplayRuleConditions(currentRCs);
                    }
                    else{
                        foreach(TileRuleSet.Rule r in rs.rules){
                            if(r.weatherType == currentWeatherType){
                                currentRCs = r.ruleConditions;
                                DisplayRuleConditions(currentRCs);
                            }
                        }
                    }
                }
            }
        }
    }

    public void ClearRuleConditions(){
        while(rcPanels.Count > 0){
            GameObject thisGO = rcPanels[0];
            rcPanels.RemoveAt(0);
            Destroy(thisGO);
        }
    }

    public void DisplayRuleConditions(List<TileRuleSet.RuleCondition> rcs){
        foreach(TileRuleSet.RuleCondition rc in rcs){
            CreateNewPanel(rc);
        }

    }

    public void CreateNewPanel(TileRuleSet.RuleCondition rc){
        GameObject newPanel = Instantiate(rcPanelPrefab,contentHolder.transform);
        rcPanels.Add(newPanel);
        newPanel.GetComponent<RulePanelUI>().setRC(rc);
    }

    public void AddNewRuleCondition(){
        TileRuleSet.RuleCondition newRC = getNewRuleCondition();
        currentRCs.Add(newRC);
        CreateNewPanel(newRC);
    }

    public void DeleteRuleCondition(int index){
        GameObject thisPanel = rcPanels[index];
        rcPanels.RemoveAt(index);
        currentRCs.RemoveAt(index);
    }

    public void DeleteRuleCondition(GameObject rcPanel){
        int deleteIndex = rcPanels.IndexOf(rcPanel);
        if(deleteIndex < 0){
            return;
        }
        DeleteRuleCondition(deleteIndex);
    }

    public void UpdateRuleCondition(GameObject rcPanel){
        int updateIndex = rcPanels.IndexOf(rcPanel);
        if(updateIndex < 0){
            return;
        }
        currentRCs[updateIndex] = rcPanel.GetComponent<RulePanelUI>().getRC();
    }

    public TileRuleSet.RuleCondition getNewRuleCondition(){
        TileRuleSet.RuleCondition result = new TileRuleSet.RuleCondition();
        result.conditional = TileRuleSet.Operators.Always;
        result.NumTiles = 0;
        result.goalTile = Tile.TileTypes.DeepWater;
        result.TilesToCheck = new List<Tile.TileTypes>();
        return result;
    }

    public void saveRuleSet(){
        if(ruleName == null || ruleName == ""){
            SceneLeaveFailure("Cannot have empty name");
        }
        bool saveSuccess = RuleSetManager.Instance.CreateRuleSet(trs, ruleName);
        if(saveSuccess){
            LeaveScene();
        }
        else{
            SceneLeaveFailure("Ruleset with name " + ruleName + " already exists");
        }
    }

    public void LeaveScene(){
        rulesetCanvas.SetActive(false);
    }

    public void SceneLeaveFailure(string reason){
        Debug.LogWarning(reason);
        errorText.text = reason;
    }
}
