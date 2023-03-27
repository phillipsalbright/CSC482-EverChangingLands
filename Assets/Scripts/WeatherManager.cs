using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

//  TO ADD A NEW WEATHER TYPE:
//  FIRST ADD IT TO THE ENUM AND THE RNGVALS STRUCT DEFINITION
//  THEN CREATE AN RNGVALS STRUCT FOR IT
//  THEN ADD IT TO ALL EXISTING RNGVALS STRUCTS
//  THEN ADD IT TO THE RNGTABLES ON START

public class WeatherManager : Singleton<WeatherManager>
{
    
    public enum WeatherTypes{
        Sunny,
        Drought,
        Rain,
    }

    [Serializable]
    public struct WeatherNames{
        [Tooltip("tile type")]
        public WeatherTypes weatherType;
        [Tooltip("tile name")]
        public string weatherName;
    }
    [Tooltip("tile name mapping"), SerializeField]
    private List<WeatherNames> weatherNameList = new List<WeatherNames>();
    //map of tile names
    private Dictionary<WeatherTypes, String> weatherNameMap;

[Serializable]
    public struct weatherRngVals{
        [Tooltip("the chance of weather changing to sun")]
        public float sunnyChance;
        [Tooltip("the chance of weather changing to drought")]
        public float droughtChance;
        [Tooltip("the chance of weather changing to rain")]
        public float rainChance;
        [Tooltip("the minimum number of turns this weather can last")]
        public int minTurns;
        [Tooltip("the percentage that the chance of repeating weather decreases every turn.")]
        public float repeatChanceChange;
    }

    [SerializeField]
    [Tooltip("The starting random values for sunny weather")]
    private weatherRngVals sunny;
    [SerializeField]
    [Tooltip("The starting random values for drought")]
    private weatherRngVals drought;
    [Tooltip("The starting random values for rain")]
    [SerializeField]
    private weatherRngVals rain;

    [SerializeField]
    [Tooltip("The current weather")]
    private WeatherTypes currentWeather;
    [SerializeField]
    [Tooltip("next turn's weather")]
    private WeatherTypes nextWeather;
    [SerializeField]
    [Tooltip("the number of turns that the current weather has existed")]
    private int numTurns = 0;

    private Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>> defaultWeatherRngTables;
    private Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>> weatherRngTables;
    private Dictionary<WeatherTypes, int> weatherTurnCounts;
    private Dictionary<WeatherTypes, float> weatherRateChanges;

    private List<WeatherTypes> allWeatherTypes;

    void Start(){

        weatherNameMap = new Dictionary<WeatherTypes, string>();
        foreach(WeatherNames wn in weatherNameList){
            weatherNameMap.Add(wn.weatherType, wn.weatherName);
        }



        allWeatherTypes = new List<WeatherTypes>(0);
        allWeatherTypes.Add(WeatherTypes.Sunny);
        allWeatherTypes.Add(WeatherTypes.Rain);
        allWeatherTypes.Add(WeatherTypes.Drought);


        //set up the rng tables with values from the inspector
        defaultWeatherRngTables = new Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>>();
        defaultWeatherRngTables.Add(WeatherTypes.Sunny, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, sunny.sunnyChance},
            {WeatherTypes.Drought, sunny.droughtChance},
            {WeatherTypes.Rain, sunny.rainChance},
        });
        defaultWeatherRngTables.Add(WeatherTypes.Drought, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, drought.sunnyChance},
            {WeatherTypes.Drought, drought.droughtChance},
            {WeatherTypes.Rain, drought.rainChance},
        });
        defaultWeatherRngTables.Add(WeatherTypes.Rain, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, rain.sunnyChance},
            {WeatherTypes.Drought, rain.droughtChance},
            {WeatherTypes.Rain, rain.rainChance},
        });

        weatherRateChanges = new Dictionary<WeatherTypes, float>();
        weatherRateChanges.Add(WeatherTypes.Sunny, sunny.repeatChanceChange);
        weatherRateChanges.Add(WeatherTypes.Drought, drought.repeatChanceChange);
        weatherRateChanges.Add(WeatherTypes.Rain, rain.repeatChanceChange);

        weatherRngTables = new Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>>();
        foreach(KeyValuePair<WeatherTypes, Dictionary<WeatherTypes,float>> weatherChances in defaultWeatherRngTables){
            weatherRngTables.Add(weatherChances.Key, new Dictionary<WeatherTypes, float>(weatherChances.Value));
        }

        weatherTurnCounts = new Dictionary<WeatherTypes, int>();
        weatherTurnCounts.Add(WeatherTypes.Sunny, sunny.minTurns);
        weatherTurnCounts.Add(WeatherTypes.Drought, drought.minTurns);
        weatherTurnCounts.Add(WeatherTypes.Rain, rain.minTurns);

        currentWeather = WeatherTypes.Sunny;
        numTurns = 1;
        GetNextWeather();
    }
    
    public WeatherTypes GetCurrentWeather(){
        return currentWeather;
    }

    public void SetNewWeather(){
        if(currentWeather == nextWeather){
            numTurns++;
        }
        else{
            numTurns = 1;
            weatherRngTables[currentWeather] = new Dictionary<WeatherTypes, float>(defaultWeatherRngTables[currentWeather]);
            //weatherRngTables = new Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>>(defaultWeatherRngTables);
            Debug.Log("Stored current weather chance: " + ToPrettyString(defaultWeatherRngTables[currentWeather]));
        }
        //Debug.Log("new current weather situation: " + string.Join(Environment.NewLine, weatherRngTables));
        currentWeather = nextWeather;
        GetNextWeather();
    }

    public void GetNextWeather(){
        bool weatherChange = false;
        if(numTurns > weatherTurnCounts[currentWeather]){
            float rng = UnityEngine.Random.Range(0f,1f);
            float currentBar = 0f;
            Debug.Log("Weather rng: " + rng);
            foreach(KeyValuePair<WeatherTypes, float> weatherChance in weatherRngTables[currentWeather]){
                currentBar += weatherChance.Value;
                if(rng < currentBar && !weatherChange){
                    nextWeather = weatherChance.Key;
                    weatherChange = true;
                    Debug.Log("Weather changed for rng of " + currentBar);
                    break;
                    
                }
                
            }
            foreach(WeatherTypes thisWeather in allWeatherTypes){
                if(thisWeather == currentWeather){
                    weatherRngTables[currentWeather][thisWeather] -= weatherRateChanges[thisWeather];
                    
                }
                else{
                    weatherRngTables[currentWeather][thisWeather] += (weatherRateChanges[thisWeather] / (weatherRngTables[currentWeather].Count - 1));
                }
                
            }
            Debug.Log("New current weather chance: " + ToPrettyString(weatherRngTables[currentWeather]));
            Debug.Log("Stored current weather chance: " + ToPrettyString(defaultWeatherRngTables[currentWeather]));
        }
        else{
            Debug.Log("Number of turns with current weather under minimum at: " + numTurns);
        }
    }

    public static string ToPrettyString<TKey, TValue>(IDictionary<TKey, TValue> dict)
{
    var str = new StringBuilder();
    str.Append("{");
    foreach (var pair in dict)
    {
        str.Append(String.Format(" {0}={1} ", pair.Key, pair.Value));
    }
    str.Append("}");
    return str.ToString();
}

public String getWeatherNameString(WeatherTypes type){
        if(!weatherNameMap.ContainsKey(type)){
            return "~WEATHER NOT SET~";
        }
        return weatherNameMap[type];
    }
}
