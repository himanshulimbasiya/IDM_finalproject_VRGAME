using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControllerGrabObjectMono : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;

    public bool infiniteGrip;

    [SerializeField] float gripForce = 20000;
    [SerializeField] MonsterManagerMono mm;
    public Image fillImage;
    

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

        if (mm == null)
            mm = FindObjectOfType<MonsterManagerMono>();
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

        PlayerVRInterfaceMono player = other.GetComponent<PlayerVRInterfaceMono>();

        if (player != null)
        {
            //Debug.Log("If statment running.");
            mm.CanGrabFeedback(player.GetResistance());
        }
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

    private void OnEnable()
    {
        EventManager.StartListening("Game End", StopHaptic);
    }

    private void OnDisable()
    {
        EventManager.StopListening("Game End", StopHaptic);
    }


    public void SetFillOfRadialImage(float percentage)
    {
        fillImage.fillAmount = percentage;
    }

    public void RadialImageEnabled(bool value)
    {
        fillImage.enabled = value;
    }

    private void GrabObject()
    {
        // 1
        objectInHand = collidingObject;
        collidingObject = null;
        // 2
        var joint = AddFixedJoint();
        joint.connectedBody = objectInHand.GetComponent<Rigidbody>();

        if (objectInHand.GetComponent<VR2DInterfaceMono>() != null)
            objectInHand.GetComponent<VR2DInterfaceMono>().AmHeld(this);
      
    }



    // 3
    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        if (!infiniteGrip)
        {
            fx.breakForce = Mathf.Infinity;
            fx.breakTorque = Mathf.Infinity;
            StartCoroutine(ResetJoint(fx, .5f));
        }else{
            fx.breakForce = Mathf.Infinity;
            fx.breakTorque = Mathf.Infinity;
        }
        return fx;
    }

    IEnumerator ResetJoint(FixedJoint fx, float duration)
    {
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        if (fx != null)
        {
            fx.breakForce = gripForce;
            fx.breakTorque = gripForce;
        }
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

            if (objectInHand.GetComponent<VR2DInterfaceMono>() != null)
                objectInHand.GetComponent<VR2DInterfaceMono>().LetGo();

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
                if (collidingObject.gameObject.CompareTag("Player"))
                {
                    FlatlanderControllerMono player = collidingObject.GetComponent<FlatlanderControllerMono>();
                    if((player.GetResistance() /GameManagerMono.instance.snatchDivisor) < mm.GetVoluminium() && !player.GetIsDashing())
                    {
                        GrabObject();
                    }
                    else
                    {
                        HapticFeedback(1000, .5f);
                        mm.CanGrabFeedback(player.GetResistance());
                    }

                }else if(collidingObject.GetComponent<VR2DInterfaceMono>() == null || collidingObject.GetComponent<VR2DInterfaceMono>().isInteractable)
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

    public void HapticFeedback(ushort value)
    {

        Controller.TriggerHapticPulse(value);
    }



    public void HapticFeedback(ushort strength, float duration)
    {
        if(Controller != null)
        StartCoroutine(HapticDurtation(strength, duration));
    }

    IEnumerator HapticDurtation(ushort strength, float duration)
    {
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            HapticFeedback(strength);
            yield return null;
        }
    }

    void StopHaptic()
    {
        HapticFeedback(0);
    }

    private void OnJointBreak(float breakForce)
    {
        if (objectInHand.GetComponent<VR2DInterfaceMono>() != null)
            objectInHand.GetComponent<VR2DInterfaceMono>().LetGo();

        objectInHand = null;
        Debug.Log("Grip Force Exceeded.");
    }

    public SteamVR_Controller.Device GetController()
    {
        return Controller;
    }
}
