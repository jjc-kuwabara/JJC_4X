using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class MouseManager : MonoBehaviour
{
    public Vector3 selectedPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
//        Application.worldMapManager.SetRangeOverWrite(WorldMapManager.OVERWRITE_TILE.RED, selectedPos);

        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedGameObject = null; // マウスでクリックしたゲームオブジェクト.

            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    clickedGameObject = hit.collider.gameObject;


                    Application.worldMapManager.SetRangeOverWrite(WorldMapManager.OVERWRITE_TILE.RED, hit.point, 8);
                }
            }
        }
    }
}
