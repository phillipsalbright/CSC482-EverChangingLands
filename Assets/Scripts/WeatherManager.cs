using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TO ADD A NEW WEATHER TYPE:
//FIRST ADD IT TO THE ENUM AND THE RNGVALS STRUCT DEFINITION
//THEN CREATE AN RNGVALS STRUCT FOR IT
//THEN ADD IT TO ALL EXISTING RNGVALS STRUCTS
//THEN ADD IT TO THE RNGTABLES ON START

public class WeatherManager : Singleton<WeatherManager>
{
    public enum WeatherTypes{
        Sunny,
        Drought,
        Rain,
    }

[Serializable]
    public struct weatherRngVals{
        public float sunnyChance;
        public float droughtChance;
        public float rainChance;
    }

    [SerializeField]
    private weatherRngVals sunny;
    [SerializeField]
    private weatherRngVals drought;
    [SerializeField]
    private weatherRngVals rain;

    [SerializeField]
    private WeatherTypes currentWeather;
    [SerializeField]
    private WeatherTypes nextWeather;

    private Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>> weatherRngTables;

    void Start(){
        //set up the rng tables with values from the inspector
        weatherRngTables = new Dictionary<WeatherTypes, Dictionary<WeatherTypes, float>>();
        weatherRngTables.Add(WeatherTypes.Sunny, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, sunny.sunnyChance},
            {WeatherTypes.Drought, sunny.droughtChance},
            {WeatherTypes.Rain, sunny.rainChance},
        });
        weatherRngTables.Add(WeatherTypes.Drought, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, drought.sunnyChance},
            {WeatherTypes.Drought, drought.droughtChance},
            {WeatherTypes.Rain, drought.rainChance},
        });
        weatherRngTables.Add(WeatherTypes.Rain, new Dictionary<WeatherTypes, float>() {
            {WeatherTypes.Sunny, rain.sunnyChance},
            {WeatherTypes.Drought, rain.droughtChance},
            {WeatherTypes.Rain, rain.rainChance},
        });
        currentWeather = WeatherTypes.Sunny;
        GetNextWeather();
    }
    
    public WeatherTypes GetCurrentWeather(){
        return currentWeather;
    }

    public void SetNewWeather(){
        currentWeather = nextWeather;
        GetNextWeather();
    }

    public void GetNextWeather(){
        float rng = UnityEngine.Random.Range(0f,1f);
        float currentBar = 0f;
        Debug.Log("Weather rng: " + rng);
        foreach(KeyValuePair<WeatherTypes, float> weatherChance in weatherRngTables[currentWeather]){
            currentBar += weatherChance.Value;
            if(rng < currentBar){
                nextWeather = weatherChance.Key;
                return;
            }

        }
        nextWeather = currentWeather;
    }
}
