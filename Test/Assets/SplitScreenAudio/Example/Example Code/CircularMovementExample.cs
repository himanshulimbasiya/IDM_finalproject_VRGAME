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

public class CircularMovementExample : MonoBehaviour {
	
	public Vector3 circleCenter = Vector3.zero;
	public float circleRadius = 5f;
	public float angularVelocity = 1f;
	private float angle = 0f;
	
	void Update () 
	{
		angle += Time.deltaTime*angularVelocity;
		transform.position = circleCenter + circleRadius*new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
	}
}
