using UnityEngine;
using System.Collections;

public class SnowmanHead : MonoBehaviour
{
    private QuestView questView;
    private Transform myTrans;
    private bool isCountDown = false;
    private float time;

    public int timeDone = 3;

    void Awake ()
    {
        myTrans = this.transform;
        questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView> ();
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

    void OnCollisionEnter2D (Collision2D coll)
    {
        if (coll.gameObject.tag == GameConstant.BALL_TAG) {

            ContactPoint2D[] contacts = coll.contacts;
            Vector3 pos = myTrans.position;
            bool isSuccess = false;
            foreach (ContactPoint2D contact in contacts) {

                Vector2 detal = contact.point - new Vector2 (pos.x, pos.y);
                if (detal.y > 0 && Mathf.Abs (detal.x) <= 0.2f) {

                    isSuccess =  true;
                    break;
                }
            }

            if (isSuccess) {
                if (!isCountDown && timeDone > 0){
                    isCountDown = true;
                    time = 0f;
                    timeDone = 3;
                }
            }
            else {
                if (isCountDown && timeDone > 0){
                    timeDone = 3;
                    time = 0f;
                    isCountDown = false;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == GameConstant.BALL_TAG){

            if (isCountDown && timeDone > 0){
                timeDone = 3;
                time = 0f;
                isCountDown = false;
            }
        }
    }
}
