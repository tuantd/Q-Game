using UnityEngine;
using System.Collections;

public class CollisionWithBall : MonoBehaviour {
	private QuestView questView;
	// Use this for initialization
	void Awake () {
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == GameConstant.BALL_TAG){
			questView.QuestDone();
		}
	}

	void OnTriggerEnter2D (Collider2D col){
		if (col.tag == GameConstant.BALL_TAG){
			questView.QuestDone();
		}
	}
}
