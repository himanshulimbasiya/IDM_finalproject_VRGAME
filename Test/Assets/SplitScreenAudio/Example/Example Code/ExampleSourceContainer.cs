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

public class ExampleSourceContainer : MonoBehaviour {
	
	public VirtualAudioSource_PanByListenerIndex sourcePan;
	public VirtualAudioSource_ClosestListenerOnly sourceClosest;
	public VirtualAudioSource_NormalizedMultiSources sourceMulti;
	
	public CircularMovementExample movement;
	
	public int guiYBegin = 15;
	
	[HideInInspector]
	public bool displayingHelp = false;
	
	void OnGUI()
	{
		if(!displayingHelp)
		{
			if(GUI.Toggle(new Rect(100, guiYBegin, 100, 20), sourcePan.gameObject.activeInHierarchy, "2D Panning"))
			{
				sourceClosest.gameObject.SetActive(false);
				sourceMulti.gameObject.SetActive(false);
				sourcePan.gameObject.SetActive(true);
				
				sourceMulti.CleanUpDuplicateSources();
			}
			if(GUI.Toggle(new Rect(200, guiYBegin, 130, 20), sourceClosest.gameObject.activeInHierarchy, "Closest Listener"))
			{
				sourcePan.gameObject.SetActive(false);
				sourceMulti.gameObject.SetActive(false);
				sourceClosest.gameObject.SetActive(true);
				
				sourceMulti.CleanUpDuplicateSources();
				
			}
			if(GUI.Toggle(new Rect(330, guiYBegin, 100, 20), sourceMulti.gameObject.activeInHierarchy, "Multi Listeners"))
			{
				sourcePan.gameObject.SetActive(false);
				sourceClosest.gameObject.SetActive(false);
				sourceMulti.gameObject.SetActive(true);
			}
			if(GUI.Button(new Rect(440, guiYBegin, 70, 20), "Speed +"))
			{
				movement.angularVelocity += 0.05f;
			}
			if(GUI.Button(new Rect(515, guiYBegin, 70, 20), "Speed -"))
			{
				movement.angularVelocity -= 0.05f;
			}
		}
	}
}
