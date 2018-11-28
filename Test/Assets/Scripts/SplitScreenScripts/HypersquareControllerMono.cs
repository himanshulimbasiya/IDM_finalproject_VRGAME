using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypersquareControllerMono : MonoBehaviour
{

    //public variables
    public Rigidbody rb;
    public CylinderObject cylOb;
    public MonsterManagerMono monsterManager;
    public float constrainDistance, 
        embedTolerance,
        radius,
        destroyPenalty,
        embeddedRotationSpeed,
        embedWaitTime,
        pulseRate,
        explodeRadius,
        explodeDamage,
        explodeForce,
        explodeUpdwardMultiplier,
        shakeDetectionThreshold = 2f,
        accelerometerUpdateInterval = 1.0f / 60.0f,
        lowPassKernelWidthInSeconds = 1.0f,
        ignitionMaintienceSpeed;
    public ParticleSystem embeddingParticles, ignitionParticles;
    public AudioClip explosion, embed, igniteExplode;

    //hidden public variables
    [HideInInspector]public bool isEmbedded = false, isRunning = false, isEmbedding = false, isIgnited = false;

    //private Variables
    Vector3 defaultPos, anchor, origin, lowPassValue;
    float monsterHeight = 30f, lowPassFilterFactor;
    VR2DInterfaceMono vri;
    ControllerGrabObjectMono cgo;
    [SerializeField] MeshRenderer renderer;
    [SerializeField]
    GameObject fragmentsPrefab;

    //Serialized Private variables
    [SerializeField]
    AuraSource aura;
    [SerializeField]
    LayerMask isPlayer;
    [SerializeField]
    Color defaultColour, ignitedColour, embeddingColour, embeddedColour;




    // Use this for initialization
    void Start()
    {
        origin = GameManagerMono.instance.GetOrigin();
        defaultPos = origin + Vector3.up * monsterHeight;
        rb.AddForce(Random.Range(20f, -20f), Random.Range(20f, -20f), Random.Range(20f, -20f));
        rb.AddTorque(Random.Range(20f, -20f), Random.Range(20f, -20f), Random.Range(20f, -20f));
        vri = GetComponent<VR2DInterfaceMono>();
        radius = GameManagerMono.instance.GetRadius();



        if(monsterManager == null)
            monsterManager = FindObjectOfType<MonsterManagerMono>();

        ChangeColour(defaultColour);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isEmbedded && !vri.isHeld)
            ConstrainToSource();

        if (vri.isHeld)
        {
            CheckShake();

            anchor = new Vector3(origin.x, transform.position.y, origin.z);
            

            if (Vector3.Distance(anchor, transform.position) > radius - embedTolerance )
            {
                if (isIgnited)
                    FizzleIgnite();

                if(!isEmbedding)
                    StartCoroutine(WaitUntilEmbed());
            }

            
        }
    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
        monsterManager = FindObjectOfType<MonsterManagerMono>();
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

    void EmbedCube()
    {
        vri.LetGo();
        isEmbedded = true;
        transform.position = cylOb.GetCylinderPos();
        ChangeColour(embeddedColour);

        vri.isInteractable = false;
        rb.constraints = RigidbodyConstraints.FreezePosition;
        rb.isKinematic = true;
        aura.Setup();
        aura._isCausingDamage = true;
        GameManagerMono.instance.AddEmbeddedHypersquare(gameObject);
        ChangeColour(embeddedColour);
        

        

        StartCoroutine(EmbeddedPulse());
        StartCoroutine(ManageEmbeddedCube());

        
    }

    void CheckShake()
    {
        Vector3 acceleration = vri.GetControllerHolding().GetController().velocity;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;

        //Debug.Log(deltaAcceleration.sqrMagnitude);

        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            //Debug.Log("Shake event detected at time " + Time.time);
            vri.cgo.HapticFeedback(500, .01f);


            if(!isEmbedding)
                IgniteCube();
        }

    }

    void IgniteCube()
    {
        isIgnited = true;
        //ignitionParticles.Play();
        ChangeColour(ignitedColour);

        StartCoroutine(AboutToExplode());
    }

    void FizzleIgnite()
    {
        isIgnited = false;
        ignitionParticles.Stop();

        ChangeColour(defaultColour);
    }


    public void Explode()
    {

        GameManagerMono.instance.SpawnShard(transform.position);
        GameManagerMono.instance.RemoveHypersquare(gameObject);
        GameManagerMono.instance.RemoveEmbeddedHypersquare(gameObject);
        SoundManager.instance.HypersquarePlayClip(explosion);
        monsterManager.ChangeVoluminium(-destroyPenalty);
        Instantiate(fragmentsPrefab, transform.position, transform.rotation);


        Destroy(gameObject);
    }

    

    IEnumerator AboutToExplode()
    {
        //Debug.Log("Somthing here, will, eventually have to explode, have to explode.");

        while ((vri.isHeld || rb.velocity.magnitude > ignitionMaintienceSpeed) && isIgnited)
        {
            if (Vector3.Distance(transform.position, cylOb.GetCylinderPos()) < 2.5f && !vri.isHeld)
            {
                HaveToExplode();
                
            }
                yield return null;
        }

        FizzleIgnite();
    }

    void HaveToExplode()
    {
        //Debug.Log("Have to explode.");
        Collider[] players = Physics.OverlapSphere(transform.position, explodeRadius, isPlayer);


        foreach(Collider player in players)
        {
            player.GetComponent<FlatlanderControllerMono>().DamageResistance(explodeDamage/2);
            player.GetComponent<Rigidbody>().AddExplosionForce(explodeForce, transform.position, explodeRadius, explodeUpdwardMultiplier, ForceMode.Impulse);

        }
        SoundManager.instance.HypersquarePlayClip(igniteExplode, .7f);
        Instantiate(fragmentsPrefab, transform.position, transform.rotation);

        FizzleIgnite();
    }

    public void AmHeld(ControllerGrabObjectMono cgo)
    {
        this.cgo = cgo;
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
    }

    public void AutoEmbed(Vector3 pos)
    {
        StartCoroutine(LerpToPoint(pos));
    }

    void ChangeColour(Color colour)
    {
        renderer.material.SetColor("_EmissionColor", colour);
    }

    public IEnumerator LerpToPoint(Vector3 endPos)
    {
        Vector3 startPos = transform.position;
        float speed = .45f, fraction = 0f;

        while(Vector3.Distance(transform.position, endPos) > .5f)
        {
            fraction += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, fraction);

            yield return null;
        }

        EmbedCube();
    }

    IEnumerator WaitUntilEmbed()
    {
        isEmbedding = true;
        float thisWaitTime = embedWaitTime;

        ChangeColour(embeddingColour);
        SoundManager.instance.HypersquarePlayClip(embed, .5f);
        Debug.Log("Wait for embed setup running.");
        //SteamVR_Controller grabbingController= cgo.GetComponent<SteamVR_Controller>();
        ControllerGrabObjectMono cgo = vri.GetControllerHolding();
        cgo.RadialImageEnabled(true);

        //Debug.Log("Waiting for Embedding.");
        while (thisWaitTime > 0 && Vector3.Distance(cylOb.GetCylinderPos(), transform.position) < (embedTolerance))
        {
            

            if (cgo != null)
            {

                cgo.SetFillOfRadialImage((embedWaitTime - thisWaitTime / embedWaitTime) - 1.2f);   
                cgo.HapticFeedback(1000);
            }

            thisWaitTime -= Time.deltaTime;
            yield return null;
        }



        if (thisWaitTime <= 0 && !isEmbedded)
            EmbedCube();
        else
        {
            ChangeColour(defaultColour);
            SoundManager.instance.HypersquareStop();
        }

        cgo.RadialImageEnabled(false);
        isEmbedding = false;
    }

    IEnumerator EmbeddedPulse()
    {
        while (isEmbedded)
        {
            
            aura.PulseAura();
            //Debug.Log("Damaging Resistance.");
            yield return new WaitForSeconds(pulseRate);
        }
        
    }

    IEnumerator ManageEmbeddedCube()
    {
        while (isEmbedded)
        {
            transform.Rotate((Vector3.one));
            yield return null;
        }
    }
}
