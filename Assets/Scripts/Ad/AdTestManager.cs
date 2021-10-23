using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdTestManager : MonoBehaviour
{
    BannerView bannerView;
    private InterstitialAd interstitial;
    // Start is called before the first frame update
    void Start()
    {
        // Initialize the Google Mobile Ads SDK
        // app 実行のとき一度だけ
        MobileAds.Initialize(initStatus => { });

        RequestInterstitialTest();
        ShowInterstitialTest();
    }

    private void RequestInterstitialTest()
    {
        // android広告id
        string adUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";

        // Initialize an InterstitialAd
        this.interstitial = new InterstitialAd(adUnitIdAndroid);
        // Create an empty ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request
        this.interstitial.LoadAd(request);
    }

    private void ShowInterstitialTest()
    {
        if (this.interstitial.IsLoaded()) this.interstitial.Show();
    }

    private void RequestBannerTest()
    {
        // android広告id
        string adUnitIdAndroid = "ca-app-pub-3940256099942544/1033173712";

        // Create a 320x50 banner at the top of the screen
        bannerView = new BannerView(adUnitIdAndroid, AdSize.Banner, AdPosition.Center);
        // Create an empty ad request
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request
        bannerView.LoadAd(request);
    }

}
