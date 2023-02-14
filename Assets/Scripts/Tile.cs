using System.Collections;
using System.Collections.Generic;
using System.Data;
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

    public Tile(TileTypes currentTileType)
    {
        SetCurrentTileType(currentTileType);
        SetNextTileType(currentTileType);
    }

    public bool IsChanging()
    {
        return currentTileType != nextTileType;
    }

    public void SetCurrentTileType(TileTypes tileType)
    {
        currentTileType = tileType;
    }

    public void PlayerChangeTileType(TileTypes tileType){
        SetCurrentTileType(tileType);
    }

    public TileTypes GetCurrentTileType()
    {
        return currentTileType;
    }

    public void SetNextTileType(TileTypes tileType)
    {
        nextTileType = tileType;
    }

    public TileTypes GetNextTileType()
    {
        return nextTileType;
    }

    public void AddTile(Vector2Int location, Tile t)
    {
        adjacentTiles[location.x, location.y] = t;
    }

    public void CheckNextType(){
        nextTileType = TileRules.GetNewTileType(currentTileType,adjacentTiles);
    }

    public void NextTurn(){
        currentTileType = nextTileType;
        CheckNextType();
    }

    public Tile[,] GetAdjacentTiles()
    {
        return adjacentTiles;
    }
}
