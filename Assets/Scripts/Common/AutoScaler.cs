using UnityEngine;
using System.Collections;
using System.Reflection;

public class AutoScaler : MonoBehaviour 
{
	/// <summary>
	/// Width of the screen, used when the scaling style is set to Flexible.
	/// </summary>
	
	public int manualWidth = 1280;
	
	/// <summary>
	/// Height of the screen when the scaling style is set to FixedSize or Flexible.
	/// </summary>
	
	public int manualHeight = 720;
	
	#if UNITY_EDITOR
	static int mSizeFrame = -1;
	static System.Reflection.MethodInfo s_GetSizeOfMainGameView;
	static Vector2 mGameSize = Vector2.one;
	
	/// <summary>
	/// Size of the game view cannot be retrieved from Screen.width and Screen.height when the game view is hidden.
	/// </summary>
	
	static public Vector2 screenSize
	{
		get
		{
			int frame = Time.frameCount;
			
			if (mSizeFrame != frame || !Application.isPlaying)
			{
				mSizeFrame = frame;
				
				if (s_GetSizeOfMainGameView == null)
				{
					System.Type type = System.Type.GetType ("UnityEditor.GameView,UnityEditor");
					s_GetSizeOfMainGameView = type.GetMethod ("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
				}

				mGameSize = (Vector2)s_GetSizeOfMainGameView.Invoke (null, null);
			}

			return mGameSize;
		}
	}
	#else

	/// <summary>
	/// Size of the game view cannot be retrieved from Screen.width and Screen.height when the game view is hidden.
	/// </summary>
	
	static public Vector2 screenSize 
	{ 
		get 
		{ 
            return new Vector2 (Screen.width, Screen.height); 
		} 
	}

	#endif

	public float scale
	{
		get
		{
			Vector2 screen = screenSize;
			float aspect = screen.x / screen.y;
			float initialAspect = (float)manualWidth / manualHeight;
			float scale = aspect /initialAspect;
			scale = scale < 1.0f ? scale : 1.0f;
			return scale;
		}
	}

	void Awake ()
	{
		transform.localScale = Vector3.one * scale;
	}
}
