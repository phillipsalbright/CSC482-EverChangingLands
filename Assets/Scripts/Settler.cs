using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private Vector2Int positionInTilemap;

    [SerializeField] private int maxMovementRange = 5;

    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetCurrentTileAndPosition(Tile tile)
    {
        if(!sprite.enabled)
        {
            sprite.enabled = true;
        }
        currentTile = tile;
        Dictionary<Vector2Int, Tile> tiles = FindObjectOfType<TileManager>().GetTileDictionary();

        foreach(KeyValuePair<Vector2Int, Tile> kvp in tiles)
        {
            if(kvp.Value == currentTile)
            {
                positionInTilemap = kvp.Key;
            }
        }

        transform.position = FindObjectOfType<TileManager>().GetTilemap().GetCellCenterWorld(new Vector3Int(positionInTilemap.x, positionInTilemap.y, 0));
    }

    public void MoveSettler(Tile newTile)
    {
        Dictionary<Vector2Int, Tile> tiles = FindObjectOfType<TileManager>().GetTileDictionary();
        Vector2Int newTileCoordinates = new Vector2Int();

        foreach (KeyValuePair<Vector2Int, Tile> kvp in tiles)
        {
            if (kvp.Value == newTile)
            {
                newTileCoordinates = kvp.Key;
            }
        }

        if(newTileCoordinates != null)
        {
            int xDifference = Mathf.Abs(positionInTilemap.x - newTileCoordinates.x);
            int yDifference = Mathf.Abs(positionInTilemap.y - newTileCoordinates.y);

            if (xDifference + yDifference <= maxMovementRange)
            {
                currentTile = newTile;
                positionInTilemap = newTileCoordinates;
                transform.position = FindObjectOfType<TileManager>().GetTilemap().GetCellCenterWorld(new Vector3Int(newTileCoordinates.x, newTileCoordinates.y, 0));
            }
        }
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }
}
