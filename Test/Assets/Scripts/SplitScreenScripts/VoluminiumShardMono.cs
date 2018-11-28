using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoluminiumShardMono : MonoBehaviour {

    public SphereCollider collider;
    public Rigidbody rb;
    public CylinderObject cylOb;


    FlatlanderControllerMono ps;

    void OnCollisionEnter(Collision col)
    {
        ps = col.gameObject.GetComponent<FlatlanderControllerMono>();
        if (ps != null && ps.voluminiumShard == null)
        {
            

            ps.AttachVoluminium(gameObject);

            rb.detectCollisions = false;
            rb.useGravity = false;
            rb.isKinematic = true;
            collider.enabled = false;
            this.enabled = false;
            cylOb.enabled = false;
            AttachVoluminium();
        }
    }


    void AttachVoluminium()
    {
        ps.AttachVoluminium(gameObject);

        rb.detectCollisions = false;
        rb.useGravity = false;
        rb.isKinematic = true;
        collider.enabled = false;
        this.enabled = false;
        cylOb.enabled = false;

    }
}
