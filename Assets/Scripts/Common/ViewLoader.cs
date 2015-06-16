using UnityEngine;
using System.Collections;

public class ViewLoader : SingletonBehaviour<ViewLoader> 
{
	public string firstViewName;
	public GameObject CurrentView {get; set;}

	void Awake () 
	{
        if (Application.systemLanguage == SystemLanguage.Japanese){
            Language.SwitchLanguage("JA");
        }
        else{
            Language.SwitchLanguage("EN");
        }
		CurrentView = GameObject.Find (firstViewName);
		HomeView homeView = CurrentView.GetComponent<HomeView>();
		if (homeView != null){
			int areaNum = 1;
			PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + areaNum, 1);
			homeView.Initialize(areaNum);
		}
	}

	public void LoadViewName (string viewName)
	{
		Destroy (CurrentView);
		GameObject viewPrefab = Resources.Load ("Views/" + viewName) as GameObject;
		CurrentView = Instantiate (viewPrefab) as GameObject;
	}
}
