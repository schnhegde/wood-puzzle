using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;

public class AdsControl : MonoBehaviour
{


    protected AdsControl()
    {
    }

    private static AdsControl _instance;
    InterstitialAd interstitial;
    BannerView bannerView;
    public string Interstitial_ID_Android, Interstitial_ID_iOS, Banner_ID_Android, Banner_ID_iOS;
    public Boolean enableAds = true;

    public static AdsControl Instance { get { return _instance; } }

    void Awake()
    {
        if (FindObjectsOfType(typeof(AdsControl)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        RequestConfiguration requestConfiguration = new RequestConfiguration.Builder()
        .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.True)
        .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        MakeNewInterstial();
        RequestBanner();
        ShowBanner();
        DontDestroyOnLoad(gameObject); //Already done by CBManager
    }


    public void HandleInterstialAdClosed(object sender, EventArgs args)
    {

        if (interstitial != null)
            interstitial.Destroy();
        MakeNewInterstial();
    }

    void MakeNewInterstial()
    {
#if UNITY_ANDROID
        interstitial = new InterstitialAd(Interstitial_ID_Android);
#endif
#if UNITY_IPHONE
		interstitial = new InterstitialAd (Interstitial_ID_iOS);
#endif
        interstitial.OnAdClosed += HandleInterstialAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }


    public void showAds()
    {
        if (interstitial.IsLoaded() && enableAds)
            interstitial.Show();
    }


    private void RequestBanner()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
		string adUnitId = Banner_ID_Android;
#elif UNITY_IPHONE
		string adUnitId = Banner_ID_iOS;
#endif
        if (enableAds)
        {
            bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);

            // Create an empty ad request.
            AdRequest request = new AdRequest.Builder().Build();
            // Load the banner with the request.
            bannerView.LoadAd(request);
        }
    }

    public void ShowBanner()
    {
        if(enableAds)
        {
            bannerView.Show();
        }
    }

    public void HideBanner()
    {
        bannerView.Hide();
    }



    public void ShowFB()
    {
        Application.OpenURL("https://www.facebook.com/PonyStudio2507/?ref=settings");
    }

    public void RateMyGame()
    {
#if UNITY_EDITOR
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.schnhegde.woodpuzzle");
#elif UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.schnhegde.woodpuzzle");
#elif UNITY_IPHONE
        Application.OpenURL("https://itunes.apple.com/us/app/wood-block-breaker/id1412726341?ls=1&mt=8");
#else
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.schnhegde.woodpuzzle");
#endif


    }

}

