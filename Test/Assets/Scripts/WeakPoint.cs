using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour {

    public HypersquareController hype;
    FlatlanderController flat;

    private void OnTriggerEnter(Collider other)
    {

        flat = other.GetComponent<FlatlanderController>();

        if (flat == null)
            return;

        if (flat.IsDashing())
        {
            hype.CmdExplode();
        }
    }

}
