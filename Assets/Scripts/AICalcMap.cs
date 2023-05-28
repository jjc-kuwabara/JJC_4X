using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICalcMap
{
    int[][] calcMap;

    public enum CALC_TYPE
    {
        MOVE,
        NUM
    }

    public const int INVALID = -1;

    public void Initialize()
    {
        calcMap = new int[(int)CALC_TYPE.NUM][];

        for (int calcTypeIndex = 0; calcTypeIndex < (int)CALC_TYPE.NUM; calcTypeIndex++)
        {
            calcMap[calcTypeIndex] = new int[Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX];
        }

        ClearCalcMap();
    }
    public void SetRange(Vector3Int tilePos, int range)
    {
        // 指定座標が範囲内にあるかどうか.
        if (tilePos.x < 0 || tilePos.x >= Application.worldMapManager.mapMaxX)
        {
            return;
        }
        if (tilePos.y < 0 || tilePos.y >= Application.worldMapManager.mapMaxY)
        {
            return;
        }
        // 指定座標に既に値が入っていた場合に、その値と、今から入れようとしている値とを比較する.
        int currentValue = GetMapValue(CALC_TYPE.MOVE, tilePos);
        if (currentValue != INVALID)
        {
            if (currentValue >= range)
            {
                return;
            }
        }
        // 指定座標に値を入力.
        SetMapValue(CALC_TYPE.MOVE, tilePos, range);

        // 入力値が、次の値の検索をする元気を残しているかどうか.
        if (range <= 0)
        {
            return;
        }

        // その周囲.
        for (WorldMapManager.CELL_CONNECT i = WorldMapManager.CELL_CONNECT.TOP; i < WorldMapManager.CELL_CONNECT.NUM; i++)
        {
            Vector3Int calcingTilePos = Application.worldMapManager.GetConnectTilePos(tilePos, i);
            SetRange(calcingTilePos, range - 1);
        }
    }

    public void ClearCalcMap()
    {
        for (int calcTypeIndex = 0; calcTypeIndex < (int)CALC_TYPE.NUM; calcTypeIndex++)
        {
            for (int mapIndex = 0; mapIndex < Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX; mapIndex++)
            {
                calcMap[calcTypeIndex][mapIndex] = INVALID;
            }
        }
    }

    public void SetMapValue(CALC_TYPE calcType, Vector3Int tilePos, int value)
    {
        int mapIndex = tilePos.y * Application.worldMapManager.mapMaxX + tilePos.x;
        calcMap[(int)calcType][mapIndex] = value;
    }

    public int GetMapValue(CALC_TYPE calcType, Vector3Int tilePos)
    {
        int mapIndex = tilePos.y * Application.worldMapManager.mapMaxX + tilePos.x;
        return calcMap[(int)calcType][mapIndex];
    }
}
