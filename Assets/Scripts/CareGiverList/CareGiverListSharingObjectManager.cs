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
        this.containerItem0GameObj = this.playerClearScrollContainerGameObj.transform.Find("item0").gameObject;

    }
}
