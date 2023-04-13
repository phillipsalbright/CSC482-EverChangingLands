using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleMainMenuCanvas : MonoBehaviour
{
    [SerializeField, Tooltip("the dropdown")]
    private TMPro.TMP_Dropdown dropdown;
    [SerializeField, Tooltip("the default name for creating a new ruleset")]
    private string newRSString = "Start from scratch";
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public string getNewRSName(){
        return newRSString;
    }

    public void Setup(){
        SetupDropDown(RuleSetManager.Instance.GetAllRuleSets());
    }

    // Update is called once per frame
    public void SetupDropDown(List<TileRuleSet> ruleSets)
    {
        dropdown.ClearOptions();
        List<string> options = new List<string>();
        options.Add(newRSString);
        foreach(TileRuleSet trs in ruleSets){
            options.Add(trs.getRSName());
        }
        dropdown.AddOptions(options);
    }

    public void ClickedStart(){
        StartCreating(dropdown.captionText.text);
    }

    public void StartCreating(string trsName){
        RuleCreationUIManager.Instance.StartRSCreator(trsName, newRSString);
    }

    public void ClickedBack(){
        RuleCreationUIManager.Instance.BackToMain();
    }
}
