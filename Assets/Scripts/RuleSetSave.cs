using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RuleSetSave
{
    [SerializeField]
    private string name;
    [SerializeField]
    private TileRuleSet.AllRules fullRuleSet;

    public RuleSetSave(TileRuleSet.AllRules ar, string newName){
        fullRuleSet = ar;
        name = newName;
    }
    public RuleSetSave(TileRuleSet trs){
        fullRuleSet = trs.getRuleset();
        name = trs.getRSName();
    }

    public string getRSName(){
        return name;
    }
    public TileRuleSet.AllRules getRuleset(){
        return fullRuleSet;
    }
}
