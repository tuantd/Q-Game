using UnityEngine;
using System.Collections;

public class GridBallObject : MonoBehaviour {
	private GameStage gameStage;
	private SpriteRenderer spriteRender;
	public bool isColldied = false;
	// Use this for initialization
	void Awake () {
		spriteRender = GetComponent<SpriteRenderer>();
		gameStage = transform.parent.GetComponent<GameStage>();
	}
	
	private void OnCollisionEnter2D(Collision2D col){
		if (col.gameObject.tag == GameConstant.POLYGON_TAG){
			if (!isColldied){
				isColldied = true;
				spriteRender.color = Color.red;
				gameStage.DestroyItem();
			}
		}
	}
}
