using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MenuGenerator : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    [SerializeField] private TileBase transparentTile;
    // Start is called before the first frame update

    [SerializeField] private GameObject[] maps;
    [SerializeField] private Vector2 mapRePos;
    private Vector3 movementDirection = (new Vector3(-2, -1f, 0)).normalized;
    void Start()
    {
       // Debug.Log(tilemap.GetTile(new Vector3Int(1, 1, 1)));
        Vector2Int mapsize = new Vector2Int(300, 300);
        int width = mapsize.x / 2;
        int height = mapsize.y / 2;
        for (int r = -width; r < width; r++)
        {
            for (int c = -height; c < height; c++)
            { 
                Tile t = TileManager.Instance.GetTile(new Vector2Int(r, c));
                TileBase t2 = tilemap.GetTile(new Vector3Int(r, c, 0));
                TileData td;
               // Debug.Log(tilemap.GetSprite(new Vector3Int(r, c, 0)));
                if (tilemap.GetSprite(new Vector3Int(r, c, 0)) != null)
                {
                    string spriteName = tilemap.GetSprite(new Vector3Int(r, c, 0)).name;
                    if (spriteName == "reticle" && t != null)
                    {

                        tilemap.SetTile(new Vector3Int(r, c, 0), TileInfo.Instance.GetTile(t.GetCurrentTileType()));
                    }
                    else if (spriteName != "Black")
                    {

                        tilemap.SetTile(new Vector3Int(r, c, 0), transparentTile);
                    }
                }

            }
        }
        //TileManager.Instance.gameObject.SetActive(false);
    }
    void Update()
    {
        for (int i = 0; i < maps.Length; i++)
        {
         //   Debug.Log(movementDirection*Time.deltaTime * 1 + "   " + maps[i].transform.position + "   " + (maps[i].transform.position + movementDirection * Time.deltaTime * 1));
            maps[i].transform.position += movementDirection * Time.deltaTime * 2; 
            if (maps[i].transform.localPosition.x < -65)
            {
                maps[i].transform.localPosition = mapRePos;
            }
        }


    }
}
