using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMPTrigger : MonoBehaviour {

    public Collider targetCollider;
    public PMPLoader loader;



    private void OnTriggerEnter(Collider col)
    {
        if (targetCollider == col)
            loader.Charge();

    }
}
