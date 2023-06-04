using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// É^ÉCÉãè„ÇÃç¿ïW.
public class VectorOnTile
{
    public VectorOnTile(int inputX, int inputY)
    {
        Set(inputX, inputY);
    }
    public VectorOnTile(Vector3Int input)
    {
        ConvertFromVector3Int(input);
    }
    public void ConvertFromVector3Int(Vector3Int input)
    {
        Set(input.x, input.y);
    }
    public Vector3Int ConvertToVector3Int()
    {
        return new Vector3Int(x, y);
    }
    public void Set(int inputX, int inputY)
    {
        x = inputX;
        y = inputY;
    }


    public float GetMagnitude(VectorOnTile anotherTile)
    {
        Vector3 thisWorldPos = Application.worldMapManager.GetWorldPosFromTilePos(this);
        Vector3 anotherWorldPos = Application.worldMapManager.GetWorldPosFromTilePos(anotherTile);
        Vector3 diffVector = thisWorldPos - anotherWorldPos;

        return diffVector.magnitude;
    }


    public bool IsEqual(VectorOnTile anotherTile)
    {
        return (x == anotherTile.x) && (y == anotherTile.y);
    }

    public int x;
    public int y;
}
