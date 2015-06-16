using UnityEngine;
using System.Collections;

public class GameStage : MonoBehaviour {
	public int numItemClear = 0;
	public int currentItem{get; set;}
	private QuestView questView;
	// Use this for initialization
	void Awake () {
		currentItem = numItemClear;
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}

	public void DestroyItem(){
		if (currentItem > 0){
			currentItem --;
		}
		if (currentItem == 0){
			questView.QuestDone();
		}
	}
	
	public void Retry(){
		questView.Retry();
	}
}
