using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCell : MonoBehaviour
{
    public WorldMap.CELL_TYPE cellType;

    public bool isEnableHuti;
    GameObject cellObj;
    GameObject hutiObj;

    void Start()
    {
        cellObj = transform.Find("CellRender").gameObject;
        hutiObj = transform.Find("HutiRender").gameObject;
        isEnableHuti = false;

        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableHuti( bool isEnable )
    {
        isEnableHuti = isEnable;
        Refresh();
    }

    private void Refresh()
    {
        hutiObj.SetActive(isEnableHuti);
    }
}
