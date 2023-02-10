using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : Singleton<TileInfo>
{
    [SerializeField]
    private List<Tile.TileTypes> tileTypes = new List<Tile.TileTypes>();
    [SerializeField]
    private List<IsometricRuleTile> isometricRules = new List<IsometricRuleTile>();
    [SerializeField]
    private List<float> chances = new List<float>();
    private Dictionary<Tile.TileTypes, IsometricRuleTile> tileDictionary = new Dictionary<Tile.TileTypes, IsometricRuleTile>();
    private Dictionary<Tile.TileTypes, float> chanceDictionary = new Dictionary<Tile.TileTypes, float>();

    void Awake()
    {
        for (int i = 0; i < tileTypes.Count && i < isometricRules.Count && i < chances.Count; i++)
        {
            tileDictionary.Add(tileTypes[i], isometricRules[i]);
            chanceDictionary.Add(tileTypes[i], chances[i]);
        }
    }

    public float GetChance(Tile.TileTypes tileType)
    {
        return chanceDictionary[tileType];
    }

    public IsometricRuleTile GetTile(Tile.TileTypes tileType)
    {
        return tileDictionary[tileType];
    }
}
