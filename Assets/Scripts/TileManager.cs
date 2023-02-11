using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : Singleton<TileManager>
{
    [SerializeField]
    private Tilemap tilemap;
    [SerializeField]
    private int seed;
    [SerializeField, Tooltip("Keeps entered seed")]
    private bool testing;
    [SerializeField, Tooltip("Works with pretty much any size (does not need to be equal)")]
    private Vector2Int mapSize;
    [SerializeField, Range(0.1f,50), Tooltip("Smaller the number, larger the groups")]
    private float tileScale;
    [SerializeField, Range(0.1f, 50), Tooltip("Smaller the number, larger the biomes")]
    private float biomeScale;
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private Vector2 tileOffset;
    private Vector2 biomeOffset;
    [SerializeField, Tooltip("Generates surrounding ocean")]
    private bool islandMode;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        tiles.Clear();
        tilemap.ClearAllTiles();
        if (!testing)
        {
            seed = DateTime.UtcNow.ToString().GetHashCode();
        }
        UnityEngine.Random.InitState(seed);
        tileOffset.x = UnityEngine.Random.Range(0f, 9999f);
        tileOffset.y = UnityEngine.Random.Range(0f, 9999f);
        biomeOffset.x = -tileOffset.x;
        biomeOffset.y = -tileOffset.y;
        for (int r = 0; r < mapSize.x; r++)
        {
            for (int c = 0; c < mapSize.y; c++)
            {
                Vector2Int pos = new Vector2Int(r, c);
                Vector2Int spawnPos = new Vector2Int(r-mapSize.x / 2,c-mapSize.y /2);
                float tileRand = Noise.Get2DPerlin(pos, mapSize, tileScale, tileOffset);
                float biomeRand = Noise.Get2DPerlin(pos, mapSize, biomeScale, biomeOffset);
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
                Tile t = new Tile(tileType);
                tiles.Add(pos, t);
                LinkTiles(t, pos);
                tilemap.SetTile(new Vector3Int(spawnPos.x, spawnPos.y, 0), TileInfo.Instance.GetTile(tileType));
            }
        }
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
        }
        t.AddTile(relative, linkTile);
        if (linkTile != null)
        {
            Vector2Int newRelPos = new Vector2Int(relative.x, relative.y);
            if (relative.y == 0)
            {
                newRelPos.y = 1;
            }
            else
            {
                newRelPos.y = 0;
            }
            tiles[pos].AddTile(newRelPos, t);
        }
    }
}
