using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class WeatherRNGHolder : MonoBehaviour
{
    [Serializable]
    public struct WeatherSet{
        [Tooltip("this weather set's name")]
        public string weatherSetName;
        [Tooltip("the list of this weather set's rng values")]
        public List<weatherRngVals> weatherSetVals;
    }
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
    [Tooltip("The starting weather")]
    private WeatherManager.WeatherTypes startingWeather;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
