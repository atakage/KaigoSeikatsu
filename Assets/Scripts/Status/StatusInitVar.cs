using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInitVar : MonoBehaviour
{
    public GameObject canvasGameObj;
        public GameObject closeButtonGameObj;
        public GameObject playInfoBoxGameObj;
            public GameObject playTimeBox;
                public GameObject playTimeValue;
    void Awake()
    {
        this.canvasGameObj = GameObject.Find("StatusMainCanvas");
        this.closeButtonGameObj = this.canvasGameObj.transform.Find("closeButton").gameObject;
        this.playInfoBoxGameObj = this.canvasGameObj.transform.Find("playInfoBox").gameObject;
        this.playTimeBox = this.playInfoBoxGameObj.transform.Find("playTimeBox").gameObject;
        this.playTimeValue = this.playTimeBox.transform.Find("playTimeValue").gameObject;
    }
}
