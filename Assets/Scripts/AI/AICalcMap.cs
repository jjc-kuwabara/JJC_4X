using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICalcMap
{
    int[][] calcMap;    // �v�Z�̂��тɏ���������}�b�v.
    int[][] fixMap;     // ����̃C�x���g���N�������̂ݍČv�Z����}�b�v.
    List<AIAStarTileInfo> nextCalcTileList;

    public enum CALC_TYPE
    {
        MOVE,
        MOVE_RESULT,
        ASTER_FIXED_COST,
        ASTER_HEURISTICS_COST, 
        ASTER_RESULT_ROUTE,
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

        nextCalcTileList = new List<AIAStarTileInfo>();
    }
    public void SetRange(VectorOnTile tilePos, int range)
    {
        // �w����W���͈͓��ɂ��邩�ǂ���.
        if (!Application.worldMapManager.IsValid(tilePos))
        {
            return;
        }

        // �w����W�Ɋ��ɒl�������Ă����ꍇ�ɁA���̒l�ƁA���������悤�Ƃ��Ă���l�Ƃ��r����.
        int currentValue = GetMapValue(CALC_TYPE.MOVE, tilePos);
        if (currentValue != INVALID)
        {
            if (currentValue >= range)
            {
                return;
            }
        }
        // �w����W�ɒl�����.
        SetMapValue(CALC_TYPE.MOVE, tilePos, range);

        // ���͒l���A���̒l�̌��������錳�C���c���Ă��邩�ǂ���.
        if (range <= 0)
        {
            return;
        }

        // ���̎���.
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
            // �������ׂ��Z���̌���.
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
            // ��������Z�������肵���ꍇ�A���̏ꏊ�̃R�X�g�͊m�肵��.
            SetMapValue(CALC_TYPE.ASTER_FIXED_COST, calcTileInfo.pos, (int)calcTileInfo.unfixedCost);
            // ���̃Z���͒������ׂ��Z�����珜�O����.
            nextCalcTileList.RemoveAt(minIndex);

            // ���ɒ������ׂ��Z�������肷��.
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
                            // ���łɌv�Z�ς݂̖������R�X�g���A���������悤�Ƃ��Ă���R�X�g�l�����Ⴂ�Ȃ�A���̂܂�.
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
                SetMapValue(CALC_TYPE.ASTER_HEURISTICS_COST, connectTilePos, (int)(huristicsCost * 1000.0f)); // �������.


                if (goalPos.IsEqual(connectTilePos))
                {
                    // �S�[���ɂ����I.
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

        // �S�[���ɂ��Ă�����A���ۂ̃��[�g���v�Z����.
        
        if (isReachGoal)
        {
            bool isReachStart = false;
            VectorOnTile currentCheckPos = goalPos;
            SetMapValue(CALC_TYPE.ASTER_RESULT_ROUTE, goalPos, GetMapValue(CALC_TYPE.ASTER_FIXED_COST, goalPos));
            VectorOnTile minRoutePos = new VectorOnTile(0, 0);
            loopCount = 0;
            while (!isReachStart)
            {
                minScore = MAX_COST;
                int checkFixedCost = 0;
                // ���ɒ������ׂ��Z�������肷��.
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

                    if (minScore > checkFixedCost)
                    {
                        minScore = checkFixedCost;
                        minRoutePos = connectTilePos;
                    }
                }

                currentCheckPos = minRoutePos;
                SetMapValue(CALC_TYPE.ASTER_RESULT_ROUTE, currentCheckPos, checkFixedCost);

                if (startPos.IsEqual(currentCheckPos))
                {
                    // �S�[���ɂ����I.
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
