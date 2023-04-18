    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject customPanel;
    [SerializeField]
    private GameObject startPanel;
    [SerializeField]
    private CustomMap customMap;
    [SerializeField]
    private TMPro.TMP_Dropdown rsDropdown;

    [SerializeField]
    private RuleCreationUIManager rcuiMan;

    [SerializeField, Tooltip("Whether to display the ruleset creation stuff or not")]
    private bool useRSCreator = true;
    [SerializeField, Tooltip("all of the objects associated with the custom ruleset")]
    private GameObject rsCreatorObjects;


    public void StartGame()
    {
        
        Destroy(customMap.gameObject);
        SceneManager.LoadScene(1);
    }

    public void SetRCUIMan(RuleCreationUIManager rcui){
        Debug.Log("setting rcui");
        rcuiMan = rcui;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGameCustom()
    {
        
        TileRuleSet ruleSetToUse = RuleSetManager.Instance.GetRuleSetByName(rsDropdown.captionText.text);
        if(ruleSetToUse == null){
                Debug.LogError("could not get ruleset with name: " + rsDropdown.captionText.text);
        }
        else{
            customMap.setRS(ruleSetToUse);
        }
        customMap.LoadCustom();
        SceneManager.LoadScene(1);
    }

    public void GoToCreationCanvas(){
        customPanel.SetActive(false);
        RuleCreationUIManager.Instance.SetupMainMenu();
    }
    public void ReturnFromCreationCanvas(){
        customPanel.SetActive(true);
        SetupRuleSetDropDown();
    }

    public void SetupRuleSetDropDown(){
        Debug.Log("setting up the ruleset dropdown");
        if(rsDropdown != null){
            rsDropdown.ClearOptions();
            List<string> options = new List<string>();
            List<TileRuleSet> ruleSets = RuleSetManager.Instance.GetAllRuleSets();
            foreach(TileRuleSet trs in ruleSets){
                options.Add(trs.getRSName());
            }
            rsDropdown.AddOptions(options);
        }
    }

    public TileRuleSet GetRuleSetForGame(){
        return RuleSetManager.Instance.GetRuleSetByName(rsDropdown.captionText.text);
    }

    void Start(){
        if(!useRSCreator && rsCreatorObjects != null){
            rsCreatorObjects.SetActive(false);

        }
    }
}
