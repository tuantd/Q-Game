using UnityEngine;
using System.Collections;

public class IMobileAdManager : SingletonBehaviour<IMobileAdManager> {
	private string partnerID = "32452";
	private string mediaID = "156275";
	private string spotID = "410257";
	private int adViewId;
	// Use this for initialization
	void Awake () {
#if UNITY_IPHONE || UNITY_ANDROID
		IMobileSdkAdsUnityPlugin.registerFullScreen(partnerID, mediaID, spotID);
		IMobileSdkAdsUnityPlugin.start(spotID);
		IMobileSdkAdsUnityPlugin.addObserver(gameObject.name);
		adViewId = IMobileSdkAdsUnityPlugin.show(spotID, IMobileSdkAdsUnityPlugin.AdType.BANNER, IMobileSdkAdsUnityPlugin.AdAlignPosition.CENTER, IMobileSdkAdsUnityPlugin.AdValignPosition.BOTTOM);
#endif
	}

	public void ShowAd(){
		IMobileSdkAdsUnityPlugin.setVisibility(adViewId, true);
	}

	public void HideAd(){
		IMobileSdkAdsUnityPlugin.setVisibility(adViewId, false);
	}
	
	void imobileSdkAdsSpotDidReady (string value) {
		Debug.Log("imobileSdkAdsSpotDidReady:" + value);
	}
	void imobileSdkAdsSpotDidFail (string value) {
		Debug.Log("imobileSdkAdsSpotDidFail:" + value);
	}
	void imobileSdkAdsSpotDidShow (string value) {
		Debug.Log("imobileSdkAdsSpotDidShow:" + value);
	}
	void imobileSdkAdsSpotDidClick (string value) {
		Debug.Log("imobileSdkAdsSpotDidClick:" + value);
	}
	void imobileSdkAdsSpotDidClose (string value) {
		Debug.Log("imobileSdkAdsSpotDidClose:" + value);
	}
	void imobileSdkAdsSpotDidGetFilterMode (string value) {
		Debug.Log("imobileSdkAdsSpotDidGetFilterMode:" + value);
	}

}
