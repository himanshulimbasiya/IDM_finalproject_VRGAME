using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBelowSensor : MonoBehaviour {

    [SerializeField]Collider mainCol;
    

    private void OnTriggerEnter(Collider other)
    {
        FlatlanderController player = other.GetComponent<FlatlanderController>();

        if(player != null)
        {
            Physics.IgnoreCollision(mainCol, other);
        }

        FlatlanderControllerMono playerMono = other.GetComponent<FlatlanderControllerMono>();

        if (playerMono != null)
        {
            Physics.IgnoreCollision(mainCol, other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        FlatlanderController player = other.GetComponent<FlatlanderController>();

        if (player != null)
        {
            Physics.IgnoreCollision(mainCol, other, false);
        }
    }

}
