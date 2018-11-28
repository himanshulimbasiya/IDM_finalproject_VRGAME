using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {


    public void LoadScene(int index){
        Debug.Log("Loading Scene with index:"+ index);
        SceneManager.LoadScene(index);
        }

    public void PlayAgain()
    {
        Time.timeScale = 1;
        GameManagerMono.instance.GameStart();
    }

}
