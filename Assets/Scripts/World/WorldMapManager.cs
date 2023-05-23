using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class WorldMapManager : MonoBehaviour
{
    public enum WORLD_TILE
    {
        PLAIN,
        FOREST,
        RIVER,
        ROCK
    };
    public enum OVERWRITE_TILE
    {
        RED,
        BLUE,
        GREEN,
        YELLOW
    };
    public List<TileBase> worldTileList;
    public List<TileBase> overWriteTileList;

    public Tilemap worldMap;
    public Tilemap overWriteMap;
    public int mapMaxX = 50;
    public int mapMaxY = 50;

    // Start is called before the first frame update
    void Start()
    {
        worldMap = GameObject.Find("WorldMap").GetComponent<Tilemap>();
        overWriteMap = GameObject.Find("OverWriteMap").GetComponent<Tilemap>();
        InitTile();

        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int tilePos = GetTilePos(x_index, y_index);

                if (worldMap.HasTile(tilePos))
                {
                    int tileIndex = x_index % 4;
                    worldMap.SetTile(tilePos, worldTileList[tileIndex]);
                }
            }
        }
    }

    void InitTile()
    {
        worldTileList.Add(Resources.Load<TileBase>("Tile_World/Plain"));
        worldTileList.Add(Resources.Load<TileBase>("Tile_World/Forest"));
        worldTileList.Add(Resources.Load<TileBase>("Tile_World/River"));
        worldTileList.Add(Resources.Load<TileBase>("Tile_World/Rock"));

        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int tilePos = GetTilePos(x_index, y_index);

                worldMap.SetTile(tilePos, worldTileList[0]);
            }
        }

        overWriteTileList.Add(Resources.Load<TileBase>("Tile_OverWrite/OverWriteRed"));
        overWriteTileList.Add(Resources.Load<TileBase>("Tile_OverWrite/OverWriteBlue"));
        overWriteTileList.Add(Resources.Load<TileBase>("Tile_OverWrite/OverWriteGreen"));
        overWriteTileList.Add(Resources.Load<TileBase>("Tile_OverWrite/OverWriteYellow"));

        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int tilePos = GetTilePos(x_index, y_index);

                overWriteMap.SetTile(tilePos, overWriteTileList[1]);
            }
        }

        //overWriteMap.ClearAllTiles();
    }

    Vector3Int GetTilePos(int strategyX, int strategyY)
    {
        return new Vector3Int(strategyY, strategyX, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRangeOverWrite(OVERWRITE_TILE tileId, Vector3 worldPos)
    {
        Vector3Int tilePos;
        tilePos = overWriteMap.WorldToCell(worldPos);
        overWriteMap.SetTile(tilePos, overWriteTileList[(int)tileId]);

        // Žü•Ó1ƒ}ƒX•ª.
    }
}
