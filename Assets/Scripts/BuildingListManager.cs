using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingListManager : MonoBehaviour
{
    [SerializeField]
    private GameObject buildingListObject;
    private List<BuildingListUI> buildingLists;
    [SerializeField]
    private GameObject contentObject;

    public void OpenBuildingList(Tile.TileTypes tileType)
    {
        List<BuildingManager.BuildingName> availableBuildings = BuildingManager.Instance.getAvailableBuildings(tileType);
        foreach (BuildingManager.BuildingName buildingName in availableBuildings)
        {
            GameObject newBuildingObject = Instantiate(buildingListObject, contentObject.transform);
            buildingLists.Add(newBuildingObject.GetComponent<BuildingListUI>());
        }
    }
}
