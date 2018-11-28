using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MonsterNotificationController : MonoBehaviour {
    [SerializeField]
    GameObject uiPanel;
    [SerializeField]
    Text textElement;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnGameSceneLoaded;
    }

    void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
        {
            DisablePanel();
        }
    }

    private void OnEnable()
    {
        EventManager.StartListening("Game Start", DisablePanel);
        EventManager.StartListening("Game End", EnablePanel);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Game Start", DisablePanel);
        EventManager.StopListening("Game End", EnablePanel);
    }

    private void OnDestroy()
    {
        EventManager.StopListening("Game Start", DisablePanel);
        EventManager.StopListening("Game End", EnablePanel);
    }

    void DisablePanel()
    {
        uiPanel.SetActive(false);
    }

    void EnablePanel()
    {
        uiPanel.SetActive(true);
    }

    public void SetText(string newText)
    {
        textElement.text = newText;
    }
}
