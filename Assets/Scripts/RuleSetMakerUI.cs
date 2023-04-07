using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSetMakerUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddRuleSetToManager(){
        RuleSetManager.Instance.WriteDemoToFile();
    }

    public void LoadRuleSetToManager(){
        RuleSetManager.Instance.ReadDemoFromFile();
    }

    public void ClearFiles(){
        RuleSetManager.Instance.ClearRuleSets();
    }

    public void PurgeFiles(){
        RuleSetManager.Instance.PurgeRuleSets();
    }
}
