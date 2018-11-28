using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVRInterface : VR2DInterface {

    public FlatlanderController fc;
    public Rigidbody rb;
    public MonsterManager mm;

    
    // Use this for initialization
    protected override void Start ()
    {
        if(mm == null)
            mm = FindObjectOfType<MonsterManager>().GetComponent<MonsterManager>();

        base.Start();
    }

    private void Reset()
    {
        fc = GetComponent<FlatlanderController>();
        rb = GetComponent<Rigidbody>();
        mm = FindObjectOfType<MonsterManager>().GetComponent<MonsterManager>();
    }

    public override void AmHeld(ControllerGrabObject holdingObject)
    {
        

        if (mm.voluminium >= fc.resistance)
        {
            base.AmHeld(holdingObject);

            mm.voluminium -= fc.resistance;

            fc.isInteractable = false;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;

            //Debug.Log("Derived Method Running.");
        }
}

    public override void LetGo()
    {
        base.LetGo();
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        fc.isInteractable = true;
    }

    
    protected override void CmdForceRelease()
    {
        base.CmdForceRelease();

        RpcForceRelease();
    }
    
    public override void RpcForceRelease()
    {
        base.RpcForceRelease();
        rb.useGravity = true;
        fc.isInteractable = true;
    }



}
