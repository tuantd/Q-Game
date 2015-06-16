using UnityEngine;
using System.Collections;

public class DoneQuest36 : MonoBehaviour {
    private SpriteRenderer spriteRender; 
    private QuestView questView;
    private string lastBox = "";
    void Awake () {
        spriteRender = GetComponent<SpriteRenderer>();
        questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
    }
    
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == GameConstant.OBSTACLE_TAG){
            if(lastBox == "") {
                lastBox = col.gameObject.name;
                if (spriteRender != null){
                    spriteRender.color = Color.yellow;
                }
            }
            else if(lastBox != col.gameObject.name) {
                if (spriteRender != null){
                    spriteRender.color = Color.red;
                }
                questView.QuestDone();
            }
        }
    }
    
    void OnTriggerEnter2D (Collider2D col){
        if (col.gameObject.tag == GameConstant.OBSTACLE_TAG){
            if(lastBox == "") {
                lastBox = col.gameObject.name;
                if (spriteRender != null){
                    spriteRender.color = Color.yellow;
                }
            }
            else if(lastBox != col.gameObject.name) {
                if (spriteRender != null){
                    spriteRender.color = Color.red;
                }
                questView.QuestDone();
            }
        }
    }
}
