using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMoveDestination(VectorOnTile tilePos)
    {
        Vector3 worldDestPos = Application.worldMapManager.GetWorldPosFromTilePos(tilePos);
        transform.position = worldDestPos;
    }
}
