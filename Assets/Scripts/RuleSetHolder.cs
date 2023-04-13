using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSetHolder : MonoBehaviour
{
    [SerializeField]
    public List<TileRuleSet> ruleSets;
    private Dictionary<string, TileRuleSet> ruleSetMap;
    [SerializeField]
    public int check = 2;

    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }

    void Setup(){
        if(ruleSets == null){
            ruleSets = new List<TileRuleSet>();
        }
        if(ruleSetMap == null){
            ruleSetMap = new Dictionary<string, TileRuleSet>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCount(){
        return ruleSets.Count;
    }

    public void AddNewRuleSetToList(TileRuleSet newTrs){
        ruleSets.Add(newTrs);
        ruleSetMap.Add(newTrs.getRSName(),newTrs);
    }
    public void AddNewRuleSetToList(RuleSetSave newRss){
        TileRuleSet newTrs = gameObject.AddComponent<TileRuleSet>();
        newTrs.SetupFromRSS(newRss);
        ruleSets.Add(newTrs);
        ruleSetMap.Add(newTrs.getRSName(),newTrs);
    }

    public void DeleteRuleSet(TileRuleSet trs){
        ruleSets.Remove(trs);
        ruleSetMap.Remove(trs.getRSName());
    }

    public void ClearRuleSets(){
        ruleSets = new List<TileRuleSet>();
    }

    public TileRuleSet GetRuleSetByName(string trsName){
        if(!ruleSetMap.ContainsKey(trsName)){
            Debug.LogError("COULD NOT GET RULESET OF NAME " + trsName);
            return null;
        }
        return ruleSetMap[trsName];
    }

    public List<TileRuleSet> GetAllRuleSets(){
        return ruleSets;
    }

    
}
