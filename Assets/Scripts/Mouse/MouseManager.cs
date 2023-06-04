using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class MouseManager : MonoBehaviour
{
    public Vector3 selectedPos;
    bool isMiddleClickHold;
    Vector3 middleClickedWorldPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseOnWorldMap();
    }

    void UpdateMouseOnWorldMap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject clickedGameObject = null; // �}�E�X�ŃN���b�N�����Q�[���I�u�W�F�N�g.

            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();

                if (Physics.Raycast(ray, out hit))
                {
                    clickedGameObject = hit.collider.gameObject;


                    LeftClickOnWorldMap(hit.point);
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("Middle Click");
            isMiddleClickHold = true;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                middleClickedWorldPos = hit.point;
            }
        }

        if (isMiddleClickHold)
        {
            Vector3 currentMouseWorldPos = Vector3.zero;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                currentMouseWorldPos = hit.point;
                Vector3 diffPos = currentMouseWorldPos - middleClickedWorldPos;

                Application.worldMapManager.MoveTilemap(diffPos);

                middleClickedWorldPos = currentMouseWorldPos;
            }




            if (Input.GetMouseButtonUp(2))
            {
                isMiddleClickHold = false;
            }
        }
    }

    void LeftClickOnWorldMap(Vector3 clickerWorldPos)
    {
        //Application.worldMapManager.SetRangeOverWrite(WorldMapManager.OVERWRITE_TILE.RED, clickerWorldPos, 8);

        // �L�����N�^�[�̈ړ�.
        /*
         * �L�����N�^�[���}�E�X�N���b�N���đI������.
         * �ړ���̍��W���N���b�N���āA�ړ�������߂�.
         * �L�����N�^�[���ړ�����ԁA���삪�����Ȃ��悤�ɂ���.
         */
        VectorOnTile tilePos = Application.worldMapManager.GetTilePosFromWorldPos(clickerWorldPos);
        if (Application.worldMapManager.IsValid(tilePos))
        {
            GameObject.Find("Chara").GetComponent<CharacterAI>().SetMoveDestination(tilePos);

            VectorOnTile goalPos = new VectorOnTile(0, 0);
            Application.worldMapManager.CalcRouteByAStar(tilePos, goalPos);
        }
    }
}
