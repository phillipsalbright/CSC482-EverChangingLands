using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private TileManager tileManager;
    private SettlerManager settlerManager;
    private UnityEvent<int> _onTurnChanged = new();
    private int _turn = 0;
    private Tile selectedTile;
    private bool gameWon = false;
    [SerializeField] private Tilemap selectionMap;
    [SerializeField] private TileBase[] reticles;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
        tileManager = FindObjectOfType<TileManager>();
        settlerManager = FindObjectOfType<SettlerManager>();
        _turn = 1;

    }

    public event UnityAction<int> OnTurnChanged
    {
        add => _onTurnChanged.AddListener(value);
        remove => _onTurnChanged.RemoveListener(value);
    }

    public int GetTurnNum()
    {
        return _turn;
    }

    public void AdvanceTurn()
    {
        foreach (Settler s in FindObjectsOfType<Settler>())
        {
            if (s.GetCanCollect())
            {
                s.CollectResource();
            }
        }
        _turn++;
        _onTurnChanged.Invoke(_turn);
        tileManager.AdvanceTurn();
        BuildingManager.Instance.AdvanceTurn();
        foreach (Settler s in FindObjectsOfType<Settler>())
        {
            s.StartNewTurn();
        }

        if (!(hasGameBeenWon()) && settlerManager.GetNumberAliveSettlers() >= 10)
        {
            setGameWon();
        }
    }

    public void SelectTile(Tile tile, int colorindex)
    {
        //selectionMap.ClearAllTiles();
        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x, tile.GetTilePosition().y, 0), reticles[colorindex]);
    }

    public void DeleteSelection()
    {
        selectionMap.ClearAllTiles();
    }

    public void DisplayResourceTiles(Tile tile)
    {
        if(TileInfo.Instance.GetTileResourceTypes(tile.GetCurrentTileType()) != ResourceManager.ResourceTypes.None)
        {
            selectionMap.SetTile(tile.GetTilePosition(), reticles[2]);
            tile.SetIsValid(true);
        }
        else
        {
            selectionMap.SetTile(tile.GetTilePosition(), reticles[0]);
        }

        foreach(Tile t in tile.GetAdjacentTiles())
        {
            if (TileInfo.Instance.GetTileResourceTypes(t.GetCurrentTileType()) != ResourceManager.ResourceTypes.None)
            {
                selectionMap.SetTile(t.GetTilePosition(), reticles[2]);
                t.SetIsValid(true);
            }
            else
            {
                selectionMap.SetTile(t.GetTilePosition(), reticles[0]);
            }
        }
    }

    public void DisplayFlipTiles(Tile tile)
    {
        foreach (Tile t in tileManager.GetTileDictionary().Values)
        {
            t.SetIsValid(false);
        }
        selectionMap.SetTile(tile.GetTilePosition(), reticles[2]);
        tile.SetIsValid(true);
        /** same deal as below
        if (BuildingManager.Instance.hasBuilding(tile))
        {   
            selectionMap.SetTile(tile.GetTilePosition(), reticles[2]);
            tile.SetIsValid(true);
        }
        else
        {
            selectionMap.SetTile(tile.GetTilePosition(), reticles[0]);
            tile.SetIsValid(false);
        }
        */


        foreach (Tile t in tile.GetAdjacentTiles())
        {
            selectionMap.SetTile(t.GetTilePosition(), reticles[2]);
            t.SetIsValid(true);
            /** used to now allow flipping tile on building, i think its good to allow it so got rid of this
            if (BuildingManager.Instance.hasBuilding(t))
            {
                selectionMap.SetTile(t.GetTilePosition(), reticles[2]);
                t.SetIsValid(true);
            }
            else
            {
                selectionMap.SetTile(t.GetTilePosition(), reticles[0]);
                t.SetIsValid(false);
            }
            */
        }
  
    }

    public void DisplayMoveTiles(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();
        List<Tile> toVisit = new List<Tile>();
        List<Tile> visited = new List<Tile>();
        toVisit.Add(tile);

        int maxMoveDistance = 5;
        for(int i = -maxMoveDistance; i <= maxMoveDistance; i++)
        {
            for(int j = -maxMoveDistance; j <= maxMoveDistance; j++)
            {
                if((Mathf.Abs(i) + Mathf.Abs(j)) <= maxMoveDistance)
                {
                    if (tileManager.GetTileDictionary().ContainsKey(new Vector2Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j)))
                    {
                        Tile t = tileManager.GetTileDictionary()[new Vector2Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j)];

                        tiles.Add(t);
                        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j, 0), reticles[0]);

                    }
                }
            }
        }
        
        while(toVisit.Count > 0)
        {
            Tile atTile = toVisit[0];
            foreach(Tile neighbor in atTile.GetAdjacentTiles())
            {
                if (neighbor != null)
                {
                    bool compatibleTile = neighbor.GetCurrentTileType() != Tile.TileTypes.Water && neighbor.GetCurrentTileType() != Tile.TileTypes.DeepWater;
                    if (!visited.Contains(neighbor) && tiles.Contains(neighbor) && compatibleTile)
                    {
                        toVisit.Add(neighbor);
                    }

                }
            }

            bool settlerAtTile = false;
            foreach (GameObject s in settlerManager.GetSettlers())
            {
                if (s.GetComponent<Settler>().GetCurrentTile() == atTile && s.GetComponent<Settler>().GetCurrentTile() != tile)
                {
                    settlerAtTile = true;
                    break;
                }
            }

            if (atTile == tile)
            {
                selectionMap.SetTile(new Vector3Int(atTile.GetTilePos2().x, atTile.GetTilePos2().y, 0), reticles[3]);
            }
            else if (!settlerAtTile)
            {
                selectionMap.SetTile(new Vector3Int(atTile.GetTilePos2().x, atTile.GetTilePos2().y, 0), reticles[2]);
            }

            visited.Add(atTile);
            atTile.SetIsValid(true);
            toVisit.Remove(atTile);
        }

    }

    public bool hasGameBeenWon()
    {
        return gameWon;
    }

    public void setGameWon()
    {
        gameWon = true;
        Debug.LogError("GameWon");
        FindObjectOfType<PlayerUI>().SetMode(PlayerController.mode.GameWon);
    }
}
