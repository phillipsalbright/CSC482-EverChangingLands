using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;


public class RuleSetManager : Singleton<RuleSetManager>
{
    
    [SerializeField]
    private TileRuleSet demo;
    [SerializeField, Tooltip("the ruleset holder")]
    private RuleSetHolder ruleSets;
    [SerializeField, Tooltip("the rulesets that should come packaged with the game")]
    private List<TileRuleSet> starterRuleSets;
    [SerializeField]
    private RuleSetIO rsIO;
    private RuleSetFileDirectorySave fileDirectory;
    [SerializeField]
    private string directoryFilepath = "tileRuleSet_directory_save_vtecl";

    // Start is called before the first frame update
    void Start()
    {
        
        if(ruleSets == null){
            ruleSets = gameObject.AddComponent<RuleSetHolder>();

        }
        if(rsIO == null){
            rsIO = gameObject.AddComponent<RuleSetIO>();
        }
        if(rsIO.DoesFileExist(directoryFilepath)){
            bool success = rsIO.ReadDirectory(directoryFilepath, out fileDirectory);
            if(!success){
                Debug.LogWarning("Failed to load existing directory file. creating new");
                fileDirectory = new RuleSetFileDirectorySave(directoryFilepath);
                WriteStarterRuleSets();
                ReadRulesFromDirectory(fileDirectory);
            }
            else{
                WriteStarterRuleSets();
                ReadRulesFromDirectory(fileDirectory);
            }
        }
        else{
            Debug.Log("No directory file found. creating new");
            fileDirectory = new RuleSetFileDirectorySave(directoryFilepath);
            WriteStarterRuleSets();
            ReadRulesFromDirectory(fileDirectory);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddDemo(){
        
    }

    public void WriteStarterRuleSets(){
        foreach(TileRuleSet trs in starterRuleSets){
            if(trs == null){
                Debug.LogError("starter ruleset doesn't exist");
                continue;
            }
            rsIO.WriteToFile(trs, trs.getRSName());
            if(ruleSets == null){
                Debug.LogError("rulesetHolder nonexistent");
            }
            ruleSets.AddNewRuleSetToList(trs);
            fileDirectory.AddNewRuleSetToList(trs, true);
        }
        rsIO.WriteDirectory(directoryFilepath, fileDirectory);
    }

    public void WriteDemoToFile(){
        CreateRuleSet(demo);
    }
    public void ReadDemoFromFile(){
        RuleSetSave newRss;
        bool success = rsIO.ReadFromFile("demo", out newRss);

        if(success){    
            ruleSets.AddNewRuleSetToList(newRss);
            DisplayListLength();
        }
    }

    private void ReadRulesFromDirectory(RuleSetFileDirectorySave rsfd){
        List<string> filepaths = rsfd.getFilepaths();
        foreach(string s in filepaths){
            ReadRuleFromFile(s);
        }
    }

    private bool ReadRuleFromFile(string ruleFilepath){
        RuleSetSave newRss;
        bool success = rsIO.ReadFromFile(ruleFilepath, out newRss);
        if(success){    
            ruleSets.AddNewRuleSetToList(newRss);
            DisplayListLength();
        }
        return success;
    }
    

    private bool AddNewRuleSetToList(TileRuleSet trs){
        ruleSets.AddNewRuleSetToList(trs);
        DisplayListLength();
        return true;
    }

    private bool AddNewRuleSetToList(RuleSetSave rss){
        ruleSets.AddNewRuleSetToList(rss);
        DisplayListLength();
        return true;
    }

    public bool CreateRuleSet(TileRuleSet trs){
        if(fileDirectory.ContainsRuleSetName(trs.getRSName())){
            Debug.LogWarning("tried to save over ruleset with existing name: " + trs.getRSName());
            return false;
        }
        bool s1 = rsIO.WriteToFile(trs, trs.getRSName());
        ruleSets.AddNewRuleSetToList(trs);
        fileDirectory.AddNewRuleSetToList(trs, true);
        bool s2 = rsIO.WriteDirectory(directoryFilepath, fileDirectory);

        return s1 && s2;
    }
    public bool CreateRuleSet(TileRuleSet.AllRules ar, string rulesName){
        TileRuleSet thisTRS = ruleSets.gameObject.AddComponent<TileRuleSet>();
        thisTRS.setRSName(rulesName);
        thisTRS.SetRuleSet(ar);
        return CreateRuleSet(thisTRS);
    }

    private bool OverwriteRuleSet(TileRuleSet trs){
        return rsIO.WriteToFile(trs, trs.getRSName());
    }

    public void DisplayListLength(){
        Debug.Log("List length: " + ruleSets.GetCount());
    }

    //WARNING. WILL LEAVE GAME WITHOUT RULESETS TO USE. ONLY FOR TESTING PURPOSES. DO NOT USE IN GAME
    public void PurgeRuleSets(){
        ClearRuleSets();
        rsIO.DeleteFile(directoryFilepath);
        fileDirectory = null;
        Debug.LogError("FILE DIRECTORY DELETED. PLEASE EXIT GAME NOW OR NAVIGATE TO NEW MENU. RULESETS CANNOT BE SAVED UNTIL REGENERATED");
    }

    public void ClearRuleSets(){
        List<string> ls = fileDirectory.getFilepaths();
        foreach(string s in ls){
            rsIO.DeleteFile(s);
        }
        fileDirectory.ClearRuleSets();
        ruleSets.ClearRuleSets();
    }

    public void DeleteRuleSet(TileRuleSet trs){
        ruleSets.DeleteRuleSet(trs);
        fileDirectory.DeleteRuleSet(trs);
        rsIO.WriteDirectory(directoryFilepath, fileDirectory);
        rsIO.DeleteFile(trs.getRSName());
    }

    public TileRuleSet GetRuleSetByName(string trsName){
        return ruleSets.GetRuleSetByName(trsName);
    }

    public List<TileRuleSet> GetAllRuleSets(){
        return ruleSets.GetAllRuleSets();
    }

    // private void ApplyChanges()
}
