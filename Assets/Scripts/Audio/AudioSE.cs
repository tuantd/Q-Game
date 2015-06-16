using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioSE : MonoBehaviour {

    [HideInInspector]
    public bool isVoice;

    public bool EnableCache {
        get { return enableCache; }
        set { enableCache = value; }
    }

    public bool AutoDestory {
        set { autoDestory = value; }
    }

    private bool  enableCache;
    private bool  autoDestory;
    private float volume;

	public delegate void EndHandler(string clipName);
	public event EndHandler endHandler = delegate{ };

    void Update() {
        if(GetComponent<AudioSource>().isPlaying) return;
        StopWithNotice();
    }

    public void Init(AudioClip clip, float volume) {
        Stop();
        GetComponent<AudioSource>().clip = clip;
        this.volume = volume;
    }

    public void Play(bool mute, float seVolume, float voiceVolume) {
        enabled = true;
        GetComponent<AudioSource>().enabled = true;
        GetComponent<AudioSource>().mute = mute;
        GetComponent<AudioSource>().volume = isVoice ? voiceVolume : seVolume * volume;
        GetComponent<AudioSource>().Play();
    }

    public void OnChangeVolume(bool mute, float seVolume, float voiceVolume) {
        GetComponent<AudioSource>().mute = mute;
        GetComponent<AudioSource>().volume = isVoice ? voiceVolume : seVolume * volume;
    }

    public void Stop() {
        enabled = false;
        GetComponent<AudioSource>().Stop();
        GetComponent<AudioSource>().enabled = false;
    }

    public void StopWithNotice() {
        Stop();
        AudioManager.Instance.OnStopSE(this);
		if(GetComponent<AudioSource>().clip != null)
			endHandler(GetComponent<AudioSource>().clip.name);
        if(autoDestory) Destroy(gameObject);
    }
}
