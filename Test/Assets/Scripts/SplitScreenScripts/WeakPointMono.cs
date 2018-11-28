using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPointMono : MonoBehaviour {

    public HypersquareControllerMono hype;
    FlatlanderControllerMono flat;
    int count = 0;

    private void OnTriggerEnter(Collider other)
    {
        count++;
        if (count > 1)
            return;
        flat = other.GetComponent<FlatlanderControllerMono>();

        if (flat == null)
            return;

        if (flat.GetIsDashing())
        {
            hype.Explode();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        count--;
        if (count < 0)
        {
            count = 0;
        }
    }

}
