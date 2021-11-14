using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusInitVar : MonoBehaviour
{
    public GameObject canvasGameObj;
        public GameObject closeButtonGameObj;
        public GameObject playerNameBoxGameObj;
        public GameObject playInfoBoxGameObj;
            public GameObject playTimeBox;
                public GameObject playTimeValue;
            public GameObject progressBoxGameObj;
                public GameObject progressValueGameObj;
                public GameObject progressBarGameObj;
            public GameObject statusBoxGameObj;
                public GameObject fatigueValueGameObj;
                public GameObject satisfactionValueGameObj;
                public GameObject feelingValueGameObj;
            public GameObject moneyBoxGameObj;
                public GameObject moneyValueGameObj;
    void Awake()
    {
        this.canvasGameObj = GameObject.Find("StatusMainCanvas");
        this.closeButtonGameObj = this.canvasGameObj.transform.Find("closeButton").gameObject;
        this.playerNameBoxGameObj = this.canvasGameObj.transform.Find("playerNameBox").gameObject;
        this.playInfoBoxGameObj = this.canvasGameObj.transform.Find("playInfoBox").gameObject;
        this.playTimeBox = this.playInfoBoxGameObj.transform.Find("playTimeBox").gameObject;
        this.playTimeValue = this.playTimeBox.transform.Find("playTimeValue").gameObject;
        this.progressBoxGameObj = this.playInfoBoxGameObj.transform.Find("progressBox").gameObject;
        this.progressValueGameObj = this.progressBoxGameObj.transform.Find("progressValue").gameObject;
        this.progressBarGameObj = this.progressBoxGameObj.transform.Find("progressBar").gameObject;
        this.statusBoxGameObj = this.playInfoBoxGameObj.transform.Find("statusBox").gameObject;
        this.fatigueValueGameObj = this.statusBoxGameObj.transform.Find("fatigueValue").gameObject;
        this.satisfactionValueGameObj = this.statusBoxGameObj.transform.Find("satisfactionValue").gameObject;
        this.feelingValueGameObj = this.statusBoxGameObj.transform.Find("feelingValue").gameObject;
        this.moneyBoxGameObj = this.playInfoBoxGameObj.transform.Find("moneyBox").gameObject;
        this.moneyValueGameObj = this.moneyBoxGameObj.transform.Find("moneyValue").gameObject;
    }
}
