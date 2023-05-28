using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DebugUIManager : MonoBehaviour
{

    public GameObject debugTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(Vector3 worldPos, string text)
    {
        GameObject instance = Instantiate(debugTextPrefab);
        instance.transform.parent = this.transform;
        instance.transform.position = Camera.main.WorldToScreenPoint(worldPos);
        instance.GetComponent<Text>().text = text;
    }

    public void ClearDebugUI()
    {
        int childNum = transform.childCount;
        for (int i = 0; i < childNum; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
