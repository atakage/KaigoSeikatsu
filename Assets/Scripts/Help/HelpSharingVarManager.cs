using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpSharingVarManager : MonoBehaviour
{
    public GameObject helpCanvasGameObj;
    public GameObject closeButtonGameObj;

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
        this.helpCanvasGameObj = GameObject.Find("HelpCanvas");
        this.closeButtonGameObj = this.helpCanvasGameObj.transform.Find("closeButton").gameObject;

    }
}
