using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> settlers;
    [SerializeField] private int initialNumberOfSettlers = 3;
    [SerializeField] private GameObject settlerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        settlers = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentNumberOfSettlers()
    {
        return settlers.Count;
    }

    public int GetInitialNumberOfSettlers()
    {
        return initialNumberOfSettlers;
    }

    public List<GameObject> GetSettlers()
    {
        return settlers;
    }

    public void AddSettlerAtTile(Tile tile)
    {
        bool compatableTile = tile.GetCurrentTileType() != Tile.TileTypes.Water && tile.GetCurrentTileType() != Tile.TileTypes.DeepWater;
        bool tileHasSettler = false;

        foreach(GameObject s in settlers)
        {
            if(s.GetComponent<Settler>().GetCurrentTile() == tile)
            {
                tileHasSettler = true;
                break;
            }
        }

        if (compatableTile && !tileHasSettler)
        {
            GameObject settler = GameObject.Instantiate(settlerPrefab, Vector3.zero, Quaternion.identity);
            settler.GetComponent<Settler>().SetCurrentTileAndPosition(tile);
            settlers.Add(settler);
        }
    }
}
