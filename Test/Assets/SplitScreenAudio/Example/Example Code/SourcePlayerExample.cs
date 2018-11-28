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

public class SourcePlayerExample : MonoBehaviour {
	
	public VirtualAudioSource source;
	public ParticleSystem visualAid;
	
	private float originalVolume = 1/3f;
	private float originalPitch = 1f;
	
	void Update () 
	{
		
		if(source != null && source.mySource != null && !source.isPlaying)
		{
			source.StopAllCoroutines();//terminates coroutines associated with sources that have their type switched while playing
			
			source.mySource.volume = originalVolume; //this is necessary because the volume is manipulated during coroutines and may not reflect the original volume if the coroutine is cut off
			//In practice, where you'll likely never have a single clip be switching audio types, you can adjust the volume attributes just once in the inspector, and that will be fine.
			source.mySource.pitch = originalPitch; //this is necessary for the same reason as the volume, and is once again unnecessary if you do not switch audio types mid game.
			source.Play();
			visualAid.Emit(10);
		}
		//*/
		/*
		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			source.Play();
			visualAid.Emit(10);
		}
		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			source.PlayDelayed(2f);
			visualAid.Emit(10);
		}
		if(Input.GetKeyDown(KeyCode.Alpha3))
		{
			source.Pause();
		}
		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			source.Stop();
		}
		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			source.mySource.PlayDelayed(2f);
		}
		if(Input.GetKeyDown(KeyCode.Alpha6))
		{
			source.mySource.Play();
		}
		//*/
	}
	void OnEnable()
	{
		visualAid.Clear();
	}
}
