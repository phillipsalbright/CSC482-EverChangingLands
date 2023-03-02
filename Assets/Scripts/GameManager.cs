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
    private int _turn;
    private Tile selectedTile;
    [SerializeField] private Tilemap selectionMap;
    [SerializeField] private TileBase reticleRedTile;
    [SerializeField] private TileBase reticleGreenTile;
    [SerializeField] private TileBase reticleYellowTile;
    [SerializeField] private TileBase reticleBlueTile;
    [SerializeField] private TileBase reticlePurpleTile;
    [SerializeField] private TileBase reticleOrangeTile;

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
        _turn++;
        _onTurnChanged.Invoke(_turn);
        tileManager.AdvanceTurn();
        foreach(Settler s in FindObjectsOfType<Settler>())
        {
            s.StartNewTurn();
        }
    }

    public void SelectTile(Tile tile)
    {
        selectionMap.ClearAllTiles();
        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x, tile.GetTilePosition().y, 0), reticleBlueTile);
    }

    public void DeleteSelection()
    {
        selectionMap.ClearAllTiles();
    }

    public void DisplayMoveTiles(Tile tile)
    {
        int maxMoveDistance = 5;
        for(int i = -maxMoveDistance; i <= maxMoveDistance; i++)
        {
            for(int j = -maxMoveDistance; j <= maxMoveDistance; j++)
            {
                if((Mathf.Abs(i) + Mathf.Abs(j)) <= maxMoveDistance)
                {
                    Tile t = tileManager.GetTileDictionary()[new Vector2Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j)];

                    bool settlerAtTile = false;
                    foreach(GameObject s in settlerManager.GetSettlers())
                    {
                        if(s.GetComponent<Settler>().GetCurrentTile() == t && s.GetComponent<Settler>().GetCurrentTile() != tile)
                        {
                            settlerAtTile = true;
                            break;
                        }
                    }

                    if(i == 0 && j == 0)
                    {
                        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j, 0), reticleBlueTile);
                    }
                    else if (t != null && !settlerAtTile && t.GetCurrentTileType() != Tile.TileTypes.Water && t.GetCurrentTileType() != Tile.TileTypes.DeepWater)
                    {
                        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j, 0), reticleYellowTile);
                    }
                    else
                    {
                        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x + i, tile.GetTilePosition().y + j, 0), reticleRedTile);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
