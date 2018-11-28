using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerMono : MonoBehaviour {

    public static GameManagerMono instance;
    public float snatchDivisor;

    [SerializeField] Vector3 origin = Vector3.zero;
    [SerializeField] float radius;
    [SerializeField] float cameraDistance, hypersquareMax = 4;
    [SerializeField] [Range(1, 4)] int playerCount;
    [SerializeField] MonsterManagerMono mm;
    [SerializeField] GameObject flatlanderPrefab, hypersquarePrefab, voluminiumShardPrefab, voluminiumSpawn;
    [SerializeField] GameObject[] spawnPoints;
    [SerializeField] LayerMask whereToSpawn;
    [SerializeField] AudioClip embedClip;
    

    public GlobalBroadcastUI broadcastUI;

    List<GameObject> flatlanders = new List<GameObject>();
    List<GameObject> hypersquares = new List<GameObject>(), embeddedHypersquares = new List<GameObject>();

    bool gameOngoing = false, isCountingDown = false, gamePaused = false;
    float embedTimeLeft;


	// Use this for initialization
	void Awake () {
        if (GameManagerMono.instance == null)
        {
            GameManagerMono.instance = this;
        }
        else if(GameManagerMono.instance != this)
        {
            Debug.Log("Copy Found. Destroying Instance.");
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnGameSceneLoaded;

        //TODO: Move this to somewhere sensible. Needs to trigger ON GAME START, not when the object is created.


    }

    


    // Update is called once per frame
    void Update () {

        if (gameOngoing)
        {
            CalculateVoluminiumIncrease();
            CheckWin();
        }


	}

    private void OnEnable()
    {
        //EventManager.StartListening("Paused", GamePaused);
    }

    private void OnDisable()
    {
        //EventManager.StopListening("Paused", GamePaused);
    }

    public void GameStart()
    {
        if (gameOngoing)
            ResetVariables();


        Debug.Log("GameStart() running.");

        EventManager.StartListening("Paused", GamePaused);
        Time.timeScale = 1;
        //do the game start stuff
        gameOngoing = true;
        gamePaused = false;
        FindSpawnPoints();

        //TODO: build a proper intro scene
        FindPlayers();
        
        //SpawnPlayers();
        ResetFlatlanders();

        mm.ResetToDefault();

        FindHypersquares();
        ResetHypersquares();

        FlatlandersInteracable(true);

        SpawnHypersquares();
        EventManager.TriggerEvent("Game Start");

        
        StartCoroutine(CountDownToAutoEmbed());

        //if(broadcastUI != null)
        //    broadcastUI.EnableInterface(false);

    }

    void ResetVariables()
    {
        gameOngoing = false;
        gamePaused = false;
        flatlanders.Clear();
        hypersquares.Clear();
        isCountingDown = false;
        DestroyEmbeddedHypersquares();
        FlatlandersInteracable(false);
        DestroyLooseShards();
        StopAllCoroutines();
        mm.GetLeftHand().StopAllCoroutines();
        mm.GetRightHand().StopAllCoroutines();
        EventManager.StopListening("Paused", GamePaused);
    }

    void GameEnd()
    {
        gameOngoing = false;
        DestroyEmbeddedHypersquares();
        DestroyLooseShards();
        StopAllCoroutines();
        FlatlandersInteracable(false);
        EventManager.StopListening("Paused", GamePaused);
        EventManager.TriggerEvent("Game End");
    }

    void GamePaused()
    {


        if (gamePaused)
        {
            Debug.Log("Game Unpaused.");
            gamePaused = false;
            Time.timeScale = 1;
            FlatlandersInteracable(true);
            broadcastUI.EnableInterface(false);
            broadcastUI.EnableButtons(false);
            broadcastUI.ChangeText("Paused");
        }
        else
        {
            Debug.Log("Game paused.");
            gamePaused = true;
            Time.timeScale = 0;
            FlatlandersInteracable(false);
            broadcastUI.EnableInterface(true);
            broadcastUI.EnableButtons(true);
            broadcastUI.ChangeText("Paused");
        }
    }

    void CalculateVoluminiumIncrease()
    {
        if (embeddedHypersquares.Count == 0)
            return;
        float increase;

        increase = Mathf.Log(embeddedHypersquares.Count * 2, 2.2f);
        //Debug.Log("Embedded Cubes: "+ embeddedHypersquares.Count + ". Rate: "+ increase);

        mm.ChangeVoluminium(increase*Time.deltaTime);
    }

    private void ResetFlatlanders()
    {
        foreach(GameObject flatlander in flatlanders)
        {
            FlatlanderControllerMono fc = flatlander.GetComponent<FlatlanderControllerMono>();

            
            flatlander.transform.position = spawnPoints[(int)fc.GetIndex()].transform.position;
            fc.ResetFlatlander();
        }
    }

    void FindSpawnPoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "GameScene Split Screen")
        {
            if (broadcastUI == null)
            {
                broadcastUI = FindObjectOfType<GlobalBroadcastUI>();

                if(broadcastUI != null)
                    broadcastUI.EnableInterface(false);
            }

            if (mm == null)
                mm = FindObjectOfType<MonsterManagerMono>();

            FindSpawnPoints();
            GameStart();
        }
    }

    void FindHypersquares()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Hypersquare");

        foreach(GameObject hypersquare in gos) 
        {
            if (hypersquares.Contains(hypersquare))
                return;
            hypersquares.Add(hypersquare);
        }
    }

    void ResetHypersquares()
    {
        if (hypersquares.Count == 0)
            return;

        foreach(GameObject hypersquare in hypersquares)
        {
            //SUPER SLOW

            Destroy(hypersquare);
        }

        hypersquares.Clear();
    }

    void SpawnHypersquares()
    {
        while(hypersquares.Count < hypersquareMax)
        {
            SpawnHypersquare();
        }
    }

    void SpawnHypersquare()
    {
            GameObject newHypersquare = Instantiate(hypersquarePrefab, FindSpawnNewPosition(), Quaternion.identity);
            hypersquares.Add(newHypersquare);
    }

    public void AddEmbeddedHypersquare(GameObject hypersquare)
    {
        hypersquares.Remove(hypersquare);
        SpawnHypersquares();
        embeddedHypersquares.Add(hypersquare);

        SoundManager.instance.HypersquareEmbedded(true);
    }

    public void RemoveEmbeddedHypersquare(GameObject hypersquare)
    {
        embeddedHypersquares.Remove(hypersquare);
        if(embeddedHypersquares.Count == 0)
        {
            StartCoroutine(CountDownToAutoEmbed());
            SoundManager.instance.HypersquareEmbedded(false);
        }
        Debug.Log("Embedded Hypersquares: "+ embeddedHypersquares.Count);

        
    }

    public void RemoveHypersquare(GameObject squareToRemove)
    {
        hypersquares.Remove(squareToRemove);

        SpawnHypersquares();
    }

    void AutoEmbedHypersquare()
    {
        //Debug.Log("AutoEmbedding.");
        bool acquiredTarget = false;
        FindHypersquares();

        while (!acquiredTarget) { 
            VR2DInterfaceMono vri = hypersquares[Random.Range(0,3)].GetComponent<VR2DInterfaceMono>();


            if (!vri.isHeld)
            {
                HypersquareControllerMono hype = vri.GetComponent<HypersquareControllerMono>();
                vri.isInteractable = false;
                hype.rb.isKinematic = true;
                hype.embeddingParticles.Play();

                SoundManager.instance.HypersquarePlayClip(embedClip, .5f);
                Vector3 pos = spawnPoints[Random.Range(0, 3)].transform.position;

                hype.AutoEmbed(pos);
                acquiredTarget = true;
            }
        }
    }

    void FlatlandersInteracable(bool value)
    {
        foreach(GameObject flatlander in flatlanders)
        {

                FlatlanderControllerMono fc = flatlander.GetComponent<FlatlanderControllerMono>();

                fc.isInteractable = value;
            
        }
    }



    void CheckWin()
    {
        if (!gameOngoing)
            return;

        if (mm.GetVoluminium() >= 100)
        {
            MonsterWin();
            
        }
        else if(mm.GetHealth() == 0)
        {
            FlatlanderWin();
            
        }
    }

    void MonsterWin()
    {
        string globalText = "Monster Wins!";
        GameEnd();

        //monster Wins
        Time.timeScale = 0;
        broadcastUI.EnableInterface(true);
        broadcastUI.ChangeText(globalText);
        mm.SetNotificationText(globalText);
        
    }

    void FlatlanderWin()
    {
        string globalText = "Flatlanders Win!";
        GameEnd();

        //flatlanders win
        Time.timeScale = 0;
        broadcastUI.EnableInterface(true);
        broadcastUI.ChangeText(globalText);
        mm.SetNotificationText(globalText);

        
    }

    void DestroyEmbeddedHypersquares()
    {
        foreach(GameObject hypersquare in embeddedHypersquares)
        {
            
            Destroy(hypersquare);
        }

        embeddedHypersquares.Clear();

        SoundManager.instance.HypersquareEmbedded(false);
    }

    void DestroyLooseShards()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Shard");

        foreach(GameObject shard in gos)
        {
            Destroy(shard);
        }
    }

    void FindPlayers()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Player");
        //playerCount = gos.Length;
        flatlanders.Clear();

        foreach(GameObject flatlander in gos)
        {
            if (flatlanders.Contains(flatlander))
                return;

            flatlanders.Add(flatlander);
        }


            foreach(GameObject flatlander in gos)
            {
                if((int)flatlander.GetComponent<FlatlanderControllerMono>().GetIndex() > playerCount -1)
                {
                    flatlander.SetActive(false);
                }
            }
        
    }

    void ResetPMPs()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("PMP");
        foreach(GameObject go in gos)
        {
            PMPLoader pmp = go.GetComponent<PMPLoader>();
            pmp.ResetLoader();
        }
    }

    public void SpawnShard(Vector3 pos)
    {
        StartCoroutine(ShardSpawning(pos));
    }

    IEnumerator ShardSpawning(Vector3 pos)
    {
        GameObject go = Instantiate(voluminiumSpawn, pos, Quaternion.identity);
        float timeleft = 1f;

       
        
        while(timeleft > 0f)
        {
            timeleft -= Time.deltaTime;
            yield return null;
        }

        Instantiate(voluminiumShardPrefab, pos, Quaternion.identity);
        

        Destroy(go);
    }

    IEnumerator CountDownToAutoEmbed()
    {
        isCountingDown = true;
        embedTimeLeft = 15f;


        while(embeddedHypersquares.Count == 0 && embedTimeLeft > 0)
        {

            embedTimeLeft -= Time.deltaTime;
            mm.UpdateEmbedTime();

            if(embedTimeLeft < 5f)
            {

                mm.GetLeftHand().HapticFeedback(500);
            }
            yield return null;
        }

        if(embeddedHypersquares.Count == 0 && embedTimeLeft <= 0)
        {
            AutoEmbedHypersquare();
        }

        isCountingDown = false;
        mm.UpdateEmbedTime();
    }

    Vector3 FindSpawnNewPosition()
    {
        Vector3 newPos;
        Collider[] neighbours;

        do
        {
            newPos = new Vector3(Random.Range(-20, 20), 40, Random.Range(-20, 20));

            neighbours = Physics.OverlapBox(newPos, Vector3.one * 1.5f, Quaternion.identity, whereToSpawn);

        } while (neighbours.Length > 0);

        return newPos;
    }

    public Transform GetClosestHypersquare(Vector3 position)
    {
        if (embeddedHypersquares.Count == 0)
            return null;

        Transform closest = transform;
        float closestDistance = float.MaxValue;

        foreach(GameObject hypersquare in embeddedHypersquares)
        {
            Vector3 heading = hypersquare.transform.position - position;

            if (closestDistance > heading.sqrMagnitude)
            {
                closestDistance = heading.sqrMagnitude;
                closest = hypersquare.transform;
            }
        }

            return closest.transform;

    }

    public void LoadScene(int index)
    {
        ResetVariables();
        SceneManager.LoadScene(index);
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

    public List<GameObject> GetFlatalnders()
    {
        return flatlanders;
    }

    public void SetPlayerCount(int count)
    {
        playerCount = count;
    }

    public int GetPlayerCount()
    {
        return playerCount;
    }

    public float GetCountdown()
    {
        return embedTimeLeft;
    }

    public void SetGameOngoing(bool value)
    {
        gameOngoing = value;
    }

    public bool IsCountingDown()
    {
        return isCountingDown;
    }
}
