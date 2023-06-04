using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAStarTileInfo
{
    public VectorOnTile pos;
    public float unfixedCost;
    public float heuristicsCost;

    public AIAStarTileInfo(VectorOnTile input_pos, float input_unfixedCost, float input_heuristicsCost)
    {
        Set(input_pos, input_unfixedCost, input_heuristicsCost);
    }

    void Set(VectorOnTile input_pos, float input_unfixedCost, float input_heuristicsCost)
    {
        pos = input_pos;
        unfixedCost = input_unfixedCost;
        heuristicsCost = input_heuristicsCost;
    }

    public float GetScore()
    {
        return unfixedCost + heuristicsCost;
    }

    public void SetUnfixedcost(float input_unfixedCost)
    {
        unfixedCost = input_unfixedCost;
    }


}
