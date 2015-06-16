using UnityEngine;
using System.Collections;

public class DoneQuest12 : MonoBehaviour {
	private float y = 0;
	private QuestView questView;
	// Use this for initialization
	void Awake () {
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
		BoxCollider2D boxCollider2D = GetComponent<BoxCollider2D>();
		Vector3 localScale = transform.localScale;
		y = transform.localPosition.y + boxCollider2D.size.y * localScale.y;
	}

	void OnCollisionEnter2D (Collision2D col){
		if (col.gameObject.tag == GameConstant.BALL_TAG){
			Transform ballTrans = col.gameObject.transform;
			Vector3 ballPos = ballTrans.localPosition;
			CircleCollider2D ballCollider = ballTrans.GetComponent<CircleCollider2D>();
			Vector3 scale = ballTrans.localScale;
			float ballY = ballPos.y - ballCollider.radius * scale.y;
			if (ballY >= y - 0.1f){
				questView.QuestDone();
			}
		}
	}
}
