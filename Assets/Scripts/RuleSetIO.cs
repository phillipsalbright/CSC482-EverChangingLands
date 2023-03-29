using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

    /* use this for json stuff: https://docs.unity3d.com/ScriptReference/JsonUtility.html, 
     * file directory: https://docs.unity3d.com/ScriptReference/Windows.Directory.html
     * for persistent data: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html
     * reading/writing to file: https://learn.microsoft.com/en-us/dotnet/api/system.io.streamwriter?view=net-7.0 
     */
public class RuleSetIO : MonoBehaviour
{
    public bool WriteToFile(TileRuleSet trs, string filename = null, bool overwrite = true){
        RuleSetSave rss = new RuleSetSave(trs);
        if(trs == null){
            Debug.LogWarning("could not write to file. ruleset was null");
            return false;
        }
        if(filename == null){
            filename = trs.getRSName();
        }
        string filepath = Application.persistentDataPath + "/" + filename;
        Debug.Log("trying to write to filepath: " + filepath);
        if(!overwrite && System.IO.File.Exists(filepath) ){
            Debug.LogWarning("tried to overwrite file without overwrite set to false");
            return false;
        }
        string js = JsonUtility.ToJson(rss);
        try{
            StreamWriter sw = new StreamWriter(filepath, false);
            sw.Write(js);
            sw.Close();
        }
        catch(Exception e){
            Debug.Log("Failure writing to file: " + e.Message);
            return false;
        }
        
        return true;
    }

    public bool ReadFromFile(string filename, out RuleSetSave trs){
        trs = null;
        string filepath = Application.persistentDataPath + "/" + filename;
        Debug.Log("trying to read from filepath: " + filepath);
        if(!System.IO.File.Exists(filepath)){
            Debug.LogWarning("file does not exist at desired filepath");
            return false;
        }
        string js;
        try{
            StreamReader sr = new StreamReader(filepath);
            js = sr.ReadToEnd();
            sr.Close();
        }
        catch(Exception e){
            Debug.Log("Failure reading from file: " + e.Message);
            return false;
        }
        try{
            trs = JsonUtility.FromJson<RuleSetSave>(js);
        }
        catch(Exception e){
            Debug.Log("Failure converting file from json: " + e.Message);
        }

        return true;
    }
}
