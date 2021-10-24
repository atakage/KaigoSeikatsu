using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public AdManager adManager;

    void Start()
    {
        adManager = GameObject.Find("AdManager").GetComponent("AdManager") as AdManager;
    }

    void Update()
    {
        adManager.ShowInterstitial();
        // 2021.10.24 追加
        // 広告が終わったら再生off
        adManager.adSwitch = false;
    }
}