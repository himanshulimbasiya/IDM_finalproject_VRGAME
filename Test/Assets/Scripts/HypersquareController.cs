using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HypersquareController : NetworkBehaviour
{

    //public variables
    public Rigidbody rb;
    //public CylinderObject cylOb;
    public MonsterManager monsterManager;
    public float constrainDistance, 
        embedTolerance,
        radius,
        embeddedRotationSpeed,
        embedWaitTime;
    public ParticleSystem embeddingParticles;
    public GameObject voluminiumShardPrefab;

    //hidden public variables
    [HideInInspector]public bool isEmbedded = false, isRunning = false, isEmbedding;

    //private Variables
    Vector3 defaultPos, anchor, origin;
    float monsterHeight = 30f;
    VR2DInterface vr2D;
    ControllerGrabObject cgo;
    

    // Use this for initialization
    void Start()
    {
        origin = GameManager.instance.GetOrigin();
        defaultPos = origin + Vector3.up * monsterHeight;
        rb.AddForce(Random.Range(20f, -20f), Random.Range(20f, -20f), Random.Range(20f, -20f));
        rb.AddTorque(Random.Range(20f, -20f), Random.Range(20f, -20f), Random.Range(20f, -20f));
        vr2D = GetComponent<VR2DInterface>();
        radius = GameManager.instance.GetRadius();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEmbedded && !vr2D.isHeld)
            ConstrainToSource();

        if (vr2D.isHeld)
        {


            anchor = new Vector3(origin.x, transform.position.y, origin.z);
            

            if (Vector3.Distance(anchor, transform.position) > radius - embedTolerance )
            {
                StartCoroutine(WaitUntilEmbed());
            }
        }
    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }


    //this function checks to see if the cube has wandered too far from the source and pushes it back if it has. 
    //it also slows it down if it is moving too fast withing the center
    void ConstrainToSource()
    {
        if (Vector3.Distance(defaultPos, transform.position) > constrainDistance)
        {
            Vector3 force = defaultPos - transform.position;
            rb.AddForce(force);
        }
        else if (rb.velocity.magnitude > 1)
        {
            rb.AddForce(-rb.velocity * .5f);
        }
    }

    //void EmbedCube() {
    //    //do the housework to inform all the relevant classes of the event. Perhaps this should be an event.
    //    Debug.Log("I am embedded");
    //    vr2D.LetGo();
    //    isEmbedded = true;
    //    //cylOb.enabled = true;
    //    vr2D.isInteractable = false;
        
    //    rb.constraints = RigidbodyConstraints.FreezePosition;
    //    rb.isKinematic = true;


    //    StartCoroutine(OncePerSecondEmbedded());
    //    StartCoroutine(ManageEmbeddedCube());
    //}

    [Command]
    void CmdEmbedCube()
    {
        Debug.Log("Commanding the Server to Embed a hypersquare.");
        vr2D.LetGo();
        isEmbedded = true;
        //cylOb.enabled = true;
        vr2D.isInteractable = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.isKinematic = true;


        StartCoroutine(OncePerSecondEmbedded());
        StartCoroutine(ManageEmbeddedCube());


        RpcEmbedCube(transform.position);
    }

    [ClientRpc]
    void RpcEmbedCube(Vector3 pos)
    {

        Debug.Log("Commanding all clients to embed a hypersquare.");
        vr2D.LetGo();
        transform.position = pos;
        isEmbedded = true;
        //cylOb.enabled = true;
        vr2D.isInteractable = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.isKinematic = true;


        StartCoroutine(OncePerSecondEmbedded());
        StartCoroutine(ManageEmbeddedCube());
    }



    [Command]
    public void CmdExplode()
    {
        Debug.Log("BOOM! Hypersquare Down on server!");
        GameObject go = Instantiate(voluminiumShardPrefab, transform.position, transform.rotation);
        NetworkServer.Spawn(go);

        Destroy(gameObject);
        RpcExplode();
    }

    [ClientRpc]
    void RpcExplode()
    {
        Debug.Log("BOOM! Hypersquare Down on clients!");
        Destroy(gameObject);
    }

    public void AmHeld(ControllerGrabObject cgo)
    {
        this.cgo = cgo;   
    }


    IEnumerator WaitUntilEmbed()
    {
        float thisWaitTime = embedWaitTime;

        embeddingParticles.Play();
        //SteamVR_Controller grabbingController= cgo.GetComponent<SteamVR_Controller>();

        //Debug.Log("Waiting for Embedding.");
        while (thisWaitTime > 0 && Vector3.Distance(anchor, transform.position) > (radius - embedTolerance))
        {
           
            thisWaitTime -= Time.deltaTime;
            yield return null;
        }
        embeddingParticles.Stop();

        if (thisWaitTime <= 0 && !isEmbedded)
            CmdEmbedCube();
    }

    IEnumerator OncePerSecondEmbedded()
    {
        while (isEmbedded)
        {
            monsterManager.voluminium++;
            //Debug.Log("Damaging Resistance.");
            yield return new WaitForSeconds(1f);
        }
        
    }

    IEnumerator ManageEmbeddedCube()
    {
        while (isEmbedded)
        {
            transform.Rotate((Vector3.one * embeddedRotationSpeed), Time.deltaTime, Space.World);
            yield return null;
        }
    }

    IEnumerator ReportDistance()
    {
        isRunning = true;
        while (vr2D.isHeld)
        {
            if(anchor != null)
                Debug.Log("Distance to embedding is " + Vector3.Distance(anchor, transform.position));

            yield return new WaitForSeconds(2f);
        }
        isRunning = false;
    }
}
