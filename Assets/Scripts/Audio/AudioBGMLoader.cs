using UnityEngine;
using System.Collections;

public class AudioBGMLoader : MonoBehaviour {

    public AudioClip clip;

    void OnEnable() {
        AudioManager.Instance.PlayBGM(clip);
    }

	public void Play() {
		AudioManager.Instance.PlayBGM(clip);
	}
}
