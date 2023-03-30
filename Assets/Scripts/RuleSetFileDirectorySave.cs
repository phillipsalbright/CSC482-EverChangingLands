using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleSetFileDirectorySave
{
    public List<string> filepaths;
    public string thisFilepath;

    public RuleSetFileDirectorySave(string filepath){
        thisFilepath = filepath;
        filepaths = new List<string>();
    }

    public RuleSetFileDirectorySave(string filepath, List<string> rsFilepaths){
        thisFilepath = filepath;
        filepaths = rsFilepaths;
    }

    public List<string> getFilepaths(){
        return filepaths;
    }

    public string getFilepath(){
        return thisFilepath;
    }

    public bool ContainsRuleSetName(string rsName){
        return filepaths.Contains(rsName);
    }

    public bool AddNewRuleSetToList(TileRuleSet trs, bool overwrite = false){
        if(!overwrite && ContainsRuleSetName(trs.getRSName())){
            return false;
        }
        filepaths.Add(trs.getRSName());
        return true;
    }

    public bool AddNewRuleSetToList(RuleSetSave rss, bool overwrite = false){
        if(!overwrite && ContainsRuleSetName(rss.getRSName())){
            return false;
        }
        filepaths.Add(rss.getRSName());
        return true;
    }

    public bool AddNewRuleSetToList(string rsFilepath, bool overwrite = false){
        if(!overwrite && ContainsRuleSetName(rsFilepath)){
            return false;
        }
        filepaths.Add(rsFilepath);
        return true;
    }

    public bool DeleteRuleSet(TileRuleSet trs){
        if(!filepaths.Contains(trs.getRSName())){
            return false;
        }
        filepaths.Remove(trs.getRSName());
        return true;
    }

    public bool DeleteRuleSet(RuleSetSave rss){
        if(!filepaths.Contains(rss.getRSName())){
            return false;
        }
        filepaths.Remove(rss.getRSName());
        return true;
    }

    public bool DeleteRuleSet(string rsFilepath){
        if(!filepaths.Contains(rsFilepath)){
            return false;
        }
        filepaths.Remove(rsFilepath);
        return true;
    }

    public void ClearRuleSets(){
        filepaths = new List<string>();
    }
}
