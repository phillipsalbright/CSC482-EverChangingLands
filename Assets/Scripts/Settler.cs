using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private Vector2Int positionInTilemap;

    [SerializeField] private int maxMovementRange = 5;

    private SpriteRenderer sprite;

    private bool canMove = true;
    private bool canCollect = true;
    private bool isDead = false;

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

        positionInTilemap = tile.GetTilePos2();

        transform.position = FindObjectOfType<TileManager>().GetTilemap().GetCellCenterWorld(new Vector3Int(positionInTilemap.x, positionInTilemap.y, 0));
    }

    public void MoveSettler(Tile newTile)
    {
        SettlerManager sm = FindObjectOfType<SettlerManager>();
        bool settlerAtTile = false;
        foreach(GameObject s in sm.GetSettlers())
        {
            if(s.GetComponent<Settler>().GetCurrentTile() == newTile)
            {
                settlerAtTile = true;
                break;
            }
        }

        bool compatableTile = !settlerAtTile && newTile.GetCurrentTileType() != Tile.TileTypes.Water && newTile.GetCurrentTileType() != Tile.TileTypes.DeepWater && newTile.GetIsValid();
        if(compatableTile)
        {
            Vector2Int newTileCoordinates = newTile.GetTilePos2();

            if (canMove && newTileCoordinates != null)
            {
                int xDifference = Mathf.Abs(positionInTilemap.x - newTileCoordinates.x);
                int yDifference = Mathf.Abs(positionInTilemap.y - newTileCoordinates.y);

                if (xDifference + yDifference <= maxMovementRange)
                {
                    currentTile = newTile;
                    positionInTilemap = newTileCoordinates;
                    transform.position = TileManager.Instance.GetTilemap().GetCellCenterWorld(new Vector3Int(newTileCoordinates.x, newTileCoordinates.y, 0));

                    canMove = false;
                }
            }
        }

        TileManager.Instance.ResetValidTilemap();
    }

    public void StartNewTurn()
    {
        canMove = true;
        canCollect = true;
    }

    public bool GetCanMove()
    {
        return canMove;
    }

    public bool GetCanCollect()
    {
        return canCollect;
    }

    public void CollectResource()
    {
        ResourceManager.Instance.AddResource(this.GetCurrentTile().GetResourceType(), 5);
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    public bool IsSettlerDead()
    {
        return isDead;
    }
}
