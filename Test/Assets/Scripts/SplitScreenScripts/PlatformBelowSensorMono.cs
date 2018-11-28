using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBelowSensorMono : MonoBehaviour {

    [SerializeField]Collider mainCol;
    PlatformController controller;

    private void Start()
    {
        controller = mainCol.gameObject.GetComponent<PlatformController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        FlatlanderControllerMono player = other.GetComponent<FlatlanderControllerMono>();

        if(player != null)
        {

            Physics.IgnoreCollision(mainCol, other);
            controller.AddToIgnored(other);
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    FlatlanderControllerMono player = other.GetComponent<FlatlanderControllerMono>();

    //    if (player != null)
    //    {
    //        Physics.IgnoreCollision(mainCol, other, false);
    //        controller.RemoveIgnored(other);
    //    }
    //}

}
