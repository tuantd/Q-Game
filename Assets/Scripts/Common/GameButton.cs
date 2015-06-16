using UnityEngine;
using System.Collections;

public class GameButton : MonoBehaviour {
	public enum ButtonState
	{
		NORMAL,
		FADE
	}	

	private Animator animator;
	private AnimatorStateInfo currentBaseState;
	static int normalState = Animator.StringToHash("Game Button.Normal");
	static int fadeState = Animator.StringToHash("Game Button.NextFade");

	public void ChangeState(ButtonState state){
		animator = GetComponent<Animator>();
		currentBaseState = animator.GetCurrentAnimatorStateInfo(0); 
		switch(state)
		{
		case ButtonState.NORMAL:
			if (currentBaseState.nameHash == fadeState)
			{
				Debug.Log("Fade->Normal");
				animator.SetBool("isFade", false);
			}
			break;
		case ButtonState.FADE:
			if (currentBaseState.nameHash == normalState)
			{
				Debug.Log("Normal->Fade");
				animator.SetBool("isFade", true);
			}
			break;
		}
	}


}


