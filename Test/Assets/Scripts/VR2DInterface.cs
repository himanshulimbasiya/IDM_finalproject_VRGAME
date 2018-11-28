using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VR2DInterface : NetworkBehaviour {

    [HideInInspector] public bool isHeld = false;
    [HideInInspector] public ControllerGrabObject cgo;

    public bool isInteractable = true;

    CylinderObject cylOb;
    NetworkManager nM;

    protected virtual void Start()
    {
        cylOb = GetComponent<CylinderObject>();
    }

    public virtual void AmHeld(ControllerGrabObject holdingObject)
    {
        Debug.Log("Base function called. CylinderObject: " + cylOb);

        if (isInteractable)
        {
            isHeld = true;
            cgo = holdingObject;
            if (cylOb != null)
                cylOb.enabled = false;
        }
        else
        {
            RpcForceRelease();
        }



    }

    public virtual void LetGo()
    {
        isHeld = false;
        cgo = null;
        if (cylOb != null)
            cylOb.enabled = true;
    }

    [Command]
    protected virtual void CmdForceRelease()
    {
        isHeld = false;
        cgo.ForceRelease();
        cgo = null;
        if (cylOb != null)
            cylOb.enabled = true;
        RpcForceRelease();
    }

    [ClientRpc]
    public virtual void RpcForceRelease()
    {
        isHeld = false;
        cgo.ForceRelease();
        cgo = null;
        if (cylOb != null)
            cylOb.enabled = true;
    }

}