using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VR2DInterfaceMono : MonoBehaviour {

    [HideInInspector] public bool isHeld = false;
    [HideInInspector] public ControllerGrabObjectMono cgo;
    float maxHeight = 53, minHeight = 12;
    

    public bool isInteractable = true, isHypersquare = false;

    protected CylinderObject cylOb;

    Vector3 targetPos;


    protected virtual void Start()
    {
        cylOb = GetComponent<CylinderObject>();

        if (gameObject.CompareTag("Hypersquare"))
        {
            isHypersquare = true;
        }
    }



    public virtual void AmHeld(ControllerGrabObjectMono holdingObject)
    {
        //Debug.Log("Base function called. CylinderObject: " + cylOb);

        if (isInteractable)
        {
            isHeld = true;
            cgo = holdingObject;
            if (cylOb != null)
                cylOb.enabled = false;
        }
        else
        {
            ForceRelease();
        }



    }

    public virtual void LetGo()
    {
        isHeld = false;
        cgo = null;

        if (transform.position.y > maxHeight)
        {
            targetPos = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }
        else if (transform.position.y < minHeight)
        {
            targetPos = new Vector3(transform.position.x, minHeight, transform.position.z);
        }
        else
        {
            targetPos = transform.position;
        }

        if (!isHypersquare)
            StartCoroutine(LerpToPoint(cylOb.GetCylinderPos(targetPos)));


    }
    
    public ControllerGrabObjectMono GetControllerHolding()
    {
        return cgo;
    }
    
    protected virtual void ForceRelease()
    {
        isHeld = false;
        if (cgo != null)
            cgo.ForceRelease();
        cgo = null;


        if (transform.position.y > maxHeight)
        {
            targetPos = new Vector3(transform.position.x, maxHeight, transform.position.z);
        }
        else if (transform.position.y < minHeight)
        {
            targetPos = new Vector3(transform.position.x, minHeight, transform.position.z);
        }
        else
        {
            targetPos = transform.position;
        }


        StartCoroutine(LerpToPoint(cylOb.GetCylinderPos(targetPos)));
    }




    protected IEnumerator LerpToPoint(Vector3 endPos)
    {
        Vector3 startPos = transform.position;
        float speed = 1.5f, fraction = 0f;
        


        while (Vector3.Distance(transform.position, endPos) > .5f)
        {
            fraction += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, endPos, fraction);
            yield return null;
        }


        ResetToCylinder();
    }

    void ResetToCylinder()
    {
        if (cylOb != null)
            cylOb.enabled = true;

    }

}