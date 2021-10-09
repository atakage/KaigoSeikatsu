using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitVar : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject menuBoxGameObj;
    public GameObject menuGridGameObj;
    public GameObject titleReturnAlertBoxGameObj;
    void Awake()
    {
        this.canvasGameObj = GameObject.Find("Canvas");
        this.menuBoxGameObj = this.canvasGameObj.transform.Find("menuBox").gameObject;
        this.menuGridGameObj = this.menuBoxGameObj.transform.Find("menuGrid").gameObject;
        this.titleReturnAlertBoxGameObj = this.menuBoxGameObj.transform.Find("titleReturnAlertBox").gameObject;
    }
}
