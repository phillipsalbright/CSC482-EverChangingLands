using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelWeatherUI : MonoBehaviour
{
    [SerializeField, Tooltip("whether this is an allweather condition panel or not")]
    private bool allWeather = true;
    [SerializeField, Tooltip("the weather type this panel is for")]
    private WeatherManager.WeatherTypes weatherType = WeatherManager.WeatherTypes.Sunny;
    
    public void ChangeWeatherType(){
        RuleCreationUIManager.Instance.SetNewWeather(gameObject, allWeather, weatherType);
    }

    public void ChangeColor(Color newColor){
        gameObject.GetComponent<Image>().color = newColor;
    }

    public Color GetColor(){
        return gameObject.GetComponent<Image>().color;
    }
}
