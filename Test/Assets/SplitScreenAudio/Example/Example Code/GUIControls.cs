/************************************************************
 * Created in 2014 by:  LunaArgenteus
 * This software is not free. If you acquired this code without paying for it, please consider supporting me
 * by purchasing it on the Unity Asset Store to help me continue creating awesome stuff!
 * 
 * If you have any questions that are not answered by the readme, you can ask on the official support thread on Unity forums, 
 * forum.unity3d.com/threads/273344/
 * send me a private message, or can email me at LunaArgenteus@gmail.com (Please include the name of this product (Split Screen Audio) in the subject line or I may not respond),
 * but please consult the readme first!
 ************************************************************
 */
using UnityEngine;
using System.Collections;

public class GUIControls : MonoBehaviour {
	
	public ExampleSourceContainer source1;
	public ExampleSourceContainer source2;
	public ExampleSourceContainer source3;
	
	public VirtualAudioListener listener1;
	public VirtualAudioListener listener2;
	public VirtualAudioListener listener3;
	
	public Camera cam1;
	public Camera cam2;
	public Camera cam3;
	
	private bool displayingHelp = false;
	
	private Vector2 scrollPos;
	
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			displayingHelp = !displayingHelp;
			source1.displayingHelp = displayingHelp;
			source2.displayingHelp = displayingHelp;
			source3.displayingHelp = displayingHelp;
		}
	}
	void OnGUI()
	{
		
		
		if(displayingHelp)
		{
			GUI.Label(new Rect(15, 0, 300, 20), "Press esc to restore controls . . .");
			
			GUILayout.BeginArea(new Rect(15,15, Screen.width - 30, Screen.height - 30));
			
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			
			GUI.skin.box.wordWrap = true;
			GUI.skin.box.padding= new RectOffset(15,15,15,15);
			GUI.skin.box.alignment = TextAnchor.UpperLeft;
			GUILayout.Box(
				"There are 3 available sample players and 3 available sample sounds to toggle. There are 3 cameras to represent different players, with a 4th camera providing a top-down view to use as a reference to see how the sounds interact with the listeners. " +
				"Each sound can be played using one of 3 modes. " +
				"\n\n2D Panning - Each listener is assigned a value between -1 and 1, " +
				"which represents how far to the left or right the sound should be played when heard by that listener. This type has it's own rolloff settings to fake the likeness of a 3d sound. " +
				"In this example, Player 1 (Blue) is assigned the value of -1, while Players 2 (Green) and 3 (Red) are assigned values of 1. Doppler settings are also available. In this example, a mild amount of doppler effect is used." +
				"\n\nClosest Listener - The sound is played using 3D settings for whichever listener is closest. It can be set to allow the listener to switch mid clip, or lock the sound so once it starts playing for a listener, the clip finishes playing for that listener. " +
				"In this example, the clips are locked to whatever listener they start playing for." +
				"\n\nMulti Listeners - The sound is played for each listener and the results are combined with the volume adjusted to account for the layering of the audio clips." +
				"\n\nThe speed buttons allow you to change the speed of the moving sounds." +
				"\nTry experimenting with which players are active and what sounds are playing to feel out what each mode sounds like under different circumstances!");
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}
		else
		{
			GUI.Label(new Rect(15, 0, 300, 20), "Press esc to display help . . .");
		
			source1.gameObject.SetActive(GUI.Toggle(new Rect(15, 15, 80, 20), source1.gameObject.activeInHierarchy, " Source 1"));
			source2.gameObject.SetActive(GUI.Toggle(new Rect(15, 35, 80, 20), source2.gameObject.activeInHierarchy, " Source 2"));
			source3.gameObject.SetActive(GUI.Toggle(new Rect(15, 55, 80, 20), source3.gameObject.activeInHierarchy, " Source 3"));
			
			listener1.gameObject.SetActive(GUI.Toggle(new Rect(15, 90, 80, 20), listener1.gameObject.activeInHierarchy, " Player 1"));
			listener2.gameObject.SetActive(GUI.Toggle(new Rect(15, 110, 80, 20), listener2.gameObject.activeInHierarchy, " Player 2"));
			listener3.gameObject.SetActive(GUI.Toggle(new Rect(15, 130, 80, 20), listener3.gameObject.activeInHierarchy, " Player 3"));
			
			if(listener1.gameObject.activeInHierarchy)
			{
				if(listener2.gameObject.activeInHierarchy)
				{
					if(listener3.gameObject.activeInHierarchy)
					{
						cam1.rect = new Rect(0, 0.501f, 0.499f, 0.499f);
						cam2.rect = new Rect(0.501f, 0.501f, 0.499f, 0.499f);
						cam3.rect = new Rect(0.501f, 0f, 0.499f, 0.499f);
					}
					else
					{
						cam1.rect = new Rect(0, 0, 0.499f, 1);
						cam2.rect = new Rect(0.501f, 0, 0.499f, 1);
					}
				}
				else if(listener3.gameObject.activeInHierarchy)
				{
					cam1.rect = new Rect(0, 0, 0.499f, 1);
					cam3.rect = new Rect(0.501f, 0, 0.499f, 1);
				}
				else
				{
					cam1.rect = new Rect(0, 0, 1, 1);
				}
			}
			else if(listener2.gameObject.activeInHierarchy)
			{
				if(listener3.gameObject.activeInHierarchy)
				{
					cam2.rect = new Rect(0, 0, 0.499f, 1);
					cam3.rect = new Rect(0.501f, 0, 0.499f, 1);
				}
				else
				{
					cam2.rect = new Rect(0, 0, 1, 1);
				}
			}
			else
			{
				cam3.rect = new Rect(0, 0, 1, 1);
			}
		}
	}
}
