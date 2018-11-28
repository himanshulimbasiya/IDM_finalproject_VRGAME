using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControllerObject : NetworkBehaviour {

    public Camera playerCam;
    public GameObject flatlanderPrefab,
        monsterPrefab;

    enum selection { monster, flatlander, none }

    GameObject localAvatar;
    bool hasMonsterPlayer = false;


	// Use this for initialization
	void Start () {
        //playerCam = Camera.main;

        if (isLocalPlayer == false)
            return;
        
	}


	
	// Update is called once per frame
	void Update () {


        //past this, we are on local player only
        if (!isLocalPlayer)
            return;

	}

    [Command]
    void CmdSpawnFlatlander()
    {
        playerCam.enabled = false;
        // do the stuff to spawn a flatlander and assign it to this player
        GameObject go = Instantiate(flatlanderPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        
    }


    [Command]
    void CmdSpawnMonster()
    {
        //do stuff to spawn the monster
        GameObject go = Instantiate(monsterPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
    }

    /// ///////////////////////////////// -------- RCP
    /// 


}
