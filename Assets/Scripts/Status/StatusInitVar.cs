using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInitVar : MonoBehaviour
{
    public GameObject canvasGameObj;
    public GameObject closeButtonGameObj;
    void Awake()
    {
        this.canvasGameObj = GameObject.Find("StatusMainCanvas");
        this.closeButtonGameObj = this.canvasGameObj.transform.Find("closeButton").gameObject;
    }
}
