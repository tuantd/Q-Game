using UnityEngine;
using System.Collections;

public class BallObject : MonoBehaviour {
	private Transform myTransform;
	private Vector3 initialPos;
	private GameStage gameStage;
	private QuestView questView;
	// Use this for initialization
	void Awake () {
		myTransform = transform;
		initialPos = myTransform.localPosition;
		gameStage = myTransform.parent.GetComponent<GameStage>();
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}

    private void OnTriggerEnter2D (Collider2D collider){
		if (collider.tag == GameConstant.ITEM_TAG){
			Destroy(collider.gameObject);
			gameStage.DestroyItem();
		}
	}

	private void OnBecameInvisible(){
		if (!questView.isQuit){
			if (gameStage.currentItem > 0){
				gameStage.Retry();
			}
		}
	}
}
