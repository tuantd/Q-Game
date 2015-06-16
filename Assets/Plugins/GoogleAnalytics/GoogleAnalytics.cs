using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoogleAnalytics : MonoBehaviour {

	public string propertyID;
	
	public static GoogleAnalytics instance;
	
	public string bundleID;
	public string appName;
	public string appVersion;
	
	private string screenRes;
	private string clientID;
	
	void Awake()
	{
		if(instance)
			DestroyImmediate(gameObject);
		else
		{
			DontDestroyOnLoad(gameObject);
			instance = this;
		}
	}
	
	void Start() 
	{
	
		screenRes = Screen.width + "x" + Screen.height;
		
		clientID = SystemInfo.deviceUniqueIdentifier;
			
	}

	public void LogScreen(string title)
	{
		title = WWW.EscapeURL(title);
		StartCoroutine(SendData(title));
	}

	private IEnumerator SendData(string title){
		
		var url = "http://www.google-analytics.com/collect?v=1&ul=en-us&t=appview&sr="+screenRes+"&an="+WWW.EscapeURL(appName)+"&a=448166238&tid="+propertyID+"&aid="+bundleID+"&cid="+WWW.EscapeURL(clientID)+"&_u=.sB&av="+appVersion+"&_v=ma1b3&cd="+title+"&qt=2500&z=185";
		Debug.Log(url);
		WWW www = new WWW(url);
		yield return www;
		if(www.error == null)
		{
			if (www.responseHeaders.ContainsKey("STATUS"))
			{
				if (www.responseHeaders["STATUS"] == "HTTP/1.1 200 OK")	
				{
					Debug.Log ("GA Success");
				}else{
					Debug.LogWarning(www.responseHeaders["STATUS"]);	
				}
			}else{
				Debug.LogWarning("Event failed to send to Google");	
			}
		}else{
			Debug.LogWarning(www.error.ToString());	
		}
	}
	
	
}