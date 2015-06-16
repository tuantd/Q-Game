using UnityEngine;
using System.Collections;

public class AudioSELoader : MonoBehaviour {

    public enum InstanceType {
        Child,
        Sibiling,
    }

    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    public bool stopOnDisable;
    public bool enablePositionEffect;
    public InstanceType type;
    public GameObject target;
    [HideInInspector]
    public bool isVoice;
    private AudioSE se;
	
    void Awake() {
        if(!enablePositionEffect) return;

        se = GameObjectUtility.AddChild<AudioSE>("SE", target);
        if(type == InstanceType.Sibiling) se.transform.parent = target.transform.parent;
        
        se.EnableCache = false;
        se.Init(clip, volume);
        se.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
        se.isVoice = isVoice;
    }

    void OnEnable() {
        //TODO: long@oneofthem.jp
        // Not play sound after being loaded.
//        Play();
    }

    void OnDisable() {
        if(!stopOnDisable) return;
        se.StopWithNotice();
    }

    void OnDestory() {
        if(!enablePositionEffect) return;
        
        if(se.GetComponent<AudioSource>().isPlaying) se.AutoDestory = true;
        else Destroy(se.gameObject);
    }

	public void Play() {
		if (enablePositionEffect) {
            AudioManager.Instance.PlaySE(se);
        }
		else{ 
            se = AudioManager.Instance.PlaySE(clip, volume);
        }
	}
}
