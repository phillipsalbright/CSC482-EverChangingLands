using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RulePanelTileUI : MonoBehaviour
{
    [SerializeField,Tooltip("whether this is an AllTiles panel or not")]
    public bool allTiles = false;
    [SerializeField, Tooltip("the tile this panel is related for")]
    public Tile.TileTypes thisTileType = Tile.TileTypes.DeepWater;

    public void ChangeTileType(){
        RuleCreationUIManager.Instance.SetNewTileType(gameObject, allTiles, thisTileType);
    } 

    public void ChangeColor(Color newColor){
        gameObject.GetComponent<Image>().color = newColor;
    }

    public Color GetColor(){
        return gameObject.GetComponent<Image>().color;
    }

    void Start(){
        if(allTiles){
            RuleCreationUIManager.Instance.SetDefaultColor(GetColor());
        }
    }
}
