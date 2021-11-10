using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleManager : MonoBehaviour
{
    public AdManager adManager;
    public int SetWidth = 720;
    public int SetHeight = 1280;
    void Start()
    {
        adManager = GameObject.Find("AdManager").GetComponent("AdManager") as AdManager;
        Screen.SetResolution(SetWidth, SetHeight, true);
    }

    void Update()
    {
        adManager.ShowInterstitial();
        // 2021.10.24 追加
        // 広告が終わったら再生off
        adManager.adSwitch = false;
    }
}