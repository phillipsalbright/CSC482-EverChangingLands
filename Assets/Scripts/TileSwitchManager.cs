using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSwitchManager : Singleton<TileSwitchManager>
{
    [SerializeField]
    private GameObject tileListObject;
    private List<TileSwitchListItem> tileLists = new List<TileSwitchListItem>();
    private List<GameObject> tileObjects = new List<GameObject>();
    [SerializeField]
    private GameObject contentObject;
    

    public void OpenTileManager(Tile.TileTypes tileType, Vector2Int tilePos)
    {
        List<TileInfo.TileSwitch> tileSwitches = TileInfo.Instance.GetTileSwitches(tileType);
        int i = 0;
        foreach (TileInfo.TileSwitch tileSwitch in tileSwitches)
        {
            if (i < tileObjects.Count)
            {
                tileLists[i].SetTileSwitch(tileSwitch, tilePos);
                tileObjects[i].SetActive(true);
            }
            else
            {
                GameObject newTileSwitchObject = Instantiate(tileListObject, contentObject.transform);
                tileLists.Add(newTileSwitchObject.GetComponent<TileSwitchListItem>());
                newTileSwitchObject.GetComponent<TileSwitchListItem>().SetTileSwitch(tileSwitch, tilePos);
                tileObjects.Add(newTileSwitchObject);
            }
            i++;
        }
        gameObject.SetActive(true);
    }
}
