using UnityEngine;
using System.Collections;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

public class NendAdInterstitialManager : SingletonBehaviour<NendAdInterstitialManager> {
	public string apiKey = "18caa9662dbbb269b08b0ba9c10b877b5dfe6d3e";
	public string spotId = "321632";
	public delegate void NenAdInterstitialCallback(); 
	public event NenAdInterstitialCallback Callback;
	// Use this for initialization
	void Start () {
		NendAdInterstitial.Instance.Load(apiKey, spotId);
		NendAdInterstitial.Instance.AdLoaded += OnFinishLoadInterstitialAd;
		NendAdInterstitial.Instance.AdShown += OnShowInterstitialAd;
		NendAdInterstitial.Instance.AdClicked += OnClickInterstitialAd;
	}

	public void Show(){
		NendAdInterstitial.Instance.Show();
	}

	public void Show(string spotId){
		NendAdInterstitial.Instance.Show(spotId);
	}

	public void OnFinishLoadInterstitialAd (object sender, NendAdInterstitialLoadEventArgs args)
	{
		switch (args.StatusCode) {
		case NendAdInterstitialStatusCode.SUCCESS:
			Debug.Log("Load ad success");
			break;
		case NendAdInterstitialStatusCode.FAILED_AD_DOWNLOAD:
			Debug.Log("Failed ad download");
			break;
		case NendAdInterstitialStatusCode.FAILED_AD_REQUEST:
			Debug.Log("Failed ad request");
			break;
		case NendAdInterstitialStatusCode.INVALID_RESPONSE_TYPE:
			Debug.Log("Invalid response type");
			break;
		}
		Callback();
	}
	
	public void OnClickInterstitialAd (object sender, NendAdInterstitialClickEventArgs args)
	{
		switch (args.ClickType) {
		case NendAdInterstitialClickType.CLOSE:
			Debug.Log("Click close");
			break;
		case NendAdInterstitialClickType.DOWNLOAD:
			Debug.Log("Click download");
			break;
		case NendAdInterstitialClickType.EXIT:
			Debug.Log("Click exit");
			break;
		}
	}
	
	public void OnShowInterstitialAd (object sender, NendAdInterstitialShowEventArgs args)
	{
		Debug.Log ("Show Interstitial Ad");
		switch (args.ShowResult) {

		}
	}
}
