using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance = null;
	[Tooltip( "Anytime you play a sound and set the scaledVolume it is multiplied by this value" )]
	public float soundEffectVolume = 1f;
	public int initialCapacity = 10;
	public int maxCapacity = 15;
	public bool dontDestroyOnLoad = true;
	public bool clearAllAudioClipsOnLevelLoad = true;
	[NonSerialized]
	public SoundController backgroundSound;

	public static int IsSoundOn = 0;

	private Stack<SoundController> _availableSounds;
	private List<SoundController>  _playingSounds;
	
	#region MonoBehaviour

	void Awake ()
	{
		// Avoid duplicates
		if (instance != null)
		{
			// We set dontDestroyOnLoad to false here due to the Destroy being delayed. it will avoid issues
			// with OnLevelWasLoaded being called while the object is being destroyed.
			dontDestroyOnLoad = false;
			Destroy (gameObject);
			return;
		}

		instance = this;

		if (dontDestroyOnLoad)
		{
			DontDestroyOnLoad (gameObject);
		}

		// Create the _soundList to speed up sound playing in game
		_availableSounds = new Stack<SoundController> (maxCapacity);
		_playingSounds   = new List<SoundController> ();

		for (int i = 0; i < initialCapacity; i++)
		{
			_availableSounds.Push (new SoundController (this));
		}
	}
	
	void OnApplicationQuit ()
	{
		instance = null;
	}

	void OnLevelWasLoaded (int level)
	{
		if (dontDestroyOnLoad && clearAllAudioClipsOnLevelLoad)
		{
			for (var i = _playingSounds.Count - 1; i >= 0; i--)
			{
				var s = _playingSounds [i];
				s.audioSource.clip = null;

				_availableSounds.Push (s);
				_playingSounds.RemoveAt (i);
			}
		}
	}
	
	void Update ()
	{
		for (var i = _playingSounds.Count - 1; i >= 0; i--)
		{
			var sound = _playingSounds [i];

			if (sound._playingLoopingAudio)
			{
				continue;
			}

			sound._elapsedTime += Time.deltaTime;

			if (sound._elapsedTime > sound.audioSource.clip.length)
			{
				sound.Stop ();
			}
		}
	}

	#endregion

	/// <summary>
	/// Fetches the next available sound and adds the sound to the _playingSounds List
	/// </summary>
	/// <returns>The available sound.</returns>

	private SoundController NextAvailableSound ()
	{
		SoundController sound = null;

		if (_availableSounds.Count > 0)
		{
			sound = _availableSounds.Pop ();
		}

		// if we didnt find an available found, bail out
		if (sound == null)
		{
			sound = new SoundController (this);
		}

		_playingSounds.Add (sound);

		return sound;
	}
	
	/// <summary>
	/// Starts up the background music and optionally loops it. You can access the SoundController via the backgroundSound field.
	/// </summary>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="loop">If set to <c>true</c> loop.</param>

	public void PlayBackgroundMusic (AudioClip audioClip, float volume, bool loop = true)
	{
		if (backgroundSound == null)
		{
			backgroundSound = new SoundController (this);
		}

		backgroundSound.PlayAudioClip (audioClip, volume, 1.0f);
		backgroundSound.SetLoop (loop);
	}

	/// <summary>
	/// Stop the background music. If you dont want to music background play continue when level load 
	/// and want to replay it. Call this function before load level.
	/// </summary>

	public void StopBackgroundMusic ()
	{
		backgroundSound.Reset ();
	}

	/// <summary>
	/// Pause the background music. In case pause game or something you want to pause background music.
	/// </summary>

	public void PauseBackgroundMusic ()
	{
		backgroundSound.Pause ();
	}

	/// <summary>
	/// Resume the background music after pause it before.
	/// </summary>
	
	public void ResumeBackgroundMusic ()
	{
		backgroundSound.Resume ();
	}

	/// <summary>
	/// Fetches any AudioSource it can find and uses the standard PlayOneShot to play. Use this if you don't require any
	/// extra control over a clip and don't care about when it completes. It avoids the call to StartCoroutine.
	/// </summary>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="volumeScale">Volume scale.</param>

	public void PlayOneShot (AudioClip audioClip, float volumeScale = 1.0f)
	{
		// Find an audio source. any will work
		AudioSource source = null;

		if (_availableSounds.Count > 0)
		{
			source = _availableSounds.Peek ().audioSource;
		}
		else
		{
			source = _playingSounds [0].audioSource;
		}

		source.PlayOneShot (audioClip, volumeScale * soundEffectVolume);
	}

	/// <summary>
	/// Plays the AudioClip with the default volume (soundEffectVolume)
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="audioClip">Audio clip.</param>

	public SoundController PlaySound (AudioClip audioClip)
	{
		return PlaySound (audioClip, 1.0f);
	}
	
	/// <summary>
	/// Plays the AudioClip with the specified volume
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="volume">Volume.</param>

	public SoundController PlaySound (AudioClip audioClip, float volume)
	{
		return PlaySound (audioClip, volume, 1.0f);
	}

	/// <summary>
	/// Plays the AudioClip with the specified pitch
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="pitch">Pitch.</param>

	public SoundController PlayPitchedSound (AudioClip audioClip, float pitch)
	{
		return PlaySound (audioClip, 1.0f, pitch);
	}
	
	/// <summary>
	/// Plays the AudioClip with the specified volumeScale and pitch
	/// </summary>
	/// <returns>The sound.</returns>
	/// <param name="audioClip">Audio clip.</param>
	/// <param name="volume">Volume.</param>

	public SoundController PlaySound (AudioClip audioClip, float volumeScale, float pitch)
	{
		// Find the first SoundController not being used. if they are all in use, create a new one
		SoundController sound = NextAvailableSound ();
		sound.PlayAudioClip (audioClip, volumeScale * soundEffectVolume, pitch);

		return sound;
	}

	/// <summary>
	/// Loops the AudioClip. Do note that you are responsible for calling either stop or fadeOutAndStop on the SoundController
	/// or it will not be recycled
	/// </summary>
	/// <returns>The sound looped.</returns>
	/// <param name="audioClip">Audio clip.</param>
	public SoundController PlaySoundLooped (AudioClip audioClip)
	{
		// Find the first SoundController not being used. if they are all in use, create a new one
		SoundController sound = NextAvailableSound ();
		sound.PlayAudioClip (audioClip, soundEffectVolume, 1.0f);
		sound.SetLoop (true);

		return sound;
	}
	
	/// <summary>
	/// Used internally to recycle SoundControllers and their AudioSources
	/// </summary>
	/// <param name="sound">Sound.</param>
	public void RecycleSound (SoundController sound)
	{
		var index = 0;
		while (index < _playingSounds.Count)
		{
			if (_playingSounds[index] == sound)
			{
				break;
			}

			index++;
		}

		_playingSounds.RemoveAt (index);


		// If we are already over capacity dont recycle this sound but destroy it instead
		if (_availableSounds.Count + _playingSounds.Count >= maxCapacity)
		{
			Destroy (sound.audioSource);
		}
		else
		{
			_availableSounds.Push (sound);
		}
	}
	
	#region SoundSource inner class

	public class SoundController
	{
		private SoundManager _manager;

		public AudioSource audioSource;
		internal Action _completionHandler;
		internal bool _playingLoopingAudio;
		internal float _elapsedTime;

		public SoundController (SoundManager manager)
		{
			_manager = manager;
			audioSource = _manager.gameObject.AddComponent<AudioSource> ();
			audioSource.playOnAwake = false;
		}

		/// <summary>
		/// Fades out the audio over duration. this will short circuit and stop immediately if the elapsedTime exceeds the clip.length
		/// </summary>
		/// <returns>The out.</returns>
		/// <param name="duration">Duration.</param>
		/// <param name="onComplete">On complete.</param>

		private IEnumerator FadeOut (float duration, Action onComplete)
		{
			var startingVolume = audioSource.volume;

			// fade out the volume
			while (audioSource.volume > 0.0f && _elapsedTime < audioSource.clip.length)
			{
				audioSource.volume -= Time.deltaTime * startingVolume / duration;
				yield return null;
			}

			Stop ();

			// all done fading out
			if (onComplete != null)
			{
				onComplete ();
			}
		}

		/// <summary>
		/// Sets whether the SoundController should loop. If true, you are responsible for calling stop on the SoundController to recycle it!
		/// </summary>
		/// <returns>The SoundController.</returns>
		/// <param name="shouldLoop">If set to <c>true</c> should loop.</param>

		public SoundController SetLoop (bool shouldLoop)
		{
			_playingLoopingAudio = true;
			audioSource.loop     = shouldLoop;

			return this;
		}

		/// <summary>
		/// Sets an Action that will be called when the clip finishes playing
		/// </summary>
		/// <returns>The SoundController.</returns>
		/// <param name="handler">Handler.</param>

		public SoundController SetCompletionHandler (Action handler)
		{
			_completionHandler = handler;

			return this;
		}

		public void Reset ()
		{
			audioSource.Stop ();
		}

		public void Pause ()
		{
			audioSource.Pause ();
		}

		public void Resume ()
		{
			audioSource.Play ();
		}

		/// <summary>
		/// Stops the audio clip, fires the completionHandler if necessary and recycles the SoundController
		/// </summary>

		public void Stop ()
		{
			audioSource.Stop ();

			if (_completionHandler != null)
			{
				_completionHandler ();
				_completionHandler = null;
			}

			_manager.RecycleSound (this);
		}

		/// <summary>
		/// fades out the audio clip over time. Note that if the clip finishes before the fade completes it will short circuit
		/// the fade and stop playing
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="handler">Handler.</param>

		public void FadeOutAndStop (float duration, Action handler = null)
		{
			_manager.StartCoroutine
			(
				FadeOut (duration, () =>
			    {
					if (handler != null)
					{
						handler ();
					}
				})
			);
		}

		internal void PlayAudioClip (AudioClip audioClip, float volume, float pitch)
		{
			_playingLoopingAudio = false;
			_elapsedTime         = 0;

			// setup the GameObject and AudioSource and start playing
			audioSource.clip     = audioClip;
			audioSource.volume   = volume;
			audioSource.pitch    = pitch;

			// reset some defaults in case the AudioSource was changed
			audioSource.loop     = false;
			audioSource.pan      = 0;
			audioSource.mute     = false;

			audioSource.Play ();
		}
	}

	#endregion
}
