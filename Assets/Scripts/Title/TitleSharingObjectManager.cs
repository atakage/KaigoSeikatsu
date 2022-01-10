using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject versionTextValueGameObj;

    // Start is called before the first frame update
    // Awake() -> Start()
    /*
     * 
     * 1. object disabled, script disabled
     * Awake() X
     * Start() X
     * 
     * 2. object enable, script disabled
     * Awake() O
     * Start() X
     * 
     * 3. object enable, script enable
     * Awake() O
     * Start() O
     */
    void Awake()
    {
        Debug.Log("call TitleSharingObjectManager Awake()");
        this.canvasGameObj = GameObject.Find("Canvas");
        this.versionTextValueGameObj = this.canvasGameObj.transform.Find("versionTextValue").gameObject;
    }
}
