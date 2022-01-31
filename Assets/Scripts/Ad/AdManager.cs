using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    static AdManager adManagerInstance;
    public BuildManager buildManager;
    public bool adSwitch;
    private InterstitialAd interstitial;

    private void Awake()
    {
        if (adManagerInstance != null)
        {
            Destroy(this.gameObject);
        }else if (adManagerInstance == null)
        {
            adManagerInstance = this;
            DontDestroyOnLoad(adManagerInstance);
        }
    }

    private void Start()
    {
        buildManager = GameObject.Find("BuildManager").GetComponent("BuildManager") as BuildManager;

        RequestInterstitial();
        Debug.Log("call RequestInterstitialTest()");
    }

    private void RequestInterstitial()
    {
        string adUnitIdAndroid = "";
        // 2022.01.12 修正
        // buildModeによってAdMobId設定
        // realなら
        if (buildManager.realMode)
        {
            adUnitIdAndroid = "ca-app-pub-1638099411160865/4720497227";
        }
        // testなら
        else
        {
            adUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
        }
        
        // Initialize an InterstitialAd
        this.interstitial = new InterstitialAd(adUnitIdAndroid);
        // Create an empty ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded() && this.adSwitch)
        {
            Debug.Log("Ads On!");
            this.interstitial.Show();
        }
            
    }
}
