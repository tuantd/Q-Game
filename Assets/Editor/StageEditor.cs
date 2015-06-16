using UnityEngine;
using System.Collections;
using UnityEditor;

public class StageEditor : EditorWindow {
	private string unlockAreaString = string.Empty;
	private bool isClearArea = false;
	private int clearArea = 1;
	[MenuItem ("Window/Stage Manager")]
	static void Init(){
		// Get existing open window or if none, make a new one:
		StageEditor window = (StageEditor)EditorWindow.GetWindow (typeof (StageEditor));
		window.Show();
	}

	void OnGUI(){
		if(GUILayout.Button("Clear Prefs", GUILayout.Width(100))){
			PlayerPrefs.DeleteAll();
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label("Unlock Area: ", GUILayout.Width(80));
		unlockAreaString = GUILayout.TextField(unlockAreaString, 25);
		if (GUILayout.Button("Unlock", GUILayout.Width(50))){
			UnlockArea(int.Parse(unlockAreaString));
		}
		GUILayout.EndHorizontal();

		if (!isClearArea){
			GUILayout.BeginHorizontal();

			for (int i = 1; i <= GameConstant.MAX_LEVEL; i ++){
				if (GUILayout.Button("Clear Area" + i, GUILayout.Width(100))){
					isClearArea = true;
					clearArea = i;
				}
			}
			GUILayout.EndHorizontal();
		}
		else{
			int from = (clearArea - 1) * 10 + 1;
			int to = clearArea * 10;
			GUILayout.BeginHorizontal();
			string prefix = string.Empty;
			for (int i = from; i <= from + 4; i++){
				string stageKey = clearArea + "_" + i;
				int status = PlayerPrefs.GetInt(stageKey, 0);
				if (status == 0){
					prefix = "Clear";
				}
				else{
					prefix = "Lock";
				}
				if (GUILayout.Button(prefix + " " + stageKey, GUILayout.Width(80))){
					if (status == 0){
						PlayerPrefs.SetInt(stageKey, 1);
						prefix = "Lock";
					}
					else{
						PlayerPrefs.SetInt(stageKey, 0);
						prefix = "Clear";
					}
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			
			for (int i = from + 5; i <= to; i++){
				string stageKey = clearArea + "_" + i;
				int status = PlayerPrefs.GetInt(stageKey, 0);
				if (status == 0){
					prefix = "Clear";
				}
				else{
					prefix = "Lock";
				}
				if (GUILayout.Button(prefix + " " + stageKey, GUILayout.Width(80))){
					if (status == 0){
						PlayerPrefs.SetInt(stageKey, 1);
						prefix = "Lock";
					}
					else{
						PlayerPrefs.SetInt(stageKey, 0);
						prefix = "Clear";
					}
				}
			}
			GUILayout.EndHorizontal();

			if (GUILayout.Button("Close", GUILayout.Width(80))){
				isClearArea = false;
			}

		}



	}

	private void UnlockArea(int unlockArea){
		if(unlockArea > GameConstant.MAX_LEVEL){
			unlockArea = GameConstant.MAX_LEVEL;
		}

		if (unlockArea == 0)
			return;

		for(int areaNum = 1; areaNum <= unlockArea - 1; areaNum ++){
			int from = (areaNum - 1) * 10 + 1;
			int to = areaNum * 10;
			for (int i = from; i <= to; i ++){
				PlayerPrefs.SetInt(areaNum + "_" + i, 1);
			}
			PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + areaNum, 1);
		}
		PlayerPrefs.SetInt(GameConstant.UNLOCK_PREFIX + "_" + unlockArea, 1);
	}


}
