using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControllerGrabObject : NetworkBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public bool infiniteGrip;

    // 1
    private GameObject collidingObject;
    // 2
    private GameObject objectInHand;

    private SteamVR_Controller.Device Controller
    {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void SetCollidingObject(Collider col)
    {
        // 1
        if (collidingObject || !col.GetComponent<Rigidbody>())
        {
            return;
        }
        // 2
        collidingObject = col.gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        SetCollidingObject(other);
    }

    // 2
    public void OnTriggerStay(Collider other)
    {
        SetCollidingObject(other);
    }

    // 3
    public void OnTriggerExit(Collider other)
    {
        if (!collidingObject)
        {
            return;
        }

        collidingObject = null;
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        if (objectInHand.GetComponent<VR2DInterface>() != null)
            objectInHand.GetComponent<VR2DInterface>().AmHeld(this);
      
    }

    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        if (!infiniteGrip)
        {
            fx.breakForce = 20000;
            fx.breakTorque = 20000;
        }else{
            fx.breakForce = Mathf.Infinity;
            fx.breakTorque = Mathf.Infinity;
        }
        return fx;
    }

    private void ReleaseObject()
    {
        // 1
        if (GetComponent<FixedJoint>())
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            // 3
            objectInHand.GetComponent<Rigidbody>().velocity = Controller.velocity * 30f;
            objectInHand.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;

            if (objectInHand.GetComponent<VR2DInterface>() != null)
                objectInHand.GetComponent<VR2DInterface>().LetGo();

            //Debug.Log("Released object's velocity is " + objectInHand.GetComponent<Rigidbody>().velocity+ ". Controller's velocity is" + Controller.velocity);
        }
        // 4
        objectInHand = null;
    }

    public void ForceRelease()
    {
        if (GetComponent<FixedJoint>() != null)
        {
            // 2
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
           

            Debug.Log("We were forced to let go of the object.");
        }
        // 4
        objectInHand = null;
    }

    // Update is called once per frame
    void Update()
    {

        // 1
        if (Controller.GetHairTriggerDown())
        {
            if (collidingObject)
            {
                if(collidingObject.GetComponent<VR2DInterface>() == null || collidingObject.GetComponent<VR2DInterface>().isInteractable)
                    GrabObject();
            }
        }

        // 2
        if (Controller.GetHairTriggerUp())
        {
            if (objectInHand)
            {
                ReleaseObject();
            }
        }
    }
}
