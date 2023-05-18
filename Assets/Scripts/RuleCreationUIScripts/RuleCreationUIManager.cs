using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleCreationUIManager : Singleton<RuleCreationUIManager>
{
    [Header("ui pieces")]
    [SerializeField, Tooltip("The main menu script")]
    private MainMenu mainMenuScript;
    [SerializeField, Tooltip("The canvas to be used for the ruleset creation")]
    private GameObject rulesetCanvas;
    [SerializeField, Tooltip("The canvas to be used before ruleset creation")]
    private GameObject rulesetMenuCanvas;
    [SerializeField, Tooltip("The main menu dropdown")]
    private TMPro.TMP_Dropdown rulesetSelectionDropDown;
    [SerializeField, Tooltip("the content holder for the rulesets")]
    private GameObject contentHolder;
    [SerializeField, Tooltip("the rulecondition panel prefab")]
    private GameObject rcPanelPrefab;
    [SerializeField, Tooltip("the rulecondition panels currently being displayed")]
    private List<GameObject> rcPanels;
    [SerializeField, Tooltip("the error text area")]
    private TMPro.TextMeshProUGUI errorText;
    [SerializeField, Tooltip("The container for Tile panel objects")]
    private GameObject tilePanelHolder;
    [SerializeField, Tooltip("the container for weather panel objects")]
    private GameObject weatherPanelHolder;
    [Header("colors")]
    [SerializeField, Tooltip("the default background color")]
    private Color defaultColor = Color.black;
    [SerializeField, Tooltip("the selected color")]
    private Color selectedColor = Color.red;
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
    
    // void Start(){
    //     //StartCreatingRuleSet();
    //     RuleCreationUIManager.Instance.CallThisToExist();
        
    // }

    public void CallThisToExist(){
        //do nothing
        if(mainMenuScript != null){
            mainMenuScript.SetRCUIMan(this);
        }
        Debug.Log("I am beggin you");
    }

    public void SetupMainMenu(){
        LeaveScene();
        rulesetMenuCanvas.GetComponent<RuleMainMenuCanvas>().Setup();
    }

    public void SetDefaultColor(Color startColor){
        if(defaultColor != Color.black){
            return;
        }
        defaultColor = startColor;
    }

    public void StartRSCreator(string trsName, string newRSString){
        Debug.Log("starting rs creator with name: " + trsName + ", and default: " + newRSString);
        if(trsName ==  newRSString){
            StartCreatingRuleSet();
        }
        else{
            TileRuleSet ruleSetToUse = RuleSetManager.Instance.GetRuleSetByName(trsName);
            if(ruleSetToUse == null){
                return;
            }
            StartEditingRuleSet(ruleSetToUse, trsName);
        }
    }


    public void StartCreationScene(){
        allTiles = true;
        nonWeather = true;;
        currentTileType = Tile.TileTypes.DeepWater;
        currentWeatherType = WeatherManager.WeatherTypes.Sunny;
        errorText.text = "";
        rulesetCanvas.SetActive(true);
        rulesetMenuCanvas.SetActive(false);
        SetTilePanelSelected(0);
        SetWeatherPanelSelected(0);
    }

    public void StartEditingRuleSet(TileRuleSet ruleSet, string rsName = null){
        Debug.Log("start editing");
        StartCreationScene();
        if(rsName == null){
            rsName = ruleSet.getRSName();
        }
        ruleName = rsName;
        trs = ruleSet.getRuleset();
    }

    public void StartCreatingRuleSet(){
        Debug.Log("start creating");
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
            trs.tileRules.Add(thisRS);
        }
        StartCreationScene();
        RefreshRuleConditionsView();
        

    }

    public void SetNewWeather(GameObject panelSelected, bool allWeather, WeatherManager.WeatherTypes newWeather = WeatherManager.WeatherTypes.Sunny){
        nonWeather = allWeather;
        currentWeatherType = newWeather;
        RefreshRuleConditionsView();
        SetWeatherPanelSelected(panelSelected);
    }

    public void SetNewTileType(GameObject panelSelected, bool allTileTypes, Tile.TileTypes newTile = Tile.TileTypes.DeepWater){
        allTiles = allTileTypes;
        currentTileType = newTile;
        RefreshRuleConditionsView();
        SetTilePanelSelected(panelSelected);
    }

    public void RefreshRuleConditionsView(){
        Debug.Log("refresh rules view");
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
        Debug.Log("clear rules");
        while(rcPanels.Count > 0){
            GameObject thisGO = rcPanels[0];
            rcPanels.RemoveAt(0);
            Destroy(thisGO);
        }
    }

    public void DisplayRuleConditions(List<TileRuleSet.RuleCondition> rcs){
        // foreach(TileRuleSet.RuleCondition rc in rcs){
        //     CreateNewPanel(rc);
        // }
        for(int i = 0; i < rcs.Count; i ++){
            CreateNewPanel(rcs[i]);
        }

    }

    public void CreateNewPanel(TileRuleSet.RuleCondition rc){
        GameObject newPanel = Instantiate(rcPanelPrefab,contentHolder.transform);
        rcPanels.Add(newPanel);
        newPanel.GetComponent<RulePanelUI>().setRC(rc);
        newPanel.GetComponent<RulePanelUI>().Setup();
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
        Destroy(thisPanel);
    }

    public void DeleteRuleCondition(GameObject rcPanel){
        int deleteIndex = rcPanels.IndexOf(rcPanel);
        if(deleteIndex < 0){
            return;
        }
        DeleteRuleCondition(deleteIndex);
    }

    public void MoveRCUp(GameObject rcPanel){
        int moveIndex = rcPanels.IndexOf(rcPanel);
        if(moveIndex <= 0){
            return;
        }
        setnewIndex(rcPanel, moveIndex, moveIndex-1);
    }

    public void MoveRCDown(GameObject rcPanel){
        int moveIndex = rcPanels.IndexOf(rcPanel);
        if(moveIndex >= currentRCs.Count - 1){
            return;
        }
        setnewIndex(rcPanel, moveIndex, moveIndex + 1);
    }

    public void setnewIndex(GameObject rcPanel, int startIndex, int endIndex){
        rcPanels.RemoveAt(startIndex);
        rcPanels.Insert(endIndex, rcPanel);

        currentRCs.RemoveAt(startIndex);
        currentRCs.Insert(endIndex, rcPanel.GetComponent<RulePanelUI>().getRC());

        rcPanel.transform.SetSiblingIndex(endIndex + 1);

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
            SetupMainMenu();
        }
        else{
            SceneLeaveFailure("Ruleset with name " + ruleName + " already exists");
        }
    }

    public void LeaveScene(){
        if(rulesetCanvas == null){
            Debug.LogWarning("Literally how");
            gameObject.name = "This thing sucks ass";
        }
        rulesetCanvas.SetActive(false);
        rulesetMenuCanvas.SetActive(true);
    }
    public void BackToMain(){
        rulesetCanvas.SetActive(false);
        rulesetMenuCanvas.SetActive(false);
        if(mainMenuScript != null){
            mainMenuScript.ReturnFromCreationCanvas();
        }
    }

    public void UpdateName(TMPro.TMP_InputField text){
        ruleName = text.text;
    }

    public void SceneLeaveFailure(string reason){
        Debug.LogWarning(reason);
        errorText.text = reason;
    }

    public void ClearTileColors(){
        foreach(RulePanelTileUI rpt in tilePanelHolder.transform.GetComponentsInChildren<RulePanelTileUI>()){
            rpt.ChangeColor(defaultColor);
        }
    }

    public void ClearWeatherColors(){
        foreach(RulePanelWeatherUI rpw in weatherPanelHolder.transform.GetComponentsInChildren<RulePanelWeatherUI>()){
            rpw.ChangeColor(defaultColor);
        }
    }

    public void SetTilePanelSelected(GameObject selected){
        ClearTileColors();
        selected.GetComponent<RulePanelTileUI>().ChangeColor(selectedColor);
    }
    public void SetTilePanelSelected(int selectedIndex){
        tilePanelHolder.transform.position = new Vector3(tilePanelHolder.transform.position.x, -50, tilePanelHolder.transform.position.z);
        GameObject tilePanelToStart = tilePanelHolder.transform.GetChild(0).gameObject;
        if(defaultColor == Color.black){
            defaultColor = tilePanelToStart.GetComponent<RulePanelTileUI>().GetColor();
        }
        SetTilePanelSelected(tilePanelToStart);
    }

    public void SetWeatherPanelSelected(GameObject selected){
        ClearWeatherColors();
        selected.GetComponent<RulePanelWeatherUI>().ChangeColor(selectedColor);
    }

    public void SetWeatherPanelSelected(int selectedIndex){
        SetWeatherPanelSelected(weatherPanelHolder.transform.GetChild(0).gameObject);
    }
}
