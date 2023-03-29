using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;


public class RuleSetManager : Singleton<RuleSetManager>
{
    
    [SerializeField]
    private TileRuleSet demo;
    [SerializeField, Tooltip("the rulesets that should come packaged with the game")]
    private RuleSetHolder ruleSets;
    [SerializeField, Tooltip("the rulesets that should come packaged with the game")]
    private List<TileRuleSet> starterRuleSets;
    [SerializeField]
    private RuleSetIO rsIO;

    // Start is called before the first frame update
    void Start()
    {
        
        if(ruleSets == null){
            ruleSets = gameObject.AddComponent<RuleSetHolder>();

        }
        if(rsIO == null){
            rsIO = gameObject.AddComponent<RuleSetIO>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDemo(){
        
    }

    public void WriteDemoToFile(){
        rsIO.WriteToFile(demo, "demo");
    }
    public void ReadDemoFromFile(){
        RuleSetSave newRss;
        bool success = rsIO.ReadFromFile("demo", out newRss);

        if(success){    
            ruleSets.AddNewRuleSetToList(newRss);
            DisplayListLength();
        }
    }
    

    private bool AddNewRuleSetToList(TileRuleSet trs){
        return false;
    }

    public void DisplayListLength(){
        Debug.Log("List length: " + ruleSets.GetCount());
    }

    // private void ApplyChanges()
}
