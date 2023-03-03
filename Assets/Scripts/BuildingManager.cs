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

        public ResourceManager.ResourceTypes resourceProduced;

        public int amountProduced;

        public List<Tile.TileTypes> acceptedTiles;
    }

    public struct ResourceCost
    {
        public ResourceManager.ResourceTypes resourceType;

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
            if(canBuild(b.buildingType, tileType)) {
                list.Add(b.buildingType);
            }
        }
        return list;
    }

    public List<BuildingName> getAffordableBuildings(List<BuildingName> bList) {
        List<BuildingName> list = new List<BuildingName>();
        foreach(BuildingName name in bList) {
            if(canAfford(name)) {
                list.Add(name);
            }
        }
        return list;
    }

    //Return true if building can be afforded
    public bool canAfford(BuildingName name) {
        Building b = buildingDictionary[name];
        bool hasResources = true;
        foreach(ResourceCost c in b.resourceCostList) {
            if(c.amount > ResourceManager.Instance.getResourceCount(c.resourceType)) {
                hasResources = false;
            }
        }
        return hasResources;
    }

    //Return true if building is on proper tile
    public bool canBuild(BuildingName name, Tile.TileTypes tileType) {
        Building b = buildingDictionary[name];
        return b.acceptedTiles.Contains(tileType);
    }

    public void produceResources(BuildingName name) {
        //Resource Manager produce a certain resource
        ResourceManager.Instance.AddResource(buildingDictionary[name].resourceProduced, buildingDictionary[name].amountProduced);
    }

    public void buildBuilding(BuildingName name, Vector2Int p) {
        if(canAfford(name)){
            Building b = buildingDictionary[name];
            builtBuildings.Add(p, name);
            buildingMap.SetTile(new Vector3Int(p.x, p.y, 1), b.isometricTile);
            //Take away resources to build
            foreach(ResourceCost c in b.resourceCostList){
                ResourceManager.Instance.RemoveResource(c.resourceType, c.amount);
            }
        }
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
