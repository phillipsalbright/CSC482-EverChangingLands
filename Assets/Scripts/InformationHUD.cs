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
    [SerializeField] private GameObject tileInfo;

    public void SetInformation(Tile.TileTypes type, WeatherManager.WeatherTypes weatherType)
    {
        foreach(Transform child in contentHolder.transform)
        {
            Destroy(child.gameObject);
        }
        IsometricRuleTile t = TileInfo.Instance.GetTile(type);
        tileImage.sprite = t.m_DefaultSprite;
        tileNameText.text = t.name;
        List<string> rules = TileManager.Instance.GetTileRuleSet().toStringForTileGivenWeather(type, weatherType);
        for (int i = 0; i < rules.Count; i++)
        {
            GameObject p = Instantiate(rulePanelPrefab.gameObject, contentHolder.transform);
            p.GetComponent<RulePanel>().SetRuleNumText(i + 1);
            p.GetComponent<RulePanel>().SetRuleDescText(rules[i]);
        }
        tileInfo.SetActive(true);
        
    }

    public void HideInfoDisplay()
    {
        tileInfo.SetActive(false);
    }
}
