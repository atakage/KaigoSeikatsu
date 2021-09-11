using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyForEndingSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject panelGameObj;
    public GameObject panelTextGameObj;
    public GameObject confirmButtonGameObj;
    public GameObject surveyBoxGameObj;
    public GameObject alertBoxGameObj;
    public GameObject alertBoxCancelButtonGameObj;
    public GameObject confirmAlertBoxGameObj;
    public GameObject confirmAlertBoxConfirmButtonGameObj;
    public GameObject confirmAlertBoxCancelButtonGameObj;
    public GameObject continueAlertBoxGameObj;
    public GameObject continueAlertConfirmButtonBoxGameObj;
    public GameObject continueAlertCancelButtonBoxGameObj;
    public GameObject fadeGameObj;
    public GameObject thanksImageGameObj;
    public GameObject plusButtonGameObj;
    public GameObject dropDownBoxGameObj;
    public GameObject causeDropDownBox0GameObj;

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
        this.panelGameObj = this.canvasGameObj.transform.Find("Panel").gameObject;
        this.panelTextGameObj = this.panelGameObj.transform.Find("Text").gameObject;
        this.surveyBoxGameObj = this.canvasGameObj.transform.Find("surveyBox").gameObject;
        this.alertBoxGameObj = this.canvasGameObj.transform.Find("alertBox").gameObject;
        this.alertBoxCancelButtonGameObj = this.alertBoxGameObj.transform.Find("cancelButton").gameObject;
        this.confirmAlertBoxGameObj = this.canvasGameObj.transform.Find("confirmAlertBox").gameObject;
        this.confirmAlertBoxConfirmButtonGameObj = this.confirmAlertBoxGameObj.transform.Find("confirmButton").gameObject;
        this.confirmAlertBoxCancelButtonGameObj = this.confirmAlertBoxGameObj.transform.Find("cancelButton").gameObject;
        this.continueAlertBoxGameObj = this.canvasGameObj.transform.Find("continueAlertBox").gameObject;
        this.continueAlertConfirmButtonBoxGameObj = this.continueAlertBoxGameObj.transform.Find("confirmButton").gameObject;
        this.continueAlertCancelButtonBoxGameObj = this.continueAlertBoxGameObj.transform.Find("cancelButton").gameObject;
        this.confirmButtonGameObj = this.surveyBoxGameObj.transform.Find("confirmButton").gameObject;
        this.fadeGameObj = this.canvasGameObj.transform.Find("fadeImage").gameObject;
        this.thanksImageGameObj = this.canvasGameObj.transform.Find("thanksImage").gameObject;
        this.plusButtonGameObj = this.surveyBoxGameObj.transform.Find("plusButton").gameObject;
        this.dropDownBoxGameObj = this.surveyBoxGameObj.transform.Find("DropdownBox").gameObject;
        this.causeDropDownBox0GameObj = this.dropDownBoxGameObj.transform.Find("causeDropDownBox0").gameObject;
    }
}
