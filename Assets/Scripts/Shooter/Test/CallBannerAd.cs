using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBannerAd : MonoBehaviour
{
    [SerializeField] AdsController adScript;
    // Start is called before the first frame update
    void Start()
    {
        adScript.CallBannerAd();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableBannerAd() {
        adScript.DisableBanner();
    }

}
