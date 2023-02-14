using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(WeatherManager))]
public class WeatherManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        WeatherManager manager = (WeatherManager)target;
        if (GUILayout.Button("Next Weather"))
        {
            if (manager.IsSceneBound())
            {
                manager.SetNewWeather();
            }
        }
    }
}
