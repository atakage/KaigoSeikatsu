using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public AdManager adManager;
    public BuildManager buildManager;
    public TitleSharingObjectManager titleSharingObjectManager;
    public int SetWidth = 720;
    public int SetHeight = 1280;
    void Start()
    {
        adManager = GameObject.Find("AdManager").GetComponent("AdManager") as AdManager;
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;
        titleSharingObjectManager = GameObject.Find("TitleSharingObjectManager").GetComponent("TitleSharingObjectManager") as TitleSharingObjectManager;
        Screen.SetResolution(SetWidth, SetHeight, true);

        titleSharingObjectManager.versionTextValueGameObj.GetComponent<Text>().text = buildManager.version;
    }

    void Update()
    {
        adManager.ShowInterstitial();
        // 2021.10.24 追加
        // 広告が終わったら再生off
        adManager.adSwitch = false;
    }

}