using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;
    public float snatchDivisor;

    [SerializeField]
    Vector3 origin = Vector3.zero;
    [SerializeField]
    float radius;
    [SerializeField]
    float cameraDistance;
    [SerializeField]
    int playerCount;


	// Use this for initialization
	void Awake () {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else if(GameManager.instance != this)
        {
            Debug.Log("Copy Found. Destroying Instance.");
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        //TODO: Move this to somewhere sensible.
        FindPlayerCount();
	}
	
	// Update is called once per frame
	void Update () {
        CheckWin();
	}

    void GameStart()
    {
        //do the game start stuff
        //TODO: build a proper intro scene
        FindPlayerCount();
    }

    void CheckWin()
    {

    }

    public void FindPlayerCount()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        playerCount = gos.Length;
        Debug.Log("Player count:" + playerCount);
    }

    public float GetRadius()
    {
        return radius;
    }

    public float GetCameraRadius()
    {
        return cameraDistance;
    }

    public Vector3 GetOrigin()
    {
        return origin;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }
}
