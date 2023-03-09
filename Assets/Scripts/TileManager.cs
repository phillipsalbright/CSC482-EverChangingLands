using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private Tilemap changeMap;
    [SerializeField]
    private int seed;
    [SerializeField, Tooltip("Keeps entered seed")]
    private bool testing;
    [SerializeField, Tooltip("Works with pretty much any size (does not need to be equal)")]
    private Vector2Int mapSize;
    [SerializeField, Tooltip("How much deep water to add onto ends")]
    private Vector2Int oceanExtension;
    [SerializeField, Range(0.1f,50), Tooltip("Smaller the number, larger the groups")]
    private float tileScale;
    [SerializeField, Range(0.1f, 50), Tooltip("Smaller the number, larger the biomes")]
    private float biomeScale;
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private Vector2 tileOffset;
    private Vector2 biomeOffset;
    [SerializeField, Tooltip("Generates surrounding ocean")]
    private bool islandMode;
    private int width;
    private int height;

    [SerializeField, Tooltip("this game's tile ruleset")]
    private TileRuleSet tileRuleSet;

    private bool viewingPrediction = false;
    private int selectedBuilding = -1;


    void Start()
    {
        GenerateMap();
        CheckTiles(true);
        if(tileRuleSet == null){
            Debug.Log("Ruleset was null. checking components");
            tileRuleSet = gameObject.GetComponent<TileRuleSet>();
            
        }
        if(tileRuleSet != null){
            Debug.Log("ruleset not null. trying to set.");
            TileRules.SetRuleSet(tileRuleSet);
        }
    }

    public void AdvanceTurn()
    {
        if (viewingPrediction)
        {
            this.ViewChangeMap();
        }
        foreach (KeyValuePair<Vector2Int, Tile> kvp in tiles)
        {
            if (kvp.Value.GetCurrentTileType() == Tile.TileTypes.DeepWater)
                continue;
            kvp.Value.SetCurrentTileType(kvp.Value.GetNextTileType());
            changeMap.SetColor(new Vector3Int(kvp.Key.x, kvp.Key.y, 0), Color.white);
        }
        Tilemap temp = tilemap;
        tilemap = changeMap;
        tilemap.gameObject.SetActive(true);
        changeMap = temp;
        changeMap.gameObject.SetActive(false);
        WeatherManager.Instance.SetNewWeather();
        CheckTiles(false);
    }

    public void CheckTiles(bool initial)
    {
        foreach (KeyValuePair<Vector2Int, Tile> kvp in tiles)
        {
            Tile.TileTypes newTileType = Tile.TileTypes.DeepWater;
            if (kvp.Value.GetCurrentTileType() != Tile.TileTypes.DeepWater)
            {
                newTileType = TileRules.GetNewTileType(kvp.Value.GetCurrentTileType(), kvp.Value.GetAdjacentTiles());
                kvp.Value.SetNextTileType(newTileType);
            }
            IsometricRuleTile newTile = TileInfo.Instance.GetTile(newTileType);
            Vector3Int loc = new Vector3Int(kvp.Key.x, kvp.Key.y, 0);
            if (changeMap.GetSprite(loc) != newTile.m_DefaultSprite)
            {
                changeMap.SetTile(loc, newTile); 
            }
            if (newTileType != kvp.Value.GetCurrentTileType())
            {
                changeMap.SetColor(loc, Color.red);
            }
        }
        changeMap.gameObject.SetActive(false);
    }

    public void GenerateMap()
    {
        tiles.Clear();
        tilemap.ClearAllTiles();
        if (!testing)
        {
            seed = DateTime.UtcNow.ToString().GetHashCode();
        }
        CustomMap customMap = FindObjectOfType<CustomMap>();
        if (customMap != null)
        {
            mapSize = customMap.GetMapSize();
            biomeScale = customMap.GetBiomeScale();
            tileScale = customMap.GetTileScale();
            seed = customMap.GetSeed();
            Debug.Log(biomeScale + " " + tileScale);
        }
        Debug.Log(seed);
        UnityEngine.Random.InitState(seed);
        tileOffset.x = UnityEngine.Random.Range(0f, 9999f);
        tileOffset.y = UnityEngine.Random.Range(0f, 9999f);
        Debug.Log(tileOffset);
        biomeOffset.x = -tileOffset.x;
        biomeOffset.y = -tileOffset.y;
        width = mapSize.x / 2;
        height = mapSize.y / 2;
        for (int r = -width - oceanExtension.x; r < width + oceanExtension.x; r++)
        {
            for (int c = -height - oceanExtension.y; c < height + oceanExtension.x; c++)
            {
                if (Mathf.Abs(r) < width && Mathf.Abs(c) < height)
                {
                    ActualMapFillIn(r, c);
                }
                else
                {
                    tilemap.SetTile(new Vector3Int(r, c, 0), TileInfo.Instance.GetTile(Tile.TileTypes.DeepWater));
                    changeMap.SetTile(new Vector3Int(r, c, 0), TileInfo.Instance.GetTile(Tile.TileTypes.DeepWater));
                }
            }
        }
        foreach (Vector2Int pos in tiles.Keys)
        {
            LinkTiles(tiles[pos], pos);
        }
    }

    private void ActualMapFillIn(int r, int c)
    {
        //Vector2Int pos = new Vector2Int(r, c);
        Vector2Int spawnPos = new Vector2Int(r, c);
        float tileRand = Noise.Get2DPerlin(spawnPos, mapSize, tileScale, tileOffset);
        float biomeRand = Noise.Get2DPerlin(spawnPos, mapSize, biomeScale, biomeOffset);
        Tile.TileTypes tileType;
        if (islandMode)
        {
            Vector2 dist = new Vector2(2 * (float)Mathf.Abs(spawnPos.x) / mapSize.x, 2 * (float)Mathf.Abs(spawnPos.y) / mapSize.y);
            tileType = TileInfo.Instance.GetTileTypeWaterEdge(biomeRand, tileRand, dist);
        }
        else
        {
            tileType = TileInfo.Instance.GetTileType(biomeRand, tileRand);
        }
        Vector3Int spawnPos3D = new Vector3Int(spawnPos.x, spawnPos.y, 0);
        Tile t = new Tile(tileType, spawnPos3D);
        tiles.Add(spawnPos, t);
        //LinkTiles(t, spawnPos);
        tilemap.SetTile(spawnPos3D, TileInfo.Instance.GetTile(t.GetCurrentTileType()));
    }

    public Vector2Int GetMapSize()
    {
        return mapSize;
    }

    public void LinkTiles(Tile t, Vector2Int pos)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                LinkTileWithRelative(t, pos, new Vector2Int(i, j));
            }
        }
    }

    public void LinkTileWithRelative(Tile t, Vector2Int pos, Vector2Int relative)
    {
        Tile linkTile = null;
        Vector2Int otherPos = pos;
        if (relative.x == 1)
        {
            if (relative.y == 0)
            {
                otherPos += new Vector2Int(-1,0);
            }
            else
            {
                otherPos += new Vector2Int(1, 0);
            }
        }
        else
        {
            if (relative.y == 0)
            {
                otherPos += new Vector2Int(0,-1);
            }
            else
            {
                otherPos += new Vector2Int(0,1);
            }
        }
        if (tiles.ContainsKey(otherPos))
        {
            linkTile = tiles[otherPos];
            if (linkTile.GetCurrentTileType() == Tile.TileTypes.DeepWater && t.GetCurrentTileType() != Tile.TileTypes.DeepWater && t.GetCurrentTileType() != Tile.TileTypes.Water)
            {
                t.SetCurrentTileType(Tile.TileTypes.Water);
                tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), TileInfo.Instance.GetTile(t.GetCurrentTileType()));
            }
        }
        t.AddTile(relative, linkTile);
    }

    // Predict ahead
    public void ViewChangeMap()
    {
        if (!viewingPrediction)
        {
            changeMap.gameObject.SetActive(true);
            tilemap.gameObject.SetActive(false);
            viewingPrediction = true;
        }
        else
        {
            changeMap.gameObject.SetActive(false);
            tilemap.gameObject.SetActive(true);
            viewingPrediction = false;
        }
    }

    public bool IsViewingPrediction()
    {
        return viewingPrediction;
    }

    public Vector2Int GetTileLocation(Vector2 ClickPos)
    {
        Vector3Int worldPos = tilemap.WorldToCell(ClickPos);
        return new Vector2Int(worldPos.x, worldPos.y);
    }

    public Tile GetTileAtLocation(Vector2 ClickPos)
    {
        return tiles[GetTileLocation(ClickPos)];
    }

    public Dictionary<Vector2Int, Tile> GetTileDictionary()
    {
        return tiles;
    }

    public Tilemap GetTilemap()
    {
        return tilemap;
    }

    public void ResetValidTilemap()
    {
        foreach(KeyValuePair<Vector2Int, Tile> kvp in tiles)
        {
            kvp.Value.SetIsValid(false);
        }
    }

    public Tile GetTile(Vector2Int loc)
    {
        if (!tiles.ContainsKey(loc))
        {
            return null;
        }
        return tiles[loc];
    }
}
