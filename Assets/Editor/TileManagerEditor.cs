using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(TileManager))]
public class TileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        TileManager manager = (TileManager)target;
        if (GUILayout.Button("Regenerate"))
        {
            if (manager.IsSceneBound())
            {
                manager.GenerateMap();
            }
        }
    }
}
