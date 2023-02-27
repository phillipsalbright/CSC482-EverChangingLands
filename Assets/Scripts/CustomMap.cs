using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomMap : MonoBehaviour
{
    private float biomeScale;
    private float tileScale;
    private int seed;
    private Vector2Int mapSize;
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

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public void LoadCustom()
    {
        if (!int.TryParse(seedField.text, out seed))
        {
            return;
        }
        biomeScale = biomeField.value;
    }
}
