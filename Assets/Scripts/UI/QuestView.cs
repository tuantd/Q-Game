using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QuestView : MonoBehaviour{
	public GameObject gameViewGO;
    public GameObject doneText;
    public GameObject tapscreenText;
	public Text questText;
	public Text countDownText;
	public bool isQuit {get; set;}
	private bool isQuestDone = false;
	private ViewLoader viewLoader;
	private string currentArea;
	private string currenStage;
	private GameObject stageGO;
	private bool isTrackScreen;
	// Use this for initialization
	void Awake () {
		doneText.SetActive(false);
		tapscreenText.SetActive(false);
		viewLoader = ViewLoader.Instance;
		isTrackScreen = true;
#if !UNITY_EDITOR
		IMobileAdManager.Instance.HideAd();
#endif
	}
	
	// Update is called once per frame
	void Update () {
		if (isQuestDone){
			if (Input.GetButtonDown("Fire1")){
				isQuestDone = false;
				doneText.SetActive(false);
				tapscreenText.SetActive(false);
				Exit();
			}
		}
	}

	public void LoadStage(string areaNum, string stageNum){
		currentArea = areaNum;
		currenStage = stageNum;
		string stagePrbName = "Stage" + areaNum + "_" + stageNum;
		GameObject stagePrb = Resources.Load("Views/Stages/" + stagePrbName) as GameObject;
		stageGO = GameObjectUtility.AddChild(stagePrbName, gameViewGO, stagePrb);
		stageGO.transform.localScale = Vector3.one;
        questText.text = Language.Get("STAGE" + areaNum  + "_" + stageNum);
		if (isTrackScreen){
#if !UNITY_EDITOR
			GoogleAnalytics.instance.LogScreen(stagePrbName);
#endif
			isTrackScreen = false;
		}
	}
	
	public void QuestDone(){
		if (isQuestDone)
			return;
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.CLEAR_SOUND);
		}
		doneText.SetActive(true);
		tapscreenText.SetActive(true);
		isQuestDone = true;
		string key = currentArea + "_" + currenStage;
		PlayerPrefs.SetInt(key, 1);
	}

	public void OnClickRetry(){
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		Retry();
	}

	public void OnClickExit(){
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		Exit();
	}

	public void Retry(){
		Destroy(stageGO);
		LoadStage(currentArea, currenStage);
	}

	private void Exit(){
		BackHome();	
#if !UNITY_EDITOR
		NendAdInterstitialManager.Instance.Show();
#endif
	}

	private void BackHome(){
		Destroy(stageGO);
		viewLoader.LoadViewName("HomeView");
		HomeView homeView = viewLoader.CurrentView.GetComponent<HomeView>();
		int areaNum = int.Parse(currentArea);
		homeView.Initialize(areaNum);
	}

	
	void OnApplicationQuit() {
		isQuit = true;
	}


}
