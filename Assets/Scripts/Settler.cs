using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private Vector2Int positionInTilemap;

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

    public void SetCurrentTileAndPosition(Tile tile, Vector2 position)
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

        transform.position = position;
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }
}
