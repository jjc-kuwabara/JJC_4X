using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;
using static AICalcMap;

public class WorldMapManager : MonoBehaviour
{
    public enum MAP_TYPE
    {
        WORLD,
        OVERWRITE,
        NUM
    };
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

    public enum CELL_CONNECT {
        TOP,
        LEFT_TOP,
        LEFT_BOTTOM,
        BOTTOM,
        RIGHT_BOTTOM,
        RIGHT_TOP,
        NUM
    };
    public List<TileBase> worldTileList;
    public List<TileBase> overWriteTileList;

    public Tilemap worldMap;
    public Tilemap overWriteMap;
    public int mapMaxX = 50;
    public int mapMaxY = 50;
    AICalcMap aiCalcMap;

    // Start is called before the first frame update
    void Start()
    {
        worldMap = GameObject.Find("WorldMap").GetComponent<Tilemap>();
        overWriteMap = GameObject.Find("OverWriteMap").GetComponent<Tilemap>();
        aiCalcMap = new AICalcMap();
        aiCalcMap.Initialize();
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

//                overWriteMap.SetTile(tilePos, overWriteTileList[1]);
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



    /*
     * OverWriteMapを、特定範囲の間だけ書き込む.
     */
    public void SetRangeOverWrite(OVERWRITE_TILE tileId, Vector3 worldPos, int range)
    {
        Vector3Int tilePos;
        tilePos = overWriteMap.WorldToCell(worldPos);

        Application.debugUIManager.ClearDebugUI();

        // 計算用のMapを書き込む.
        aiCalcMap.ClearCalcMap();
        aiCalcMap.SetRange(tilePos, range);

        // 計算Mapの値をみて、実際にTilemapを書き換える.
        int nTileId = (int)tileId;
        TileBase selectedTile = overWriteTileList[nTileId];
        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int checkPos = new Vector3Int(x_index, y_index);
                int value = aiCalcMap.GetMapValue(CALC_TYPE.MOVE, checkPos);
                if (value != AICalcMap.INVALID)
                {
                    overWriteMap.SetTile(checkPos, selectedTile);

                    Application.debugUIManager.SetText(GetWorldPosFromTilePos(checkPos), value.ToString());
                }
            }
        }
    }

    public Vector3Int GetConnectTilePos(Vector3Int originalTilePos, CELL_CONNECT connectType)
    {
        Vector3Int ret = Vector3Int.zero;

        switch(connectType){
            case CELL_CONNECT.TOP:
                ret = new Vector3Int(originalTilePos.x + 1, originalTilePos.y, originalTilePos.z);
                break;
            case CELL_CONNECT.BOTTOM:
                ret = new Vector3Int(originalTilePos.x - 1, originalTilePos.y, originalTilePos.z);
                break;
            case CELL_CONNECT.RIGHT_TOP:
                if (originalTilePos.y % 2 == 0)
                {
                    ret = new Vector3Int(originalTilePos.x, originalTilePos.y + 1, originalTilePos.z);
                }
                else
                {
                    ret = new Vector3Int(originalTilePos.x + 1, originalTilePos.y + 1, originalTilePos.z);
                }
                break;
            case CELL_CONNECT.RIGHT_BOTTOM:
                if (originalTilePos.y % 2 == 0)
                {
                    ret = new Vector3Int(originalTilePos.x - 1, originalTilePos.y + 1, originalTilePos.z);
                }
                else
                {
                    ret = new Vector3Int(originalTilePos.x, originalTilePos.y + 1, originalTilePos.z);
                }
                break;
            case CELL_CONNECT.LEFT_TOP:
                if (originalTilePos.y % 2 == 0)
                {
                    ret = new Vector3Int(originalTilePos.x, originalTilePos.y - 1, originalTilePos.z);
                }
                else
                {
                    ret = new Vector3Int(originalTilePos.x + 1, originalTilePos.y - 1, originalTilePos.z);
                }
                break;
            case CELL_CONNECT.LEFT_BOTTOM:
                if (originalTilePos.y % 2 == 0)
                {
                    ret = new Vector3Int(originalTilePos.x - 1, originalTilePos.y - 1, originalTilePos.z);
                }
                else
                {
                    ret = new Vector3Int(originalTilePos.x, originalTilePos.y - 1, originalTilePos.z);
                }
                break;
        }

        return ret;
    }

    public Vector3 GetWorldPosFromTilePos(Vector3Int tilePos)
    {
        return overWriteMap.GetCellCenterWorld(tilePos);
    }
}
