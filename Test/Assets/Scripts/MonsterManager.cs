using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterManager : NetworkBehaviour {

    //public variabels
    [SyncVar]
    [Range(0f, 100f)] public float voluminium;
    public Light[] voluminiumLights = new Light[4];
    private int health = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        foreach (Light light in voluminiumLights)
        {
            light.intensity = voluminium / 50f;
        }

        if (voluminium > 100)
            voluminium = 100;
    }



    [Command]
    void CmdTakeDamage()
    {
        health--;
        RpcTakeDamage(health);
    }

    [ClientRpc]
    void RpcTakeDamage(int currentHealth)
    {
        health = currentHealth;
    }

}
