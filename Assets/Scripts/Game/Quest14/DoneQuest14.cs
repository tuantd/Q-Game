using UnityEngine;
using System.Collections;

public class DoneQuest14 : MonoBehaviour {
    private QuestView questView;
    private Transform myTransform;
    private bool isCountDown = false;
    public int timeDone = 3;
    private float time;
    // Use this for initialization
    void Awake () {
        myTransform = transform;
        questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
    }
    
    // Update is called once per frame
    void Update () {
        Vector3 localEulerAngles = myTransform.localEulerAngles;
        float z = Mathf.Floor(localEulerAngles.z);
        if ((z >= 88 && z <= 92) || (z >= 268 && z <= 272)){
            if (!isCountDown && timeDone > 0){
                isCountDown = true;
                time = 0;
                timeDone = 3;
            }
        }
        else{
            if (isCountDown && timeDone > 0){
                isCountDown = false;
                time = 0;
                timeDone = 3;
            }
        }

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
                questView.countDownText.gameObject.SetActive(false);
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
}
