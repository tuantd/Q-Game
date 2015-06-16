using UnityEngine;
using System.Collections;
using System;
using NendUnityPlugin.AD;
using NendUnityPlugin.Common;

public class NendAdIconManager : SingletonBehaviour<NendAdIconManager> {
    public GameObject nendAdIconPrb;
    private NendAdIcon nendAdIcon;
    // Use this for initialization
    void Awake () {
        if (Application.systemLanguage == SystemLanguage.Japanese){
            GameObject nendIconGO = GameObjectUtility.AddChild(gameObject, nendAdIconPrb);
            nendAdIcon = nendIconGO.GetComponent<NendAdIcon>();
            nendAdIcon.AdLoaded += OnFinishLoadIconAd;
            nendAdIcon.AdReceived += OnReceiveIconAd;
            nendAdIcon.AdFailedToReceive += OnFailToReceiveIconAd;
            nendAdIcon.AdClicked += OnClickIconAd;
        }
    }

    private void SetNendAdIcon(){
    }

    public void OnFinishLoadIconAd (object sender, EventArgs args)
    {
        Debug.Log ("Add Load Done");
    }
    
    public void OnClickIconAd (object sender, EventArgs args)
    {
        Debug.Log ("Add Click");
    }
    
    public void OnReceiveIconAd (object sender, EventArgs args)
    {
        Debug.Log ("Add Receive");
    }
    
    public void OnFailToReceiveIconAd (object sender, NendAdErrorEventArgs args)
    {
        Debug.Log ("Fail: " + args.Message);
    }
}
