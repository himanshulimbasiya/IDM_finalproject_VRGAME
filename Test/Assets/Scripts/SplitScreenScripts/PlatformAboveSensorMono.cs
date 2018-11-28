using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAboveSensorMono : MonoBehaviour {

    [SerializeField] Collider mainCol;

    PlatformController controller;

    private void Start()
    {
     //   controller = mainCol.gameObject.GetComponent<PlatformController>();
    }

    private void OnTriggerStay(Collider other)
    {

        FlatlanderControllerMono playerMono = other.GetComponent<FlatlanderControllerMono>();
        if (playerMono != null)
        {
            if (playerMono.GetVerticalAxis() < -.5f || playerMono.rb.velocity.y > .1)
            {
                Physics.IgnoreCollision(mainCol, other);
                //controller.AddToIgnored(other);
            }
            else
            {
                Physics.IgnoreCollision(mainCol, other, false);
                //controller.RemoveIgnored(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(mainCol, other);
        //controller.AddToIgnored(other);

    }


}
