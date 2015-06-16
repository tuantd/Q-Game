using UnityEngine;
using System.Collections;

public class ColliderDetect : MonoBehaviour {
	private SpriteRenderer spriteRender; 
	private QuestView questView;
	void Awake () {
		spriteRender = GetComponent<SpriteRenderer>();
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == GameConstant.POLYGON_TAG){
			if (spriteRender != null){
				spriteRender.color = Color.red;
			}
			questView.QuestDone();
		}
	}

	void OnTriggerEnter2D (Collider2D col){
		if (col.tag == GameConstant.POLYGON_TAG){
			if (spriteRender != null){
				spriteRender.color = Color.red;
			}
			questView.QuestDone();
		}
	}
}
