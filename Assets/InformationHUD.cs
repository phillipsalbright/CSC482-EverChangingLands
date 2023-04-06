using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InformationHUD : MonoBehaviour
{
    [SerializeField] private Image tileImage;
    [SerializeField] private TMP_Text tileNameText;
    [SerializeField] private RulePanel rulePanelPrefab;
    [SerializeField] private GameObject contentHolder;

    void Start()
    {
        SetInformation(Tile.TileTypes.Grass);
    }

    public void SetInformation(Tile.TileTypes type)
    {
        IsometricRuleTile t = TileInfo.Instance.GetTile(type);
        tileImage.sprite = t.m_DefaultSprite;
        tileNameText.text = t.name;
        for (int i = 0; i < t.m_TilingRules.Count; i++)
        {
            Debug.LogError("biscuit" + i);
            GameObject p = Instantiate(rulePanelPrefab.gameObject, contentHolder.transform);
            p.GetComponent<RulePanel>().SetRuleNumText(i + 1);
            p.GetComponent<RulePanel>().SetRuleDescText(t.m_TilingRules[i].ToString());
        }
        
    }
}
