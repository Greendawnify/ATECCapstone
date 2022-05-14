using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class AdsController : MonoBehaviour, IUnityAdsListener
{
    [SerializeField] BannerPosition bannerPosition;

    string GooglePlayID = "3587856";
    string myrewardedPlacementId = "rewardedVideo";
    string myVideoPlacementId = "video";
    string myBannerId = "bannerAd";
    bool testMode = false;


    // Start is called before the first frame update
    void Awake()
    {
        Advertisement.AddListener(this);
        Advertisement.Initialize(GooglePlayID, testMode);

        Advertisement.Banner.SetPosition(bannerPosition);
    }


    public void CallRewardedVideoAd() {
        // TODO put the ad in a courtine

        StartCoroutine(ShowAd(myrewardedPlacementId, false));
        Debug.Log("Rewarded ad called");
    }

    public void CallVideoAd() {
        StartCoroutine(ShowAd(myVideoPlacementId, false));
        Debug.Log("regular video ad called");
    }

    public void CallBannerAd() {
        if (Advertisement.isShowing)
            return;

        StartCoroutine(ShowAd(myBannerId, true));
        Debug.Log("banner ad called");
    }

    public bool IsAdShowing() {
        return Advertisement.isShowing;
    }

    public void DisableBanner() {
        Advertisement.Banner.Hide(false);
    }

    public bool isAdReady(string adName) {
        return Advertisement.IsReady(adName);
    }

    public void OnUnityAdsDidError(string message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            Debug.Log("You watched the ad till the end here is a reward");
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
            Debug.Log("You skipped the ad");
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidStart(string placementId)
    {
    }

    public void OnUnityAdsReady(string placementId)
    {
    }

    IEnumerator ShowAd(string adID, bool isBanner) {
        while (!Advertisement.IsReady(adID))
        {
            yield return new WaitForSeconds(0.5f);
        }

        if (isBanner)
        {
            Advertisement.Banner.Show(adID);
        }
        else {
            Advertisement.Show(adID);
        }
    }

    
}
