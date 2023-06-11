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
    public int mapMinX = 0;
    public int mapMinY = 0;
    public int mapMaxX = 50;
    public int mapMaxY = 50;
    AICalcMap aiCalcMap;
    public int debugASterLoopMax = 1000;

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
        VectorOnTile tilePos = GetTilePosFromWorldPos(worldPos);

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
                VectorOnTile checkPos = new VectorOnTile(x_index, y_index);
                int value = aiCalcMap.GetMapValue(CALC_TYPE.MOVE, checkPos);
                if (value != AICalcMap.INVALID)
                {
                    overWriteMap.SetTile(checkPos.ConvertToVector3Int(), selectedTile);

                    Application.debugUIManager.SetText(GetWorldPosFromTilePos(checkPos), value.ToString());
                }
            }
        }
    }

    public VectorOnTile GetConnectTilePos(VectorOnTile originalTilePos, CELL_CONNECT connectType)
    {
        VectorOnTile ret = new VectorOnTile(0, 0);

        switch(connectType){
            case CELL_CONNECT.TOP:
                ret.Set(originalTilePos.x + 1, originalTilePos.y);
                break;
            case CELL_CONNECT.BOTTOM:
                ret.Set(originalTilePos.x - 1, originalTilePos.y);
                break;
            case CELL_CONNECT.RIGHT_TOP:
                if (originalTilePos.y % 2 == 0)
                {
                    ret.Set(originalTilePos.x, originalTilePos.y + 1);
                }
                else
                {
                    ret.Set(originalTilePos.x + 1, originalTilePos.y + 1);
                }
                break;
            case CELL_CONNECT.RIGHT_BOTTOM:
                if (originalTilePos.y % 2 == 0)
                {
                    ret.Set(originalTilePos.x - 1, originalTilePos.y + 1);
                }
                else
                {
                    ret.Set(originalTilePos.x, originalTilePos.y + 1);
                }
                break;
            case CELL_CONNECT.LEFT_TOP:
                if (originalTilePos.y % 2 == 0)
                {
                    ret.Set(originalTilePos.x, originalTilePos.y - 1);
                }
                else
                {
                    ret.Set(originalTilePos.x + 1, originalTilePos.y - 1);
                }
                break;
            case CELL_CONNECT.LEFT_BOTTOM:
                if (originalTilePos.y % 2 == 0)
                {
                    ret.Set(originalTilePos.x - 1, originalTilePos.y - 1);
                }
                else
                {
                    ret.Set(originalTilePos.x, originalTilePos.y - 1);
                }
                break;
        }

        return ret;
    }

    public VectorOnTile GetTilePosFromWorldPos(Vector3 worldPos)
    {
        VectorOnTile tilePos = new VectorOnTile(overWriteMap.WorldToCell(worldPos));
        return tilePos;
    }

    public Vector3 GetWorldPosFromTilePos(VectorOnTile tilePos)
    {
        Vector3Int _tilePos = new Vector3Int(tilePos.x, tilePos.y);
        return overWriteMap.GetCellCenterWorld(_tilePos);
    }

    public bool IsValid(VectorOnTile tilePos)
    {
        if (mapMinX <= tilePos.x && tilePos.x <= mapMaxX)
        {
            if (mapMinY <= tilePos.y && tilePos.y <= mapMaxY)
            {
                return true;
            }
        }
        return false;
    }

    public void MoveTilemap(Vector3 diffVector)
    {
        Transform tilemapParent = worldMap.transform.parent;
        tilemapParent.position += diffVector;
    }

    public void CalcRouteByAStar(VectorOnTile startPos, VectorOnTile goalPos)
    {
        Application.debugUIManager.ClearDebugUI();

        aiCalcMap.CalcRouteByAStar(startPos, goalPos);

        // デバッグ用にマップ表示をさせる.
        overWriteMap.ClearAllTiles();
        TileBase selectedTile = overWriteTileList[0];
        int value = 0;
        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                VectorOnTile checkPos = new VectorOnTile(x_index, y_index);
                value = aiCalcMap.GetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, checkPos);
                if (value != AICalcMap.INVALID)
                {
                    selectedTile = overWriteTileList[(int)OVERWRITE_TILE.GREEN];
                    overWriteMap.SetTile(checkPos.ConvertToVector3Int(), selectedTile);
                    if (debugASterLoopMax % 2 == 0) {
                        int calcedHeuristicsCost = aiCalcMap.GetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, checkPos);
                        int fixedHeuristicsCost = aiCalcMap.GetFixMapValue(FIX_TYPE.GOAL_DIST, checkPos);
                        value = fixedHeuristicsCost;
                        Application.debugUIManager.SetText(GetWorldPosFromTilePos(checkPos), value.ToString());
                    }
                }


                value = aiCalcMap.GetMapValue(CALC_TYPE.ASTER_FIXED_COST, checkPos);
                if (value != AICalcMap.INVALID)
                {
                    selectedTile = overWriteTileList[(int)OVERWRITE_TILE.BLUE];
                    overWriteMap.SetTile(checkPos.ConvertToVector3Int(), selectedTile);
                    if (debugASterLoopMax % 2 == 1)
                    {
                        //Application.debugUIManager.SetText(GetWorldPosFromTilePos(checkPos), value.ToString());
                    }
                }

                value = aiCalcMap.GetMapValue(CALC_TYPE.ASTER_RESULT_ROUTE, checkPos);
                if (value != AICalcMap.INVALID)
                {
                    selectedTile = overWriteTileList[(int)OVERWRITE_TILE.RED];
                    overWriteMap.SetTile(checkPos.ConvertToVector3Int(), selectedTile);

                    if (debugASterLoopMax % 2 == 1)
                    {
                        Application.debugUIManager.SetText(GetWorldPosFromTilePos(checkPos), value.ToString());
                    }
                }
            }
        }

        debugASterLoopMax++;
    }
}
