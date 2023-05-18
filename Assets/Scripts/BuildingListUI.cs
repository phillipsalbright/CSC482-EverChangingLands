using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingListUI : MonoBehaviour
{
    [SerializeField]
    private Image buildingImage;
    [SerializeField]
    private TMP_Text buildingNameText;
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
    private Button buildButton;
    private BuildingManager.BuildingName buildingName;
    private Vector2Int tilePos;
    
    public void SetBuilding(BuildingManager.Building building, Vector2Int tilePos)
    {
        this.tilePos = tilePos;
        buildingName = building.buildingType;
        buildingNameText.text = building.buildingType.ToString();
        buildingImage.sprite = building.isometricTile.m_DefaultSprite;
        for (int i = 0; i < costImages.Count && i < costs.Count; i++)
        {
            bool filled = false;
            if (i < building.resourceCostList.Count)
            {
                BuildingManager.ResourceCost resourceCost = building.resourceCostList[i];
                costImages[i].sprite = ResourceManager.Instance.GetResourceSprite(resourceCost.resourceType);
                costs[i].text = resourceCost.amount + "";
                filled = true;
            }
            costImages[i].gameObject.SetActive(filled);
            costs[i].gameObject.SetActive(filled);
        }
        if (building.resourceProduced != ResourceManager.ResourceTypes.None)
        {
            productionImage.sprite = ResourceManager.Instance.GetResourceSprite(building.resourceProduced);
            production.text = building.amountProduced + "";
            productionImage.enabled = true;
            production.enabled = true;
        }
        else
        {
            productionImage.enabled = false;
            production.enabled = false;
        }
        bool buildable = BuildingManager.Instance.canAfford(building.buildingType);
        buildButton.interactable = buildable;
    }

    public void BuildBuilding()
    {
        BuildingManager.Instance.buildBuilding(buildingName, tilePos);
        PlayerUI.Instance.SetMode(PlayerController.mode.SettlerActions);
    }
}
