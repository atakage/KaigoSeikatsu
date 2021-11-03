using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CareGiverListSharingObjectManager : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject playerClearListBoxGameObj;
    public GameObject playerClearScrollGameObj;
    public GameObject playerClearScrollContainerGameObj;
    public GameObject containerItem0GameObj;
    public GameObject noneClearFileBoxGameObj;
    public GameObject scrollHelpMessageBoxGameObj;
    public GameObject transparentScreenGameObj;
    public GameObject returnButtonGameObj;
    public GameObject careGiverListBoxGameObj;
    public GameObject careGiverListScrollViewGameObj;
    public GameObject scrollHandleGameObj;
    public GameObject careGiverListViewportGameObj;
    public GameObject careGiverListContentBoxGameObj;
    public GameObject connectionFailDefaultGameObj;
    public GameObject dataReadingMsgGameObj;
    public GameObject dataNoneMessage;
    public GameObject defaultFieldsGameObj;
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
        this.playerClearListBoxGameObj = this.canvasGameObj.transform.Find("playerClearListBox").gameObject;
        this.playerClearScrollGameObj = this.playerClearListBoxGameObj.transform.Find("playerClearScroll").gameObject;
        this.playerClearScrollContainerGameObj = this.playerClearScrollGameObj.transform.Find("container").gameObject;
        this.noneClearFileBoxGameObj = this.playerClearScrollGameObj.transform.Find("noneClearFileBox").gameObject;
        this.containerItem0GameObj = this.playerClearScrollContainerGameObj.transform.Find("item0").gameObject;
        this.scrollHelpMessageBoxGameObj = this.canvasGameObj.transform.Find("scrollHelpMessageBox").gameObject;
        this.transparentScreenGameObj = this.canvasGameObj.transform.Find("transparentScreen").gameObject;
        this.returnButtonGameObj = this.canvasGameObj.transform.Find("returnButton").gameObject;
        this.careGiverListBoxGameObj = this.canvasGameObj.transform.Find("careGiverListBox").gameObject;
        this.careGiverListScrollViewGameObj = this.careGiverListBoxGameObj.transform.Find("Scroll View").gameObject;
        this.scrollHandleGameObj = this.careGiverListScrollViewGameObj.transform.Find("Scrollbar Vertical").transform.Find("Sliding Area").transform.Find("Handle").gameObject;
        this.careGiverListViewportGameObj = this.careGiverListScrollViewGameObj.transform.Find("Viewport").gameObject;
        this.careGiverListContentBoxGameObj = this.careGiverListViewportGameObj.transform.Find("contentBox").gameObject;
        this.connectionFailDefaultGameObj = this.careGiverListContentBoxGameObj.transform.Find("connectionFailDefault").gameObject;
        this.defaultFieldsGameObj = this.careGiverListContentBoxGameObj.transform.Find("defaultFields").gameObject;
        this.dataReadingMsgGameObj = this.careGiverListContentBoxGameObj.transform.Find("dataReadingMessage").gameObject;
        this.dataNoneMessage = this.careGiverListContentBoxGameObj.transform.Find("dataNoneMessage").gameObject;


    }
}
