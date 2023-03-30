using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settler : MonoBehaviour
{
    [SerializeField] private Tile currentTile;
    [SerializeField] private Vector2Int positionInTilemap;

    private SpriteRenderer sprite;

    private bool canMove = true;
    private bool canCollect = true;
    private bool canFlip = true;
    private bool isDead = false;
    private bool wantsToDie = false;
    private Vector2Int housePos;

    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource respawnSound;

    private int hunger = 5;

    // Start is called before the first frame update
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(deathSound.isPlaying)
        {
            Debug.Log("Wants to die");
            wantsToDie = true;
        }
        else if(wantsToDie && !deathSound.isPlaying)
        {
            wantsToDie = false;
            MusicManager.Instance.playBGM();
            if (!BuildingManager.Instance.GetHouses().ContainsKey(housePos))
            {
                Debug.Log("House Gone");
                SettlerManager.Instance.GetSettlers().Remove(gameObject);
                Destroy(gameObject);
            }
            gameObject.SetActive(false);
        }
    }

    public void SetInitialTileAndPosition(Tile tile, bool canMove)
    {
        if(!sprite.enabled)
        {
            sprite.enabled = true;
        }
        currentTile = tile;
        this.canMove = canMove;
        canCollect = true;

        positionInTilemap = tile.GetTilePos2();
        housePos = positionInTilemap;

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

                if (xDifference + yDifference <= SettlerManager.Instance.GetMaxSettlerMovement())
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
        if (ResourceManager.Instance.getResourceCount(ResourceManager.ResourceTypes.Food) >= hunger)
        {
            ResourceManager.Instance.RemoveResource(ResourceManager.ResourceTypes.Food, hunger);
        }
        else
        {
            isDead = true;
        }
        if(!currentTile.GetIsWalkable())
        {
            Die();
        }
        else
        {
            canMove = true;
            canCollect = true;
            canFlip = true;
        }
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
        if (this.canCollect)
        {
            //Now that most tiles have a resource, this if statement stops water from being collected
            if(!ResourceManager.Instance.AddResource(TileInfo.Instance.GetTileResourceTypes(this.GetCurrentTile().GetCurrentTileType()), 5) || true)
            {
                bool collectedWater = false;
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {

                        if (!collectedWater && this.GetCurrentTile().GetAdjacentTiles()[i, j].GetCurrentTileType() == Tile.TileTypes.Water)
                        {
                            ResourceManager.Instance.AddResource(ResourceManager.ResourceTypes.Water, 5);
                            collectedWater = true;
                        }
                    }
                }
            }
            canCollect = false;
        }
    }

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    public bool isSettlerDead()
    {
        return isDead;
    }

    public void Die()
    {
        Debug.Log("Settler Dies");
        isDead = true;
        canMove = false;
        canCollect = false;
        if (!deathSound.isPlaying)
        {
            MusicManager.Instance.pauseBGM();
            Debug.Log("Death sound play");
            deathSound.Play();
        }
    }

    public void Respawn()
    {
        Debug.Log("Settler is respawning");
        isDead = false;
        canMove = true;
        canCollect = true;
        if(!respawnSound.isPlaying)
        {
            respawnSound.Play();
        }
    }

    public void SetHousePos(Vector2Int pos)
    {
        housePos = pos;
    }


    public Vector2Int GetHousePos()
    {
        return housePos;
    }

    public void FlipTile(Tile t, Tile.TileTypes type)
    {

        TileManager.Instance.SetTile(t.GetTilePos2(), type);
        canFlip = false;
    }

    public bool GetCanFlip()
    {
        return canFlip;
    }
}
