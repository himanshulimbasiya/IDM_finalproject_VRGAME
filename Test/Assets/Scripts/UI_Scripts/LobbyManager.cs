using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour {

    [SerializeField]
    Text playerCountText;

    [SerializeField]
    Slider playerCountSlider;
    [SerializeField]
    Image[] flatlanderSprites;

    int playerCount;


    private void Start()
    {
        playerCount = GameManagerMono.instance.GetPlayerCount();
        playerCountSlider.value = playerCount;
        UpdateFlatlanderImages();
        UpdatePlayerCountText();
    }

    public void UpdatePlayerCount()
    {
        playerCount = (int)playerCountSlider.value;
        UpdatePlayerCountText();
        UpdateFlatlanderImages();
        GameManagerMono.instance.SetPlayerCount(playerCount);
    }

    void UpdatePlayerCountText()
    {
        playerCountText.text = playerCount.ToString();
    }



    void UpdateFlatlanderImages()
    {
        

        for(int i = 0; i < flatlanderSprites.Length; i++)
        {
            if(i < playerCount)
            {
                flatlanderSprites[i].enabled = true;
            }
            else
            {
                flatlanderSprites[i].enabled = false;
            }
        }
    }

    public void ChangeScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
