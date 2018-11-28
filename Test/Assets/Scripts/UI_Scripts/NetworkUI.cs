using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkUI : NetworkBehaviour {

    public Camera playerCam;
    public GameObject flatlanderPrefab, monsterPrefab;
    public Canvas ui;


    public void SpawnFlatlander()
    {
        playerCam.enabled = false;
        ui.enabled = false;

        CmdSpawnFlatlander();
    }

    public void SpawMonster()
    {
        playerCam.enabled = false;
        ui.enabled = false;
        CmdSpawnMonster();
    }

    /// ///////////////////////////////// -------- COMMANDS



    [Command]
    void CmdSpawnFlatlander()
    {
       
        // do the stuff to spawn a flatlander and assign it to this player
        GameObject go = Instantiate(flatlanderPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);

        

    }

    [Command]
    void CmdSpawnMonster()
    {


        GameObject go = Instantiate(monsterPrefab);

        NetworkServer.SpawnWithClientAuthority(go, connectionToClient);
        //do stuff to spawn the monster
    }
}
