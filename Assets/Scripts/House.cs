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
            SettlerManager.Instance.AddSettlerAtTile(TileManager.Instance.GetTileDictionary()[location]);
        }
    }
}
