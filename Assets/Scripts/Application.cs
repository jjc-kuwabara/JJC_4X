using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Application : MonoBehaviour
{
    private void Awake()
    {
        S = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ---------------- Static Section ---------------- //

    static private Application _S;
    static private Application S
    {
        get
        {
            if (_S == null)
            {
                return null;
            }
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("_SÇÕä˘Ç…ê›íËÇ≥ÇÍÇƒÇ¢Ç‹Ç∑.");
            }
            _S = value;
        }
    }

    private WorldMapManager _worldMapManager;
    static public WorldMapManager worldMapManager
    {
        get
        {
            if (S != null)
            {
                if (S._worldMapManager == null)
                {
                    S._worldMapManager = GameObject.Find("WorldMapManager").GetComponent<WorldMapManager>();
                }
                return S._worldMapManager;
            }
            return null;
        }
    }

    private DebugUIManager _debugUIManager;
    static public DebugUIManager debugUIManager
    {
        get
        {
            if (S != null)
            {
                if (S._debugUIManager == null)
                {
                    S._debugUIManager = GameObject.Find("DebugUIManager").GetComponent<DebugUIManager>();
                }
                return S._debugUIManager;
            }
            return null;
        }
    }

}
