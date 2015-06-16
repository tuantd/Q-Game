using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class HomeView : MonoBehaviour {
	public GameObject stagePrb;
	public GameObject stageParentGO;
	public Text areaText;
	public Text noticeText;
	public GameObject nextButtonGO;
	public GameObject backButton; 
	public GameObject soundButton;
    public GameObject firstStageNavi;
	public GameObject newStageGO;
	private GameButton nextButton;
	private float offset = 5.0f;
	private int areaNum = 1;
	private ViewLoader viewLoader;
	private List<GameObject> stageList;
	private GameObject areaLockGO;

	void Awake () {
		viewLoader = ViewLoader.Instance;
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioManager.Instance.PlayBGM(GameConstant.BGM_SOUND);
		}
#if UNITY_IPHONE || UNITY_ANDROID
		IMobileAdManager.Instance.ShowAd();
#endif
#if !UNITY_EDITOR
		GoogleAnalytics.instance.LogScreen("HomeView");
#endif
	}

	public void Initialize(int areaNum){
		this.areaNum = areaNum;
		nextButton = nextButtonGO.GetComponent<GameButton>();
		int nextArea = areaNum + 1;
		if (IsNextAreaUnlock(nextArea)){
			int nextAreaStatus = PlayerPrefs.GetInt(GameConstant.UNLOCK_PREFIX + "_" + nextArea, 0);
			if (nextAreaStatus == 0){
				NewStageAnimation();
				nextButton.ChangeState(GameButton.ButtonState.FADE);
				PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + nextArea, 1);
			}
			nextButtonGO.GetComponent<Image>().enabled = true;
			backButton.GetComponent<Image>().enabled = true;
		}
		stageList = new List<GameObject>();
		areaText.text = "LEVEL" + areaNum;
		if (areaNum == 1){
			Tutorial();
		}
		else{
			nextButtonGO.GetComponent<Image>().enabled = true;
			backButton.GetComponent<Image>().enabled = true;
            if (areaNum == GameConstant.MAX_LEVEL){
                LoadStageUI(1, 7);
            }
            else{
                LoadStageUI(1, 10);
            }
		}
	}

	private bool IsNextAreaUnlock(int nextArea){
		if (nextArea == 1){
			return true;
		}
		int nextAreaStatus = PlayerPrefs.GetInt(GameConstant.UNLOCK_PREFIX + "_" + nextArea, 0);
		if (nextAreaStatus == 1){
			return true;
		}
		int clearStageNum = TotalClearStage(nextArea - 1);
		int requestNum = areaNum == 1 ? GameConstant.CLEAR_STAGE_FIRST : GameConstant.CLEAR_STAGE_BASE;
		if (clearStageNum >= requestNum){
			return true;
		}
		return false;
	}

	private void Tutorial(){
		int clearStageNum = 0;
		for(int stageNum = 1; stageNum <= 4; stageNum ++){
			int status = PlayerPrefs.GetInt(areaNum + "_" + stageNum, 0);
			if (status == 0){
				break;
			}
			else{
				clearStageNum = stageNum;
			}
		}
		if (clearStageNum == 4){
			LoadStageUI (1, 10);
		}
		else if (clearStageNum < 4){
			LoadStageUI (1, clearStageNum + 1);
		}

        if (clearStageNum == 0) {
            firstStageNavi.SetActive(true);
        }
        else {
            firstStageNavi.SetActive(false);
        }
	}


	private void UpdateMissionText(){
		int clearStageNum = TotalClearStage(areaNum);
		string missionText = string.Empty;
		int requestNum = areaNum == 1 ? GameConstant.CLEAR_STAGE_FIRST : GameConstant.CLEAR_STAGE_BASE;
		if (clearStageNum >= requestNum){
			missionText = Language.Get("MISSION_DONE");
		}
		else {
            missionText = Language.Get("MISSION_TEXT");
			missionText = missionText.Replace("{X0}", "" + requestNum);
		}
		noticeText.gameObject.SetActive(true);
		noticeText.text = missionText;
	}

	public int TotalClearStage(int areaNumber){
		int clearStageNum = 0;
		int from = (areaNumber - 1) * 10 + 1;
		int to = areaNumber * 10;
		for (int i = from; i <= to; i ++){
			int status = PlayerPrefs.GetInt(areaNumber + "_" + i);
			if (status == 1){
				clearStageNum ++;
			}
		}
		return clearStageNum;
	}

	private void LoadStageUI(int fromStage, int toStage){
		if (areaLockGO != null) areaLockGO.SetActive(false);
		if (toStage == 10){
			UpdateMissionText();
		}
		for (int i = fromStage - 1; i < toStage; i++){
			int col = i % 5;
			int row = i / 5;
			GameObject stageGO = GameObjectUtility.AddChild("Stage_" + (i + 1), stageParentGO, stagePrb);
			stageList.Add(stageGO);
			Transform stageTrans = stageGO.transform;

			Vector3 localPosition = new Vector3(-310 + col * 155, 235 - row * 100, 0);
			stageTrans.localPosition = localPosition;
			Stage stage = stageGO.GetComponent<Stage>();
			int stageNum = (areaNum  - 1) * 10 + (i + 1);
			stage.SetStage(this, areaNum, stageNum);
		}
	}

	private void AddLockArea(){
		if (areaLockGO == null){
			GameObject lockPrb = Resources.Load("Common/AreaLock") as GameObject;
			GameObject panelGO = GameObject.Find("Panel");
			areaLockGO = GameObjectUtility.AddChild(panelGO, lockPrb);
			areaLockGO.transform.localScale = Vector3.one * 2.0f;
			areaLockGO.transform.localPosition = Vector3.zero;
		}
		else{
			areaLockGO.SetActive(true);
		}
		noticeText.gameObject.SetActive(false);
	}

	public void LoadStage (string stageNum){
		viewLoader.LoadViewName("QuestView");
		QuestView questView = viewLoader.CurrentView.GetComponent<QuestView>();
		questView.LoadStage(areaNum.ToString(), stageNum);
	}

	public void NextArea(){
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		int nextArea = 0;
		if (areaNum < GameConstant.MAX_LEVEL){
			nextArea = areaNum + 1;
		}
		else{
			nextArea = 1;
		}
		bool isUnlockArea = IsNextAreaUnlock(nextArea);
		areaNum = nextArea;
		foreach(GameObject stageGO in stageList){
			Destroy(stageGO);
		}
		stageList.Clear();
		areaText.text = "LEVEL" + areaNum;
		nextButton.ChangeState(GameButton.ButtonState.NORMAL);
		if (isUnlockArea){
			PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + nextArea, 1);
			if (areaNum == GameConstant.MAX_LEVEL){
				LoadStageUI(1, 7);
			}
			else{
				LoadStageUI(1, 10);
			}		
		}
		else{
			AddLockArea();
		}
	}

	public void BackArea(){
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		int nextArea = 0;
		if (areaNum > 1){
			nextArea = areaNum - 1;
		}
		else{
			nextArea = GameConstant.MAX_LEVEL;
		}
		bool isUnlockArea = IsNextAreaUnlock(nextArea);
		areaNum = nextArea;
		foreach(GameObject stageGO in stageList){
			Destroy(stageGO);
		}
		stageList.Clear();
		areaText.text = "LEVEL" + areaNum;
		nextButton.ChangeState(GameButton.ButtonState.NORMAL);
		if (isUnlockArea){
			if (areaNum == GameConstant.MAX_LEVEL){
				LoadStageUI(1, 7);
			}
			else{
				LoadStageUI(1, 10);
			}
			PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + nextArea, 1);
		}
		else{
			AddLockArea();
		}
	}

	public void OnSoundClick(){
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		ButtonView buttonView = soundButton.GetComponent<ButtonView>();
		if (soundState == 0){
			AudioManager.Instance.StopBGM();
			PlayerPrefs.SetInt(GameConstant.SOUND_STATE, 1);
			buttonView.SetState(ButtonState.INACTIVE);
		}
		else if (soundState == 1){
			PlayerPrefs.SetInt(GameConstant.SOUND_STATE, 0);
			AudioManager.Instance.PlayBGM(GameConstant.BGM_SOUND);
			buttonView.SetState(ButtonState.ACTIVE);
		}
	}

	private void NewStageAnimation(){
		newStageGO.SetActive(true);
		LeanTween.scale(newStageGO.GetComponent<RectTransform>(), Vector3.one * 0.3f, 0.8f)
			.setEase(LeanTweenType.easeOutBack)
			.setOnComplete(()=>{
			StartCoroutine("DisableNewStage");
		});
	}

	IEnumerator DisableNewStage(){
		yield return new WaitForSeconds(3.0f);
		newStageGO.SetActive(false);
	}
}
