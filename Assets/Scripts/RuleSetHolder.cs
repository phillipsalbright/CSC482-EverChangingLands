using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSetHolder : MonoBehaviour
{
    [SerializeField]
    public List<TileRuleSet> ruleSets;
    [SerializeField]
    public int check = 2;
    // Start is called before the first frame update
    void Start()
    {
        Setup();
    }

    void Setup(){
        if(ruleSets == null){
            ruleSets = new List<TileRuleSet>();
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
    }
    public void AddNewRuleSetToList(RuleSetSave newRss){
        TileRuleSet newTrs = gameObject.AddComponent<TileRuleSet>();
        newTrs.SetupFromRSS(newRss);
        ruleSets.Add(newTrs);
    }
}
