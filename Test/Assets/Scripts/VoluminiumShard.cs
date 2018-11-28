using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VoluminiumShard : NetworkBehaviour {

    public SphereCollider collider;
    public Rigidbody rb;
    public CylinderObject cylOb;


    FlatlanderController ps;

    void OnCollisionEnter(Collision col)
    {
        ps = col.gameObject.GetComponent<FlatlanderController>();
        if (ps != null)
        {
            ps.AttachVoluminium(gameObject);

            rb.detectCollisions = false;
            rb.useGravity = false;
            rb.isKinematic = true;
            collider.enabled = false;
            this.enabled = false;
            cylOb.enabled = false;
            CmdAttachVoluminium();
        }
    }

    [Command]
    void CmdAttachVoluminium()
    {
        ps.AttachVoluminium(gameObject);

        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        collider.enabled = false;
        this.enabled = false;
        cylOb.enabled = false;

        RpcAttachVoluminium();
    }

    [ClientRpc]
    void RpcAttachVoluminium()
    {
        ps.AttachVoluminium(gameObject);

        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        collider.enabled = false;
        this.enabled = false;
        cylOb.enabled = false;

        this.enabled = false;
    }
}
