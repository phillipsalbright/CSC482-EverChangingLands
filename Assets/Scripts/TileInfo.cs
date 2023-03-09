using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEngine;

public class TileInfo : Singleton<TileInfo>
{
    public enum Biomes
    {
        Grass,
        Water,
        Sand,
    }
    [Serializable]
    public struct TileSwitch
    {
        public Tile.TileTypes switchTile;
        public List<ResourceManager.ResourceTypes> requiredResources;
        public List<int> requiredResourcesCount;
    }
    [Serializable]
    public struct TileComponents
    {
        public Tile.TileTypes tileType;
        public IsometricRuleTile isometricTile;
        [Range(0,1)]
        public float chance;

        public ResourceManager.ResourceTypes associatedResource;
        public List<TileSwitch> tileSwitches;
    }
    [Serializable]
    public struct BiomeList
    {
        public Biomes biome;
        [Range (0,1)]
        public float chance;
        public List<TileComponents> tiles;
    }
    [SerializeField]
    private List<BiomeList> biomeLists = new List<BiomeList>();
    private Dictionary<Biomes, BiomeList> biomeDict = new Dictionary<Biomes, BiomeList>();
    private Dictionary<Biomes, Dictionary<Tile.TileTypes, TileComponents>> tilesDict = new Dictionary<Biomes, Dictionary<Tile.TileTypes, TileComponents>>();
    private Dictionary<Biomes, float> tilesChanceSumDict = new Dictionary<Biomes, float>();
    private Dictionary<Tile.TileTypes, IsometricRuleTile> isometricTiles = new Dictionary<Tile.TileTypes, IsometricRuleTile>();
    private Dictionary<Tile.TileTypes, TileComponents> tileList = new Dictionary<Tile.TileTypes, TileComponents>();
    private float biomeChanceSum = 0;
    [SerializeField]
    private IsometricRuleTile deepWaterTile;
    [SerializeField, Range(0,1)]
    private float deepWaterStart;
    void Awake()
    {
        base.Awake();
        SetupDictionaries();
    }

    public void SetupDictionaries()
    {
        biomeDict.Clear();
        isometricTiles.Clear();
        tilesChanceSumDict.Clear();
        tilesDict.Clear();
        biomeChanceSum = 0;
        isometricTiles.Add(Tile.TileTypes.DeepWater, deepWaterTile);
        foreach (BiomeList biomeList in biomeLists)
        {
            biomeDict.Add(biomeList.biome, biomeList);
            tilesDict.Add(biomeList.biome, new Dictionary<Tile.TileTypes, TileComponents>());
            tilesChanceSumDict.Add(biomeList.biome, 0);
            foreach (TileComponents tileComp in biomeList.tiles)
            {
                tilesDict[biomeList.biome].Add(tileComp.tileType, tileComp);
                tilesChanceSumDict[biomeList.biome] += tileComp.chance;
                isometricTiles.Add(tileComp.tileType, tileComp.isometricTile);
                tileList.Add(tileComp.tileType, tileComp);
            }
            biomeChanceSum += biomeList.chance;
        }
    }

    public IsometricRuleTile GetTile(Tile.TileTypes tileType)
    {
        if (!isometricTiles.ContainsKey(tileType))
        {
            Debug.LogError("Failure to find tile " + tileType);
            return deepWaterTile;
        }
        return isometricTiles[tileType];
    }

    public ResourceManager.ResourceTypes GetTileResourceTypes(Tile.TileTypes type)
    {
        return tileList[type].associatedResource;
    }

    public Tile.TileTypes GetTileType(float biomeRand, float tileRand)
    {
        biomeRand *= biomeChanceSum;
        float totalChance = 0;
        foreach (Biomes biome in biomeDict.Keys)
        {
            float totalMin = totalChance;
            totalChance += biomeDict[biome].chance;
            if (biomeRand <= totalChance)
            {
                return GetTileFromBiome(tileRand, biome);
            }
        }
        return Tile.TileTypes.Desert;
    }

    public Tile.TileTypes GetTileFromBiome(float rand, Biomes biome)
    {
        rand *= tilesChanceSumDict[biome];
        float totalChance = 0;
        foreach (KeyValuePair<Tile.TileTypes, TileComponents> tiles in tilesDict[biome])
        {
            totalChance += tiles.Value.chance;
            if (rand <= totalChance)
            {
                return tiles.Key;
            }
        } 
        return Tile.TileTypes.Grass;
    }

    public Tile.TileTypes GetTileTypeWaterEdge(float biomeRand, float tileRand, Vector2 distToEdge)
    {
        biomeRand *= biomeChanceSum;
        float distance = Mathf.Clamp(distToEdge.magnitude-deepWaterStart,0,1);
        distance /= (1-deepWaterStart);
        float totalChance = distance;
        float nonWaterChance = biomeChanceSum - distance;
        if (biomeRand < distance) {
            return GetTileFromBiomeWaterEdge(tileRand, Biomes.Water, distance);
        }
        foreach (KeyValuePair<Biomes, BiomeList> biomes in biomeDict)
        {
            float randMin = totalChance;
            totalChance += (biomes.Value.chance / biomeChanceSum) * nonWaterChance;
            if (biomeRand <= totalChance)
            {
                return GetTileFromBiomeWaterEdge(tileRand, biomes.Key, distance);
            }
        }
        return Tile.TileTypes.DeepWater;    
    }

    public Tile.TileTypes GetTileFromBiomeWaterEdge(float rand, Biomes biome, float distance)
    {
        rand *= tilesChanceSumDict[biome];
        float totalChance = distance;
        float nonWaterChance = tilesChanceSumDict[biome] - distance;
        if (rand < distance)
        {
            return Tile.TileTypes.DeepWater;
        }
        foreach (KeyValuePair<Tile.TileTypes, TileComponents> tiles in tilesDict[biome])
        {
            totalChance += (tiles.Value.chance / tilesChanceSumDict[biome]) * nonWaterChance;
            if (rand <= totalChance)
            {
                return tiles.Key;
            }
        } 
        return Tile.TileTypes.Grass;
    }

    public List<TileSwitch> GetTileSwitches(Tile.TileTypes tileType)
    {
        return tileList[tileType].tileSwitches;
    }
}
