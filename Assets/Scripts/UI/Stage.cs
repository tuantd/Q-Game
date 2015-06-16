using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Stage : MonoBehaviour {
	public Text stageName;
	private Button button;
	private HomeView homeView;
	private RectTransform rectTransform;

	// Use this for initialization
	void Awake () {
		button = GetComponent<Button>();
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetStage(HomeView homeView, int areaNum, int stageNum){
		this.homeView = homeView;
		stageName.text = stageNum.ToString();
		rectTransform.localScale = Vector3.one;
		string key = areaNum + "_" + stageNum;
		int status = PlayerPrefs.GetInt(key, -1);

        Image spriteImage = GetComponent<Image>();

        SpriteState spriteState = new SpriteState();

        Sprite highlighted = Resources.Load<Sprite>("Sprites/StageButtons/stageSelectBtn-highlighted");
		spriteState.pressedSprite = highlighted;
        if (status == -1 || status == 0){
            Sprite normal  = Resources.Load<Sprite>("Sprites/StageButtons/stageSelectBtn-normal");
			spriteImage.sprite = normal;
            stageName.color = Color.black;
        }
        else{
            int odd = stageNum % 10;
            if (odd > 0){
                Sprite cleared = Resources.Load<Sprite>("Sprites/StageButtons/stageSelectBtn-cleared_0" + odd);
				spriteImage.sprite = cleared;
            }
            else {
                Sprite cleared = Resources.Load<Sprite>("Sprites/StageButtons/stageSelectBtn-cleared_10");
				spriteImage.sprite = cleared;
            }
            stageName.color = Color.white;
        }

        button.spriteState = spriteState; 
	}

	public void StageSelected(){
		// Load Stage map
		int soundState = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (soundState == 0){
			AudioPlayer.Instance.PlaySE(GameConstant.BUTTON_CLICK_SOUND);
		}
		homeView.LoadStage(stageName.text);
	}
}
