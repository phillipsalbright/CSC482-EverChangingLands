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
    private float scale;
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();
    private Vector2 offset;
    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
    }

    public void GenerateMap()
    {
        tiles.Clear();
        if (!testing)
        {
            seed = DateTime.UtcNow.ToString().GetHashCode();
        }
        UnityEngine.Random.InitState(seed);
        offset.x = UnityEngine.Random.Range(0f, 9999f);
        offset.y = UnityEngine.Random.Range(0f, 9999f);
        for (int r = 0; r < mapSize.y; r++)
        {
            for (int c = 0; c < mapSize.x; c++)
            {
                Vector2Int pos = new Vector2Int(r, c);
                float rand = Noise.Get2DPerlin(pos, mapSize, scale, offset);
                Tile.TileTypes tileType = TileInfo.Instance.GetTileType(rand);
                Tile t = new Tile(tileType);
                tiles.Add(pos, t);
                tilemap.SetTile(new Vector3Int(pos.x, pos.y, 0), TileInfo.Instance.GetTile(tileType));
            }
        }
    }
}
