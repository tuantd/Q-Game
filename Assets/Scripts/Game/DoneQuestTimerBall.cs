using UnityEngine;
using System.Collections;

public class DoneQuestTimerBall : MonoBehaviour {
	private QuestView questView;
	private bool isCountDown = false;
	public int timeDone = 3;
	private float time;
	// Use this for initialization
	void Awake () {
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}
	
	void Update(){
		if (isCountDown){
			time += Time.deltaTime;
			if (time >= 0.5f){
				questView.countDownText.gameObject.SetActive(true);
			}
			if (time >= 1.0f){
				time = 0.0f;
				timeDone --;
				questView.countDownText.text = timeDone.ToString();
			}
			if (timeDone == 0){
				isCountDown = false;
				questView.QuestDone();
			}
		}
		else{
			time += Time.deltaTime;
			if (time >= 0.5f){
				questView.countDownText.gameObject.SetActive(false);
			}
		}
	}
	
	private void OnTriggerEnter2D(Collider2D collision){

        if (collision.gameObject.tag == GameConstant.BALL_TAG){
            if (!isCountDown && timeDone > 0){
                isCountDown = true;
                time = 0f;
                timeDone = 3;
            }
        }
	}
	
	private void OnTriggerExit2D(Collider2D collision){

        if (collision.gameObject.tag == GameConstant.BALL_TAG){
            if (isCountDown && timeDone > 0){
                timeDone = 3;
                time = 0f;
                isCountDown = false;
            }
        }
	}
}
