using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GlobalBroadcastUI : MonoBehaviour {

    [SerializeField]
    Text message;
    [SerializeField]
    Button firstFocus;
    [SerializeField]
    GameObject playAgain, quit, panel;

    private void OnEnable()
    {
        EventManager.StartListening("Game Start", DisablePanel);
        EventManager.StartListening("Game End", GameEnd);
        StartCoroutine(SelectContinueButtonLater());
    }

    IEnumerator SelectContinueButtonLater()
    {
        yield return null;
        firstFocus.Select();
        firstFocus.OnSelect(null);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Game Start", DisablePanel);
        EventManager.StopListening("Game End", GameEnd);
    }

    void GameEnd()
    {
        EnableInterface(true);
        quit.SetActive(false);
        playAgain.SetActive(false);
        StartCoroutine(WaitForMessage());
        
    }

    public void EnableButtons(bool value)
    {
        quit.SetActive(value);
        playAgain.SetActive(value);
    }

    public void EnableInterface(bool value)
    {
        panel.SetActive(value);
    }

    public void ChangeText(string newText)
    {
        message.text = newText;
    }

    void DisablePanel()
    {
        panel.SetActive(false);
    }

    public void LoadScene(int index)
    {
        EventManager.TriggerEvent("Paused");
        GameManagerMono.instance.LoadScene(index);
        
    }

    public void PlayAgain()
    {
        EventManager.TriggerEvent("Paused");
        Time.timeScale = 1;
        GameManagerMono.instance.GameStart();
        
    }

    IEnumerator WaitForMessage()
    {
        int count = 0;
        while (count < 100)
        {
            count++;
            yield return null;
        }

        playAgain.SetActive(true);
        quit.SetActive(true);
        StartCoroutine(SelectContinueButtonLater());
    }

}
