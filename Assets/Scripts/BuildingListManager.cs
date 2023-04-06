using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingListManager : Singleton<BuildingListManager>
{
    [SerializeField]
    private GameObject buildingListObject;
    private List<BuildingListUI> buildingLists = new List<BuildingListUI>();
    [SerializeField]
    private GameObject contentObject;

    public void OpenBuildingList(Tile.TileTypes tileType, Vector2Int tilePos)
    {
        List<BuildingManager.BuildingName> availableBuildings = BuildingManager.Instance.getAvailableBuildings(tileType);
        foreach (BuildingManager.BuildingName buildingName in availableBuildings)
        {
            GameObject newBuildingObject = Instantiate(buildingListObject, contentObject.transform);
            buildingLists.Add(newBuildingObject.GetComponent<BuildingListUI>());
            newBuildingObject.GetComponent<BuildingListUI>().SetBuilding(BuildingManager.Instance.GetBuilding(buildingName), tilePos);
        }
    }
}
