using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    private TileManager tileManager;
    private UnityEvent<int> _onTurnChanged = new();
    private int _turn;
    private Tile selectedTile;
    [SerializeField] private Tilemap selectionMap;
    [SerializeField] private TileBase reticleTile;

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
    }

    public void SelectTile(Tile tile)
    {
        selectionMap.ClearAllTiles();
        selectionMap.SetTile(new Vector3Int(tile.GetTilePosition().x, tile.GetTilePosition().y, 0), reticleTile);
    }

    public void DeleteSelection()
    {
        selectionMap.ClearAllTiles();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
