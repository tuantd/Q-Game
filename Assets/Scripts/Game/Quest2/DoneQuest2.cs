using UnityEngine;
using System.Collections;

public class DoneQuest2 : MonoBehaviour {
	private SpriteRenderer spriteRender; 
	private GameStage gameStage;
	private bool isCollided;
	void Awake () {
		spriteRender = GetComponent<SpriteRenderer>();
		gameStage = transform.parent.GetComponent<GameStage>();
	}
	
	void OnCollisionEnter2D(Collision2D coll) {
		if (coll.gameObject.tag == GameConstant.POLYGON_TAG){
			if (!isCollided){
				if (spriteRender != null){
					spriteRender.color = Color.red;
				}
				gameStage.DestroyItem();
				isCollided = true;
			}
		}
	}
}
