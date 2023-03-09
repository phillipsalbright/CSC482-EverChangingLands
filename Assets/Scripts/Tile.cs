using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Cecil;
using UnityEngine;

public class Tile
{
    public enum TileTypes
    {
        Desert,
        Grass,
        Mud,
        Forest,
        Dirt,
        Water,
        DeepWater,
    }

    private TileTypes currentTileType;
    private TileTypes nextTileType;
    //0, 0 - Bottom
    //0, 1 - Above
    //1, 0 - Left
    //1, 1 - Right
    private Tile[,] adjacentTiles = new Tile[2,2];

    [SerializeField]
    private bool tileChanging = false;

    private Vector3Int tilePos;
    private bool isValid;
    private bool isWalkable;

    public Vector2Int GetTilePos2()
    {
        return new Vector2Int(tilePos.x, tilePos.y);
    }

    public Tile(TileTypes currentTileType, Vector3Int tileLoc)
    {
        SetCurrentTileType(currentTileType);
        SetNextTileType(currentTileType);
        tilePos = tileLoc;
        isValid = false;
    }

    public Vector3Int GetTilePosition()
    {
        return tilePos;
    }

    public void SetIsValid(bool valid)
    {
        isValid = valid;
    }

    public bool GetIsValid()
    {
        return isValid;
    }

    public bool GetIsWalkable()
    {
        return isWalkable;
    }

    //checks if the tile will change terrain next turn. use for forecasting
    public bool IsChanging()
    {
        return currentTileType != nextTileType;
    }

    //sets the current tile's terrain without recalling forecast methods
    public void SetCurrentTileType(TileTypes tileType)
    {
        currentTileType = tileType;
        isWalkable = currentTileType != Tile.TileTypes.Water && currentTileType != Tile.TileTypes.DeepWater;
    }

    //sets the current tile's terrain and recall forecast methods
    public void PlayerChangeTileType(TileTypes tileType){
        SetCurrentTileType(tileType);
        CheckNextType();
        if(tileChanging){
            foreach (Tile adjTile in adjacentTiles)
            {
                adjTile.CheckNextType();
            }
        }
    }

    //returns the current terrain type of this tile
    public TileTypes GetCurrentTileType()
    {
        return currentTileType;
    }

    //sets the next terrain type for this tile
    public void SetNextTileType(TileTypes tileType)
    {
        nextTileType = tileType;
    }

    //returns the next terain type of this tile
    public TileTypes GetNextTileType()
    {
        return nextTileType;
    }

    //adds a tile reference to one of the adjacent tiles
    public void AddTile(Vector2Int location, Tile t)
    {
        adjacentTiles[location.x, location.y] = t;
    }

    //calls the tile rules to determine this tile's next terrain (CALL ON THE SECOND ROUND OF PASSING THROUGH WITH THE TILE MANAGER FOR CHANGING TURNS)
    public void CheckNextType(){
        nextTileType = TileRules.GetNewTileType(currentTileType,adjacentTiles);
        tileChanging = nextTileType != currentTileType;
    }

    //moves forwards to the next tile type (CALL ON THE FIRST ROUND OF PASSING THROUGH WITH THE TILE MANAGER FOR CHANGING TURNS)
    public void NextTurn(){
        currentTileType = nextTileType;
    }

    //checks if the tile will change next turn
    public bool WillTileChange(){
        return tileChanging;
    }

    public Tile[,] GetAdjacentTiles()
    {
        return adjacentTiles;
    }
}
