using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomMap : MonoBehaviour
{
    [Header("Default Values")]
    [SerializeField, Range(0,1)]
    private float biomeScale;
    [SerializeField, Range(0, 1)]
    private float tileScale;
    private int seed;
    [SerializeField]
    private Vector2Int mapSize;
    [Header("Input Fields")]
    [SerializeField]
    private Slider biomeField;
    [SerializeField]
    private Slider tileField;
    [SerializeField]
    private TMP_InputField seedField;
    [SerializeField]
    private TMP_InputField mapX;
    [SerializeField]
    private TMP_InputField mapY;
    bool firstAwake = true;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
        seed = DateTime.UtcNow.ToString().GetHashCode();
        seedField.text = seed.ToString();
        mapX.text = mapSize.x.ToString();
        mapY.text = mapSize.y.ToString();
        biomeField.value = biomeScale;
        tileField.value = tileScale;
    }

    public void LoadCustom()
    {
        if (!int.TryParse(seedField.text, out seed))
        {
            return;
        }
        biomeScale = biomeField.value;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0 && !firstAwake)
        {
            Destroy(this.gameObject);
        }
        firstAwake = false;
    }

    public int GetSeed()
    {
        return seed;
    }

    public Vector2Int GetMapSize()
    {
        return mapSize;
    }

    public float GetBiomeScale()
    {
        return biomeScale;
    }

    public float GetTileScale()
    {
        return tileScale;
    }
}
