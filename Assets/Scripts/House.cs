using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House
{
    Settler settler;
    Vector2Int location;

    public House(Vector2Int position)
    {
        location = position;
    }

    public void AdvanceTurn()
    {
        if (settler == null)
        {
            Debug.Log("No Settler so Spawning New One");
            SettlerManager.Instance.AddSettlerAtTile(TileManager.Instance.GetTile(location));
        }
        else if (settler.isSettlerDead())
        {
            Debug.Log("Respawn Settler");
            settler.gameObject.SetActive(true);
            settler.SetInitialTileAndPosition(TileManager.Instance.GetTile(location), false);
            settler.Respawn();
        }
    }

    public void SetSettler(Settler s)
    {
        settler = s;
    }
}
