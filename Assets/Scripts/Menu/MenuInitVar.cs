using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuInitVar : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject menuBoxGameObj;
    public GameObject menuGridGameObj;
    void Awake()
    {
        this.canvasGameObj = GameObject.Find("Canvas");
        this.menuBoxGameObj = this.canvasGameObj.transform.GetChild(0).gameObject;
        this.menuGridGameObj = this.canvasGameObj.transform.GetChild(0).GetChild(0).gameObject;
    }
}
