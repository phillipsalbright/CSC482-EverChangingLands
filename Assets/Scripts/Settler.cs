using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : MonoBehaviour
{
    private Tile currentTile;

    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
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
        transform.position = position;
    }
}
