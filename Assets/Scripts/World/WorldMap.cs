using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class WorldMap : MonoBehaviour
{
    public enum CELL_TYPE
    {
        PLAIN,
        NUM
    }
    public GameObject[] cellPrefabs = new GameObject[(int)CELL_TYPE.NUM];
    public int mapMaxX = 50;
    public int mapMaxY = 50;

    public Tilemap worldmap;
    TileBase tile;

    void Start()
    {
        worldmap = GameObject.Find("Worldmap").GetComponent<Tilemap>();

        Vector3Int tilePos = new Vector3Int(0,0,0);
        if (worldmap.HasTile(tilePos)) {
            worldmap.SetTile(tilePos, tile);
        } 

        for (int i = 0; i < mapMaxY; i++)
        {
            for (int j = 0; j < mapMaxX; j++)
            {
                /*
                GameObject instance = Instantiate(cellPrefabs[0]);
                instance.transform.SetParent(transform);
                instance.transform.position = new Vector3(0.75f * j, 0, i + 0.5f * (j % 2));
                */

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
