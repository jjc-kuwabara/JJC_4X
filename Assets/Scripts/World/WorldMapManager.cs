using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class WorldMapManager : MonoBehaviour
{
    public List<TileBase> tileList;

    public Tilemap worldmap;
    public int mapMaxX = 50;
    public int mapMaxY = 50;

    // Start is called before the first frame update
    void Start()
    {
        worldmap = GameObject.Find("Worldmap").GetComponent<Tilemap>();
        InitTile();

        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int tilePos = GetTilePos(x_index, y_index);

                if (worldmap.HasTile(tilePos))
                {
                    int tileIndex = x_index % 4;
                    worldmap.SetTile(tilePos, tileList[tileIndex]);
                }
            }
        }
    }

    void InitTile()
    {
        for (int y_index = 0; y_index < mapMaxY; y_index++)
        {
            for (int x_index = 0; x_index < mapMaxX; x_index++)
            {
                Vector3Int tilePos = GetTilePos(x_index, y_index);

                worldmap.SetTile(tilePos, tileList[0]);
            }
        }
    }

    Vector3Int GetTilePos(int strategyX, int strategyY)
    {
        return new Vector3Int(strategyY, strategyX, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
