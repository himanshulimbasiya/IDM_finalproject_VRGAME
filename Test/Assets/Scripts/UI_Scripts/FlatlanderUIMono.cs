using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlatlanderUIMono : MonoBehaviour {

    
    float resistance, maxResistance = 100, monsterVoluminium, maxVoluminium = 100;
    int monsterHealth;
    MonsterManagerMono mm;
    [SerializeField]
    Image resistanceForeground, 
        monsterVoluminiumForeground,
        monsterBar,
        worldSpaceBar;
    [SerializeField]
    Image[] healthPips;

    private void Awake()
    {
        resistance = maxResistance;
        monsterVoluminium = 0;
        mm = FindObjectOfType<MonsterManagerMono>();

        if (mm != null)
        {
            mm.AddToFlatlanders(this);
        }
        else
        {
            Debug.LogError("FlatlanderUI MonsterManager is null.");
        }
    }

    private void Start()
    {
        //TODO: UI does not scale with viewport size.
    }

    public void UpdateResistance(float newResistance)
    {
        resistance = newResistance;

        resistanceForeground.fillAmount = resistance / maxResistance;

        monsterBar.fillAmount = resistance / maxResistance;

        worldSpaceBar.fillAmount = resistance / maxResistance;
    }

    public void UpdateMonsterVoluminium(float newVoluminium)
    {
        monsterVoluminium = newVoluminium;

        monsterVoluminiumForeground.fillAmount = monsterVoluminium / maxVoluminium;
    }

    public void UpdateHealth(int newHealth)
    {
        monsterHealth = newHealth;
        foreach(Image pip in healthPips)
        {
            pip.enabled = true;
        }

        for(int i = 0; i < monsterHealth; i++)
        {
            healthPips[i].enabled = false;
        }
    }

}
