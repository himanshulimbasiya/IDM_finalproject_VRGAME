using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManagerMono : MonoBehaviour {

    //public variabels

    //TODO: change voluminium so it updates only when the value is changed.
    [SerializeField]
    [Range(0f, 100f)] float voluminium;

    [SerializeField]
    MonsterHandUI ui;

    [SerializeField]
    ControllerGrabObjectMono leftHand, rightHand;

    [SerializeField]
    MonsterNotificationController monsterNotification;

    public Light[] voluminiumLights = new Light[4];
    private int health = 3;

    List<FlatlanderUIMono> flatlanders = new List<FlatlanderUIMono>();

    

	// Use this for initialization
	void Start () {
        UpdateFlatlanders();
        UpdateUI();
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

    private void OnEnable()
    {
        EventManager.StartListening("Game Start", GameStart);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Game Start", GameStart);
    }


    void GameStart()
    {
        //Debug.Log("Game Start Running in Monster Manager.");
    }

    public void TakeDamage()
    {
        health--;
        HapticFeedbackTooBothHands(1000, 1f);
        if(health <= 0)
        {
            Debug.Log("Flatlander Win.");
        }
        UpdateFlatlanders();
        UpdateUI();
    }

    void UpdateFlatlanders()
    {
        foreach (FlatlanderUIMono flatlander in flatlanders)
        {
            flatlander.UpdateMonsterVoluminium(voluminium);
            flatlander.UpdateHealth(health);
        }
    }

    void UpdateUI()
    {
        if (ui != null)
        {
            ui.UpdateVoluminium(voluminium);
            ui.UpdateHealth(health);
        }
    }

    public void UpdateEmbedTime()
    {

        if (GameManagerMono.instance.IsCountingDown())
        {
            ui.UpdateEmbedTime(GameManagerMono.instance.GetCountdown());
            
        }
        else
            ui.UpdateEmbedTime(-1);
    }

    public void SetNotificationText(string message)
    {
        monsterNotification.SetText(message);
    }

    public void ChangeVoluminium(float change)
    {
        voluminium += change;
        if(voluminium < 0)
        {
            voluminium = 0;
        }

        UpdateUI();
        UpdateFlatlanders();
    }

    public void ResetToDefault()
    {
        health = 3;
        voluminium = 0;
        UpdateFlatlanders();
        UpdateUI();
    }

    public ControllerGrabObjectMono GetLeftHand()
    {
        return leftHand;
    }

    public ControllerGrabObjectMono GetRightHand()
    {
        return rightHand;
    }

    public void HapticFeedbackTooBothHands(ushort strength, float duration)
    {
        leftHand.HapticFeedback(strength, duration);
        rightHand.HapticFeedback(strength, duration);
    }


    public void CanGrabFeedback(float value)
    {

            ui.CanGrabFeedback(value);
        
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetVoluminium()
    {
        return voluminium;
    }

    public void AddToFlatlanders(FlatlanderUIMono flatlander)
    {
        flatlanders.Add(flatlander);
    }
}
