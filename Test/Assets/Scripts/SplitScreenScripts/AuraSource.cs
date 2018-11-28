using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuraSource : MonoBehaviour {


    public float damageAmount;

    public bool _isCausingDamage = false;

    //float resistance;

    [SerializeField]
    HypersquareControllerMono hype;

    [SerializeField]
    GameObject mesh;

    FlatlanderControllerMono fc;
    List<Collider> playersInAura = new List<Collider>();

    //public Image ForeHealth;

    //public float Min;

    //public float Max;

    //private float CurrentHealthValue;

    //private float CurrentHealthPercentage;


    private void OnTriggerEnter(Collider other)
    {

        if (!_isCausingDamage)
            return;

        

        if (other.gameObject.CompareTag("Player"))
        {
            playersInAura.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (playersInAura.Contains(other))
        {
            playersInAura.Remove(other);
        }
    }

    void EnableAuraMesh()
    {
        mesh.SetActive(true);
    }


    public void PulseAura()
    {
        foreach(Collider player in playersInAura)
        {
            FlatlanderControllerMono fc = player.GetComponent<FlatlanderControllerMono>();
            if(!fc.vri.isHeld)
                fc.DamageResistance(damageAmount);
        }
    }

    public void Setup()
    {
        EnableAuraMesh();
        Collider[] initialList = Physics.OverlapSphere(transform.position, 4f);


        foreach(Collider collider in initialList)
        {
            if (collider.gameObject.CompareTag("Player"))
                playersInAura.Add(collider);
        }
    }

   

}
