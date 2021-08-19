using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject testPaperBoxGameObj;
    public GameObject checkNameAlertBoxGameObj;
    public GameObject checkNameAlertBoxTextGameObj;
    public GameObject checkNameConfirmButtonGameObj;
    public GameObject checkNameCancelButtonGameObj;
    public GameObject offLinePlayAlertBoxGameObj;
    public GameObject offLinePlayAlertBoxOffLineButtonGameObj;
    public GameObject offLinePlayAlertBoxOnLineButtonGameObj;
    public GameObject alertBoxGameObj;
    public GameObject playerInfoBoxGameObj;
    public GameObject nameGameObj;
    public GameObject nameValueGameObj;
    public GameObject nameTextGameObj;
    public GameObject checkNameButtonGameObj;
    

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
        this.checkNameAlertBoxGameObj = this.canvasGameObj.transform.Find("checkNameAlertBox").gameObject;
        this.checkNameAlertBoxTextGameObj = this.checkNameAlertBoxGameObj.transform.Find("Text").gameObject;
        this.checkNameConfirmButtonGameObj = this.checkNameAlertBoxGameObj.transform.Find("confirmButton").gameObject;
        this.checkNameCancelButtonGameObj = this.checkNameAlertBoxGameObj.transform.Find("cancelButton").gameObject;
        this.offLinePlayAlertBoxGameObj = this.canvasGameObj.transform.Find("offLinePlayAlertBox").gameObject;
        this.offLinePlayAlertBoxOffLineButtonGameObj = this.offLinePlayAlertBoxGameObj.transform.Find("offLineButton").gameObject;
        this.offLinePlayAlertBoxOnLineButtonGameObj = this.offLinePlayAlertBoxGameObj.transform.Find("onLineButton").gameObject;
        this.alertBoxGameObj = this.canvasGameObj.transform.Find("alertBox").gameObject;
        this.playerInfoBoxGameObj = this.testPaperBoxGameObj.transform.Find("playerInfoBox").gameObject;
        this.nameGameObj = this.playerInfoBoxGameObj.transform.Find("name").gameObject;
        this.nameValueGameObj = this.nameGameObj.transform.Find("value").gameObject;
        this.nameTextGameObj = this.nameGameObj.transform.Find("text").gameObject;
        this.checkNameButtonGameObj = this.testPaperBoxGameObj.transform.Find("checkNameButton").gameObject;

    }
}
