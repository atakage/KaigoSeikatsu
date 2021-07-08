using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobDiarySharingVarManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject containerGameObj;

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
        this.canvasGameObj = GameObject.Find("Canvas");
        this.containerGameObj = this.canvasGameObj.transform.Find("JobDiaryViewBox").GetChild(0).GetChild(0).GetChild(0).gameObject;
    }
}
