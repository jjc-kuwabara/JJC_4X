using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TreeEditor.TreeEditorHelper;

public class MouseManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            GameObject clickedGameObject = null; // �}�E�X�ŃN���b�N�����Q�[���I�u�W�F�N�g.

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                clickedGameObject = hit.collider.gameObject;

                WorldCell worldCellComponent = clickedGameObject.GetComponent<WorldCell>();
                if (worldCellComponent != null)
                {
                    worldCellComponent.EnableHuti(true);
                }
            }
        }
    }
}
