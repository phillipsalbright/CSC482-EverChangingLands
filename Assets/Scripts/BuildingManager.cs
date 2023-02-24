using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Tile;

public class BuildingManager : Singleton<BuildingManager>
{
    
    public enum BuildingName
    {
        Lumber,
        Farm,
        WaterWell,
    }
    
    [Serializable]
    public struct Building
    {
        public BuildingName buildingType;

        public IsometricRuleTile isometricTile;

        public List<ResourceCost> resourceCostList;

        public int resourceProduced;

        public int amountProduced;

        public List<Tile.TileTypes> acceptedTiles;
    }

    public struct ResourceCost
    {
        public int resourceType;

        public int amount;
    }

    public Tilemap buildingMap;
    public Dictionary<BuildingName, Building> buildingDictionary;
    public Dictionary<Vector2Int, BuildingName> builtBuildings;

    [SerializeField]
    public List<Building> buildingList = new List<Building>();


    public void AdvanceTurn() {
        foreach(Vector2Int p in builtBuildings.Keys) {
            BuildingName name = builtBuildings[p];
            Tile t = TileManager.Instance.GetTileAtLocation(p);
            TileTypes tType = t.GetCurrentTileType();
            if(isDestroyed(name, tType)) {
                builtBuildings.Remove(p);
                buildingMap.SetTile( new Vector3Int(p.x, p.y, 1), null);
            } else {
                produceResources(name);
            }
        }
    }

    public List<BuildingName> getAvailableBuildings(Tile.TileTypes tileType) {
        List<BuildingName> list = new List<BuildingName>();
        foreach(Building b in buildingList) {
            if(b.acceptedTiles.Contains(tileType)) {
                list.Add(b.buildingType);
            }
        }
        return list;
    }

    public void produceResources(BuildingName name) {
        //Resource Manager produce a certain resource AARRONNNNNNNNNNN
    }

    public void buildBuilding(BuildingName name, Vector2Int p) {
        builtBuildings.Add(p, name);
        buildingMap.SetTile(new Vector3Int(p.x, p.y, 1), buildingDictionary[name].isometricTile);
    }

    public bool isDestroyed(BuildingName name, Tile.TileTypes tileType) {
        return !buildingDictionary[name].acceptedTiles.Contains(tileType);
    }


    
    // Awake is called before the first frame update
    protected override void Awake()
    {
        foreach(Building b in buildingList) {
            buildingDictionary.Add(b.buildingType, b);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
