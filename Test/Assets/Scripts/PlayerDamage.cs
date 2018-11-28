using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerDamage : MonoBehaviour {


    public float DamageRate = 0.5f;

    public int DamageAmount;

    public bool _isCausingDamage = false;

    public bool Repeat = false;

    public int Health = 100;


    public Image ForeHealth;

    public int Min;

    public int Max;

    private int CurrentHealthValue;

    private float CurrentHealthPercentage;


    void Start()
    {
        
        Min = 0;
        Max = Health;
        DamageAmount = 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        _isCausingDamage = true;

        if (other.gameObject.CompareTag("Player"))
        {
            Repeat = true;
            if (Repeat)
            {
                StartCoroutine(TakeDamage(DamageRate));
            }
            else
            {
                DamagePlayer(DamageAmount);
            }
        }
    }

    IEnumerator TakeDamage(float DamageRate)
    {
        while(_isCausingDamage)
        {
            DamagePlayer(DamageAmount);
            yield return new WaitForSeconds(DamageRate);
        }
    }

    public void DamagePlayer(int DamageAmount)
    {


        Health = Health - DamageAmount;
        if (Health < 0)
        {
            Health = 0;
        }
        
       SetHealth(Health);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            _isCausingDamage = false;
            Repeat = false;
        }
    }


    public void SetHealth(int health)
    {
        //Debug.Log(health);
        if (health != CurrentHealthValue)
        {
            if (Max - Min == 0)
            {
                CurrentHealthValue = 0;
                CurrentHealthPercentage = 0;
            }
            else
            {
                CurrentHealthValue = health;
                CurrentHealthPercentage = (float)CurrentHealthValue / (float)(Max - Min);
            }

            //ForeHealth.fillAmount = CurrentHealthPercentage;
        }
    }

    public float CurrentPercentage
    {
        get { return CurrentHealthPercentage; }
    }

    public float CurrentHealth
    {
        get { return CurrentHealthValue; }
    }

}
