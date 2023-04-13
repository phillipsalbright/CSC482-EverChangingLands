using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulePanelTileUI : MonoBehaviour
{
    [SerializeField,Tooltip("whether this is an AllTiles panel or not")]
    public bool allTiles = false;
    [SerializeField, Tooltip("the tile this panel is related for")]
    public Tile.TileTypes thisTileType = Tile.TileTypes.DeepWater;

    public void ChangeTileType(){
        RuleCreationUIManager.Instance.SetNewTileType(allTiles, thisTileType);
    } 
}
