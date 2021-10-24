using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    static AdManager adManagerInstance;
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
        RequestInterstitial();
        Debug.Log("call RequestInterstitialTest()");
    }

    private void RequestInterstitial()
    {
        // android広告id(Test)
        string adUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";
        // Initialize an InterstitialAd
        this.interstitial = new InterstitialAd(adUnitIdAndroid);
        // Create an empty ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (this.interstitial.IsLoaded() && this.adSwitch) this.interstitial.Show();
    }
}
