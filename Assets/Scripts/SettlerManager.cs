using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlerManager : Singleton<SettlerManager>
{
    [SerializeField] private List<GameObject> settlers;
    [SerializeField] private int initialNumberOfSettlers = 3;
    [SerializeField] private GameObject settlerPrefab;
    [SerializeField] private int maxSettlerMovement = 5;

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

    public int GetNumberAliveSettlers()
    {
        int alive = 0;
        for (int i = 0; i < settlers.Count; i++)
        {
            if (!settlers[i].GetComponent<Settler>().isSettlerDead())
            {
                alive++;
            }
        }

        return alive;
    }

    public int GetInitialNumberOfSettlers()
    {
        return initialNumberOfSettlers;
    }

    public int GetMaxSettlerMovement()
    {
        return maxSettlerMovement;
    }

    public List<GameObject> GetSettlers()
    {
        return settlers;
    }

    public bool AddSettlerAtTile(Tile tile)
    {
        bool compatableTile = tile.GetIsWalkable() && tile.GetCurrentTileType() != Tile.TileTypes.Desert && tile.GetCurrentTileType() != Tile.TileTypes.Forest;
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
            settler.GetComponent<Settler>().SetInitialTileAndPosition(tile, true);
            settlers.Add(settler);
            return true;
        }
        return false;
    }

    public Settler GetSettlerAtPos(Vector2Int pos)
    {
        foreach(GameObject go in settlers)
        {
            if(go.GetComponent<Settler>().GetCurrentTile().GetTilePos2() == pos)
            {
                return go.GetComponent<Settler>();
            }
        }
        return null;
    }
}
