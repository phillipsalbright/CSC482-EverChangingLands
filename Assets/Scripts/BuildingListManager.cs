using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingListManager : Singleton<BuildingListManager>
{
    [SerializeField]
    private GameObject buildingListObject;
    private List<BuildingListUI> buildingLists = new List<BuildingListUI>();
    private List<GameObject> buildingObjects = new List<GameObject>();
    [SerializeField]
    private GameObject contentObject;

    public void OpenBuildingList(Tile.TileTypes tileType, Vector2Int tilePos)
    {

        List<BuildingManager.BuildingName> availableBuildings = BuildingManager.Instance.getAvailableBuildings(tileType);
        int i = 0;
        foreach (BuildingManager.BuildingName buildingName in availableBuildings)
        {
            if (i < buildingObjects.Count)
            {
                buildingLists[i].SetBuilding(BuildingManager.Instance.GetBuilding(buildingName), tilePos);
                buildingObjects[i].SetActive(true);
            }
            else
            {
                GameObject newBuildingObject = Instantiate(buildingListObject, contentObject.transform);
                buildingLists.Add(newBuildingObject.GetComponent<BuildingListUI>());
                newBuildingObject.GetComponent<BuildingListUI>().SetBuilding(BuildingManager.Instance.GetBuilding(buildingName), tilePos);
                buildingObjects.Add(newBuildingObject);
            }
            i++;
        }
        for (int j = i; j < buildingObjects.Count; j++)
        {
            buildingObjects[j].SetActive(false);
        }
    }
}
