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
        Mine,
        House,
        WoodWall,
        StoneWall,

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
    [Serializable]
    public struct ResourceCost
    {
        public ResourceManager.ResourceTypes resourceType;

        public int amount;
    }

    public Tilemap buildingMap;
    public Dictionary<BuildingName, Building> buildingDictionary = new Dictionary<BuildingName, Building>();
    public Dictionary<Vector2Int, Building> builtBuildings = new Dictionary<Vector2Int, Building>();
    private Dictionary<Vector2Int, House> houses = new Dictionary<Vector2Int, House>();
    private Dictionary<Vector2Int, Wall> walls = new Dictionary<Vector2Int, Wall>();

    [SerializeField]
    public List<Building> buildingList = new List<Building>();

    [SerializeField] private AudioSource buildSound;
    [SerializeField] private AudioSource destroySound;

    [SerializeField] private AudioSource farmSound;
    [SerializeField] private AudioSource houseSound;
    [SerializeField] private AudioSource lumberSound;
    [SerializeField] private AudioSource waterSound;
    [SerializeField] private AudioSource mineSound;


    public void AdvanceTurn() {
        List<Vector2Int> players = new List<Vector2Int>();
        foreach(Vector2Int p in builtBuildings.Keys) {
            Building b = builtBuildings[p];
            BuildingName name = b.buildingType;
            Tile t = TileManager.Instance.GetTile(p);
            TileTypes tType = t.GetCurrentTileType();
            if(isWall(name)) {
                //Reduce condition of the wall and set tile back to original tile.
                if(!b.acceptedTiles.Contains(tType)) {
                    walls[p].reduceCondition();
                    TileManager.Instance.SetTile(p, b.acceptedTiles[0]);
                }
            }
            if(isDestroyed(b, tType, p)) {
                players.Add(p);
            } else {
                produceResources(name);
            }
            if (houses.ContainsKey(p)) {
                houses[p].AdvanceTurn();
            }
        }

        foreach (Vector2Int p in players)
        {
            destroyBuilding(p);
        }
    }

    public void destroyBuilding(Vector2Int p) {
        builtBuildings.Remove(p);
        buildingMap.SetTile( new Vector3Int(p.x, p.y, 1), null);
        TileManager.Instance.GetTile(p).setAudioSource(null);
        if(houses.ContainsKey(p)) {
            houses.Remove(p);
            if(!destroySound.isPlaying)
            {
                destroySound.Play();
            }
        }
        if(walls.ContainsKey(p)) {
            walls.Remove(p);
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

    //Return true if building type is valid for a given tile
    public bool canBuild(BuildingName name, Tile.TileTypes tile) {
        Building b = buildingDictionary[name];
        return b.acceptedTiles.Contains(tile);
    }

    // Check if a given tile has a building already on it, reversed from what it should be for some dumbass reason
    //RETURNS TRUE IF NO BUILDING, FALSE if THERE IS A BUILDING
    public bool hasBuilding(Tile tile)
    {
        return !(buildingMap.GetTile(new Vector3Int(tile.GetTilePos2().x, tile.GetTilePos2().y, 1)));
    }

    public void produceResources(BuildingName name) {
        //Resource Manager produce a certain resource
        if (name == BuildingName.House)
        {
            return;
        }
        ResourceManager.Instance.AddResource(buildingDictionary[name].resourceProduced, buildingDictionary[name].amountProduced);
    }

    public void buildBuilding(BuildingName name, Vector2Int p) {
        if(canAfford(name)){
            Building b = buildingDictionary[name];
            // builtBuildings.Add(p, b);
            if(!buildSound.isPlaying)
            {
                buildSound.Play();
            }

            Tile tile = TileManager.Instance.GetTile(p);

            switch(b.buildingType)
            {
                case BuildingName.Farm:
                    tile.setAudioSource(farmSound);
                    break;
                case BuildingName.House:
                    tile.setAudioSource(houseSound);
                    break;
                case BuildingName.Lumber:
                    tile.setAudioSource(lumberSound);
                    break;
                case BuildingName.WaterWell:
                    tile.setAudioSource(waterSound);
                    break;
                case BuildingName.Mine:
                    tile.setAudioSource(mineSound);
                    break;
            }

            Settler settler = null;

            if (b.buildingType == BuildingName.House)
            {
                if(houses.Count < SettlerManager.Instance.GetCurrentNumberOfSettlers())
                {
                    foreach(GameObject go in SettlerManager.Instance.GetSettlers())
                    {
                        Settler s = go.GetComponent<Settler>();
                        
                        if(!houses.ContainsKey(s.GetHousePos()))
                        {
                            settler = s;
                            break;
                        }
                    }
                }
                else
                {
                    Vector2Int spawnPos = p;
                    foreach(Tile t in TileManager.Instance.GetTile(p).GetAdjacentTiles())
                    {
                        bool hasSettler = SettlerManager.Instance.GetSettlerAtPos(t.GetTilePos2()) != null;

                        if(t.GetIsWalkable() && !hasSettler)
                        {
                            spawnPos = t.GetTilePos2();
                        }
                    }

                    if(spawnPos != p)
                    {
                        SettlerManager.Instance.AddSettlerAtTile(TileManager.Instance.GetTile(spawnPos));
                        settler = SettlerManager.Instance.GetSettlers()[SettlerManager.Instance.GetCurrentNumberOfSettlers() - 1].GetComponent<Settler>();
                    }
                }
                
                if(settler != null)
                {
                    House house = null;
                    houses.Add(p, house = new House(p));
                    house.SetSettler(settler);
                }
            }
            if (isWall(b.buildingType)) {
                Wall wall = new Wall(p);
                if(b.buildingType == BuildingName.WoodWall) {
                    wall.setCondition(1);
                } else if(b.buildingType == BuildingName.StoneWall) {
                    wall.setCondition(2);
                }
                walls.Add(p, wall);
                Debug.Log(p);
                //Wall sets its only accepted tile as the one it was placed on
                Tile t = TileManager.Instance.GetTile(p);
                TileTypes tType = t.GetCurrentTileType();
                List<TileTypes> tiles;
                tiles = new List<Tile.TileTypes>();
                tiles.Add(tType);
                b.acceptedTiles = tiles;
            }
            
            buildingMap.SetTile(new Vector3Int(p.x, p.y, 1), b.isometricTile);
            //Take away resources to build
            foreach(ResourceCost c in b.resourceCostList){
                ResourceManager.Instance.RemoveResource(c.resourceType, c.amount);
            }

            builtBuildings.Add(p, b);
        }


    }

    // for placing the free initial player houses
    public void PlaceInitialHouse(Vector2Int p)
    {
        Building b = buildingDictionary[BuildingName.House];
        builtBuildings.Add(p, b);
        House h = null;
        houses.Add(p, h = new House(p));

        Settler s = SettlerManager.Instance.GetSettlerAtPos(p);
        h.SetSettler(s);

        buildingMap.SetTile(new Vector3Int(p.x, p.y, 1), b.isometricTile);
    }

    public bool isDestroyed(Building b, Tile.TileTypes tileType, Vector2Int pos) {
        if(isWall(b.buildingType)) {
            return !(walls[pos].getCondition() > 0);
        } else {
            return !b.acceptedTiles.Contains(tileType);
        }
        
    }

    public Dictionary<Vector2Int, House> GetHouses()
    {
        return houses;
    }

    public Dictionary<Vector2Int, Wall> GetWalls()
    {
        return walls;
    }

    public bool isWall(BuildingName name) {
        return (name == BuildingName.WoodWall || name == BuildingName.StoneWall);
    }

    public Building GetBuilding(BuildingName buildingName)
    {
        return buildingDictionary[buildingName];
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
