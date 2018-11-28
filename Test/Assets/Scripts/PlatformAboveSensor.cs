using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformAboveSensor : MonoBehaviour {

    [SerializeField] Collider mainCol;
    

    private void OnTriggerEnter(Collider other)
    {
        FlatlanderController player = other.GetComponent<FlatlanderController>();
        if(player != null)
        {
            if(player.GetVerticalAxis() < -.5f && player.GetJumpButton())
            {
                Physics.IgnoreCollision(mainCol, other);
            }
        }

        FlatlanderControllerMono playerMono = other.GetComponent<FlatlanderControllerMono>();
        if (playerMono != null)
        {
            if (playerMono.GetVerticalAxis() < -.5f && playerMono.GetJumpButton())
            {
                Physics.IgnoreCollision(mainCol, other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Physics.IgnoreCollision(mainCol, other, false);
    }


}
