using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : MonoBehaviour
{
    static BuildManager buildManagerInstance;
    public bool realMode = false; // real, testにより異なるDBに接近
    public string buildMode = "window"; // build対象(window, android)により異なるdataPath設定
    public string version = "1.0.0"; // gameVersion

    private void Awake()
    {
        Debug.Log("realMode: " + realMode);
        Debug.Log("buildMode: " + buildMode);
        Debug.Log("version: " + version);

        if (buildManagerInstance != null)
        {
            Destroy(this.gameObject);
        }else if (buildManagerInstance == null)
        {
            buildManagerInstance = this;
            DontDestroyOnLoad(buildManagerInstance);
        }
    }
}
