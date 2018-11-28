using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVRInterfaceMono : VR2DInterfaceMono {

    public FlatlanderControllerMono fc;
    public Rigidbody rb;
    public MonsterManagerMono mm;
    public float durationHeld;

    
    // Use this for initialization
    protected override void Start ()
    {
        if(mm == null)
            mm = FindObjectOfType<MonsterManagerMono>().GetComponent<MonsterManagerMono>();

        base.Start();
    }

    private void Reset()
    {
        fc = GetComponent<FlatlanderControllerMono>();
        rb = GetComponent<Rigidbody>();
        mm = FindObjectOfType<MonsterManagerMono>().GetComponent<MonsterManagerMono>();
    }

    public override void AmHeld(ControllerGrabObjectMono holdingObject)
    {
        //Debug.Log("Derived Method Running.");

        if (mm.GetVoluminium() >= fc.GetResistance() / GameManagerMono.instance.snatchDivisor)
        {
            base.AmHeld(holdingObject);

            mm.ChangeVoluminium(-fc.GetResistance()/GameManagerMono.instance.snatchDivisor);
            
            fc.isInteractable = false;
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
            Debug.Log("Rigidbody should be unconstrained.");
            StartCoroutine(CountdownToLetGo(durationHeld));
        }
    }

    IEnumerator CountdownToLetGo(float duration)
    {
        float currentStrength = 0, rateOfIncrease = 750;

        while(base.isHeld && duration > 0)
        {
            duration -= Time.deltaTime;
            currentStrength += rateOfIncrease * Time.deltaTime;
            base.cgo.HapticFeedback((ushort)currentStrength);
            yield return null;
        }

        if (base.isHeld)
        {
            ForceRelease();
        }
    }


    public float GetResistance()
    {
        return fc.GetResistance();
    }

    public override void LetGo()
    {
        base.LetGo();
        rb.useGravity = true;
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        fc.isInteractable = true;
        fc.rb.velocity = Vector3.zero;
    }

    protected override void ForceRelease()
    {
        base.ForceRelease();
        rb.useGravity = true;
        
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        fc.isInteractable = true;
        fc.rb.velocity = Vector3.zero;
    }

    void ResetToCylinder()
    {
        if (base.cylOb != null)
            base.cylOb.enabled = true;
    }


}
