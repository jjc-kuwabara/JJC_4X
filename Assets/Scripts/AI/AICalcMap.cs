using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICalcMap
{
    int[][] calcMap;    // 計算のたびに初期化するマップ.
    int[][] fixMap;     // 特定のイベントが起きた時のみ再計算するマップ.
    List<AIAStarTileInfo> nextCalcTileList;

    public enum CALC_TYPE
    {
        MOVE,
        MOVE_RESULT,
        ASTER_FIXED_COST,
        ASTER_HEURISTICS_COST, 
        ASTER_RESULT_ROUTE,
        DEBUG_DIST_FROM_CLICK,
        NUM
    }

    public enum FIX_TYPE
    {
        MOVE_COST,
        NUM
    }

    public const int INVALID = -1;
    public const float MAX_COST = 999999.0f;

    public void Initialize()
    {
        calcMap = new int[(int)CALC_TYPE.NUM][];

        for (int calcTypeIndex = 0; calcTypeIndex < (int)CALC_TYPE.NUM; calcTypeIndex++)
        {
            calcMap[calcTypeIndex] = new int[Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX];
        }

        ClearCalcMap();

        fixMap = new int[(int)FIX_TYPE.NUM][];

        for (int typeIndex = 0; typeIndex < (int)FIX_TYPE.NUM; typeIndex++)
        {
            fixMap[typeIndex] = new int[Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX];
        }

        RefreshMoveCostMap();
        RefreshDistFromClick(new VectorOnTile(0, 0));

        nextCalcTileList = new List<AIAStarTileInfo>();
    }
    public void SetRange(VectorOnTile tilePos, int range)
    {
        // 指定座標が範囲内にあるかどうか.
        if (!Application.worldMapManager.IsValid(tilePos))
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
            VectorOnTile calcingTilePos = Application.worldMapManager.GetConnectTilePos(tilePos, i);
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

    public void RefreshMoveCostMap()
    {
        for (int mapIndex = 0; mapIndex < Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX; mapIndex++)
        {
            fixMap[(int)FIX_TYPE.MOVE_COST][mapIndex] = 1;
        }
    }

    public void DEBUG_RefreshDistFromClick(VectorOnTile mouseClickPos)
    {
        // デバッグ用に、正しいコスト値を表示.
        for (int mapIndex = 0; mapIndex < Application.worldMapManager.mapMaxY * Application.worldMapManager.mapMaxX; mapIndex++)
        {
            VectorOnTile tilePos = new VectorOnTile(mapIndex % Application.worldMapManager.mapMaxX, mapIndex / Application.worldMapManager.mapMaxX);
            float huristicsCost = mouseClickPos.GetMagnitude(tilePos);

            calcMap[(int)CALC_TYPE.DEBUG_DIST_FROM_CLICK][mapIndex] = (int)(huristicsCost * 1000.0f);
        }
    }

    public void SetMapValue(CALC_TYPE calcType, VectorOnTile tilePos, int value)
    {
        int mapIndex = tilePos.y * Application.worldMapManager.mapMaxX + tilePos.x;
        calcMap[(int)calcType][mapIndex] = value;
    }

    public int GetMapValue(CALC_TYPE calcType, VectorOnTile tilePos)
    {
        int mapIndex = tilePos.y * Application.worldMapManager.mapMaxX + tilePos.x;
        return calcMap[(int)calcType][mapIndex];
    }

    public int GetFixMapValue(FIX_TYPE fixTyle, VectorOnTile tilePos)
    {
        int mapIndex = tilePos.y * Application.worldMapManager.mapMaxX + tilePos.x;
        return fixMap[(int)fixTyle][mapIndex];
    }

    public void CalcRouteByAStar(VectorOnTile startPos, VectorOnTile goalPos)
    {
        ClearCalcMap();
        AIAStarTileInfo startTileInfo = new AIAStarTileInfo(startPos, 0.0f, 0.0f);
        nextCalcTileList.Clear();
        nextCalcTileList.Add(startTileInfo);

        bool isReachGoal = false;
        int loopCount = 0;
        float cost = 0.0f;
        float minScore = MAX_COST;
        float minHuristicCost = MAX_COST;
        int minIndex = INVALID;

        while (!isReachGoal)
        {
            // 調査すべきセルの決定.
            minScore = MAX_COST;
            minHuristicCost = MAX_COST;
            for (int listIndex = 0; listIndex < nextCalcTileList.Count; listIndex++)
            {
                AIAStarTileInfo info = nextCalcTileList[listIndex];
                if (minScore > info.GetScore())
                {
                    minScore = info.GetScore();
                    minIndex = listIndex;
                }else if (minScore == info.GetScore())
                {
                    if (minHuristicCost > info.heuristicsCost)
                    {
                        minScore = info.GetScore();
                        minHuristicCost = info.heuristicsCost;
                        minIndex = listIndex;
                    }
                }
            }

            if(minIndex == INVALID)
            {
                Debug.Assert(false);
                break;
            }

            AIAStarTileInfo calcTileInfo = nextCalcTileList[minIndex];
            Debug.Log(calcTileInfo.pos.x.ToString() + ", " + calcTileInfo.pos.y.ToString() + ", " + calcTileInfo.GetScore().ToString() );
            // 調査するセルが決定した場合、その場所のコストは確定した.
            SetMapValue(CALC_TYPE.ASTER_FIXED_COST, calcTileInfo.pos, (int)calcTileInfo.unfixedCost);
            // そのセルは調査すべきセルから除外する.
            nextCalcTileList.RemoveAt(minIndex);

            // 次に調査すべきセルを決定する.
            for (WorldMapManager.CELL_CONNECT i = WorldMapManager.CELL_CONNECT.TOP; i < WorldMapManager.CELL_CONNECT.NUM; i++)
            {
                VectorOnTile connectTilePos = Application.worldMapManager.GetConnectTilePos(calcTileInfo.pos, i);
                if (!Application.worldMapManager.IsValid(connectTilePos))
                {
                    continue;
                }
                if (GetMapValue(CALC_TYPE.ASTER_FIXED_COST, connectTilePos) != INVALID)
                {
                    continue;
                }


                int connectTileMoveCost = GetFixMapValue(FIX_TYPE.MOVE_COST, connectTilePos);
                float unfixedCost = connectTileMoveCost + calcTileInfo.unfixedCost;
                if (GetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, connectTilePos) != INVALID)
                {
                    for (int huristicsCheckListIndex = 0; huristicsCheckListIndex < nextCalcTileList.Count; huristicsCheckListIndex++)
                    {
                        AIAStarTileInfo info = nextCalcTileList[huristicsCheckListIndex];
                        if (info.pos.IsEqual(connectTilePos))
                        {
                            // すでに計算済みの未完了コストが、今から入れようとしているコスト値よりも低いなら、そのまま.
                            if( info.unfixedCost < unfixedCost)
                            {
                                continue;
                            }
                        }
                    }

                    continue;
                }

                float huristicsCost = goalPos.GetMagnitude(connectTilePos);
                AIAStarTileInfo newCalcTile = new AIAStarTileInfo(connectTilePos, unfixedCost, huristicsCost);
                nextCalcTileList.Add(newCalcTile);
                SetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, connectTilePos, (int)(huristicsCost * 1000.0f)); // 調査候補.


                if (goalPos.IsEqual(connectTilePos))
                {
                    // ゴールについた！.
                    isReachGoal = true;
                    SetMapValue(CALC_TYPE.ASTER_FIXED_COST, connectTilePos, (int)unfixedCost);
                    break;
                }
            }
            if (isReachGoal)
            {
                break;
            }

            loopCount++;
            if (loopCount > 10000)
            {
                break;
            }

            if (loopCount > Application.worldMapManager.debugASterLoopMax)
            {
                break;
            }
        }
        Debug.Log(loopCount);

        // ゴールについていたら、実際のルートを計算する.
        
        if (isReachGoal)
        {
            bool isReachStart = false;
            VectorOnTile currentCheckPos = goalPos;
            SetMapValue(CALC_TYPE.ASTER_RESULT_ROUTE, goalPos, 0);
            VectorOnTile minRoutePos = new VectorOnTile(0, 0);
            loopCount = 1;
            while (!isReachStart)
            {
                minScore = MAX_COST;
                minHuristicCost = MAX_COST;
                int checkFixedCost = 0;
                int heuristicsCost = 0;
                bool isFound = false;
                // 次に調査すべきセルを決定する.
                for (WorldMapManager.CELL_CONNECT i = WorldMapManager.CELL_CONNECT.TOP; i < WorldMapManager.CELL_CONNECT.NUM; i++)
                {
                    VectorOnTile connectTilePos = Application.worldMapManager.GetConnectTilePos(currentCheckPos, i);
                    if (!Application.worldMapManager.IsValid(connectTilePos))
                    {
                        continue;
                    }
                    checkFixedCost = GetMapValue(CALC_TYPE.ASTER_FIXED_COST, connectTilePos);
                    if (checkFixedCost == INVALID)
                    {
                        continue;
                    }

                    if (minScore >= checkFixedCost) {
                        heuristicsCost = GetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, connectTilePos);

                        if (minScore > checkFixedCost)
                        {
                            minScore = checkFixedCost;
                            minHuristicCost = heuristicsCost;
                            minRoutePos = connectTilePos;
                            isFound = true;
                        }
                        else if (minScore == checkFixedCost)
                        {
                            if (minHuristicCost > heuristicsCost)
                            {
                                minScore = checkFixedCost;
                                minHuristicCost = heuristicsCost;
                                minRoutePos = connectTilePos;
                                isFound = true;
                            }
                        }
                    }


                }

                if (!isFound)
                {
                    // たどるべきルートが無かった.
                    Debug.Assert(false);
                }

                currentCheckPos = minRoutePos;
                int cellNumFromGoal = loopCount;
                SetMapValue(CALC_TYPE.ASTER_RESULT_ROUTE, currentCheckPos, cellNumFromGoal);

                if (startPos.IsEqual(currentCheckPos))
                {
                    // ゴールについた！.
                    isReachStart = true;
                    break;
                }
                loopCount++;
                if (loopCount > 10000)
                {
                    break;
                }

            }
        }
        
    }
}
