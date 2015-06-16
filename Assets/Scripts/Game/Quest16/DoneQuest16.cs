using UnityEngine;
using System.Collections;

public class DoneQuest16 : MonoBehaviour {
	private Transform myTransform;
	private float startAngle;
	private float currentAngle;
	private float totalRotate = 0;
	private bool isDone = false;
	private QuestView questView;
	void Awake () {
		myTransform = this.transform;
		startAngle = myTransform.localEulerAngles.z;
		questView = ViewLoader.Instance.CurrentView.GetComponent<QuestView>();
	}
	
	// Update is called once per frame
	void Update () {
		if (isDone)
			return;
		currentAngle = myTransform.localEulerAngles.z;
		float angleDelta = currentAngle - startAngle;
		totalRotate += angleDelta;
		startAngle = currentAngle;
		float rotateS = Mathf.Abs(totalRotate);
		if (rotateS >= 355.0f && myTransform.rotation.z > 0
		    || (rotateS <= 10 && rotateS > 1.0f)  && myTransform.rotation.z < 0){
			isDone = true;
			questView.QuestDone();
		}
	}
}
