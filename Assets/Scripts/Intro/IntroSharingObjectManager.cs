using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject testPaperBoxGameObj;
    public GameObject playerInfoBoxGameObj;
    public GameObject nameGameObj;

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

    private void Awake()
    {
        this.canvasGameObj = GameObject.Find("Canvas");
        this.testPaperBoxGameObj = this.canvasGameObj.transform.Find("TestPaperBox").gameObject;
        this.playerInfoBoxGameObj = this.testPaperBoxGameObj.transform.Find("playerInfoBox").gameObject;
        this.nameGameObj = this.playerInfoBoxGameObj.transform.Find("name").gameObject;
    }
}
