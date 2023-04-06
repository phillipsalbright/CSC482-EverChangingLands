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
    private TMP_Text buildingName;
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
    
    public void SetBuilding(BuildingManager.BuildingName buildingName)
    {
        this.buildingName.text = buildingName.ToString();
        
    }

    public void BuildBuilding()
    {

    }
}
