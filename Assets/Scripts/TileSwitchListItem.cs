using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileSwitchListItem : MonoBehaviour
{
    [SerializeField]
    private Image tileImage;
    [SerializeField]
    private TMP_Text tileNameText;
    [SerializeField]
    private List<Image> costImages;
    [SerializeField]
    private List<TMP_Text> costs;
    [SerializeField]
    private Image productionImage;
    [SerializeField]
    private TMP_Text production;
    [SerializeField]
    private Image panel;
    [SerializeField]
    private Button switchButton;
    private Tile.TileTypes tileType;
    private TileInfo.TileSwitch tileSwitchInfo;
    private Vector2Int tilePos;

    public void SetTileSwitch(TileInfo.TileSwitch tileSwitch, Vector2Int tilePos)
    {
        this.tilePos = tilePos;
        tileSwitchInfo = tileSwitch;
        tileType = tileSwitch.switchTile;
        tileNameText.text = tileType.ToString();
        tileImage.sprite = TileInfo.Instance.GetTileSprite(tileType);
        bool switchable = true;
        for (int i = 0; i < costImages.Count && i < costs.Count; i++)
        {
            bool filled = false;
            if (i < tileSwitch.requiredResources.Count)
            {
                ResourceManager.ResourceTypes resourceType = tileSwitch.requiredResources[i];
                int resourceCost = tileSwitch.requiredResourcesCount[i];
                costImages[i].sprite = ResourceManager.Instance.GetResourceSprite(resourceType);
                costs[i].text = resourceCost + "";
                if (ResourceManager.Instance.getResourceCount(resourceType) < resourceCost)
                {
                    switchable = false;
                }
                filled = true;
            }
            costImages[i].gameObject.SetActive(filled);
            costs[i].gameObject.SetActive(filled);
        }
        ResourceManager.ResourceTypes resourceProduced = TileInfo.Instance.GetResourceProduced(tileType);
        int resourceAmountProduced = TileInfo.Instance.GetResourceAmountProduced(tileType);
        if (resourceProduced != ResourceManager.ResourceTypes.None)
        {
            productionImage.sprite = ResourceManager.Instance.GetResourceSprite(resourceProduced);
            production.text = resourceAmountProduced + "";
            productionImage.enabled = true;
            production.enabled = true;
        }
        else
        {
            productionImage.enabled = false;
            production.enabled = false;
        }
        //bool switchable = ResourceManager.Instance.getResourceCount(resourceProduced) >= resourceAmountProduced;
        switchButton.interactable = switchable;
    }

    public void SwitchTile()
    {
        PlayerUI.Instance.SwapTile(tileSwitchInfo);
    }
}
