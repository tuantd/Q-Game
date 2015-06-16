using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public enum ButtonState
{
	ACTIVE = 0,
	INACTIVE
}

public class ButtonView : MonoBehaviour {
	public Sprite activeSprite;
	public Sprite inActiveSprite;
	private Image buttonImage;
	// Use this for initialization
	void Awake () {
		buttonImage = GetComponent<Image>();
		int state = PlayerPrefs.GetInt(GameConstant.SOUND_STATE, 0);
		if (state == 0){
			buttonImage.sprite = activeSprite;
		}
		else{
			buttonImage.sprite = inActiveSprite;
		}
	}

	public void SetState (ButtonState state){
		switch(state){
		case ButtonState.ACTIVE:
			buttonImage.sprite = activeSprite;
			break;
		case ButtonState.INACTIVE:
			buttonImage.sprite = inActiveSprite;
			break;
		}
	}

}
