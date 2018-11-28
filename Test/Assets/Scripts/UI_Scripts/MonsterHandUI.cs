using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHandUI : MonoBehaviour {

    [SerializeField] Slider voluminiumBar, fadedBar;
    [SerializeField] Image fadedFill;
    [SerializeField] Text timeLeftDisplay;
    [SerializeField] Image[] healthPips;
    float voluminium, maxVoluminium = 100f, timeUntilEmbed;
    int health;


    public void CanGrabFeedback(float value)
    {
        value = value / 2;

        if(value > voluminium)
        {
            fadedFill.color = Color.red;
            fadedFill.CrossFadeAlpha(.6f, 0f, false);
            fadedBar.value = value / maxVoluminium;
            
            fadedFill.CrossFadeAlpha(0f, .5f, false);
        }
        else
        {
            fadedFill.color = Color.green;
            fadedFill.CrossFadeAlpha(1f, 0f, false);
            fadedBar.value = value / maxVoluminium;

            fadedFill.CrossFadeAlpha(0f, .5f, false); 
        }
    }

    public void UpdateVoluminium(float value)
    {
        voluminium = value;
        voluminiumBar.value = voluminium / maxVoluminium;
    }

    public void UpdateHealth(int value)
    {
        health = value;

        //Debug.Log("Updating Left hand health.");
        for(int i = 0; i < healthPips.Length; i++)
        {
            if(i < health)
            {
                healthPips[i].enabled = true;
            }
            else
            {
                healthPips[i].enabled = false;
            }
        }
    }

    public void UpdateEmbedTime(float value)
    {

        timeUntilEmbed = value;
        if (timeUntilEmbed != -1)
            timeLeftDisplay.text = timeUntilEmbed.ToString("#.#");
        else
            timeLeftDisplay.text = "";
    }

}
