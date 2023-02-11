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
    [SerializeField]
    private bool testing;
    [SerializeField]
    private Vector2Int mapSize;
    [SerializeField, Range(0.1f,50)]
    private float tileScale;
    [SerializeField, Range(0.1f, 50)]
    private float biomeScale;
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private Vector2 tileOffset;
    private Vector2 biomeOffset;
    [SerializeField]
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
        for (int r = 0; r < mapSize.y; r++)
        {
            for (int c = 0; c < mapSize.x; c++)
            {
                Vector2Int pos = new Vector2Int(r, c);
                Vector2Int spawnPos = new Vector2Int(r-mapSize.x / 2,c-mapSize.y /2);
                float tileRand = Noise.Get2DPerlin(pos, mapSize, tileScale, tileOffset);
                float biomeRand = Noise.Get2DPerlin(pos, mapSize, biomeScale, biomeOffset);
                Tile.TileTypes tileType;
                if (islandMode)
                {
                    Vector2 dist = new Vector2(2 * (float)Mathf.Abs(spawnPos.x) / mapSize.x, (float)Mathf.Abs(spawnPos.y) / (mapSize.y / 2));
                    tileType = TileInfo.Instance.GetTileTypeWaterEdge(biomeRand, tileRand, dist);
                }
                else
                {
                    tileType = TileInfo.Instance.GetTileType(biomeRand, tileRand);
                }
                Tile t = new Tile(tileType);
                tiles.Add(pos, t);
                tilemap.SetTile(new Vector3Int(spawnPos.x, spawnPos.y, 0), TileInfo.Instance.GetTile(tileType));
            }
        }
    }

    public Vector2Int GetMapSize()
    {
        return mapSize;
    }
}
