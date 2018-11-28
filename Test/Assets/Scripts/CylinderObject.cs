using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderObject : MonoBehaviour {

    //float rot = 0f;
    float radius;
    Vector3 origin, offset;

    public bool isConstrained = true;




    // Use this for initialization
    void Start () {

        if (GameManager.instance != null)
        {
            radius = GameManager.instance.GetRadius();
            origin = GameManager.instance.GetOrigin();
        }
        else
        {
            radius = GameManagerMono.instance.GetRadius();
            origin = GameManagerMono.instance.GetOrigin();
        }

        //Debug.Log("Cylinder Object Running");
    }



    // Update is called once per frame
    void Update () {
        if (!isConstrained)
            return;

        ConstrainPosToCylinder();
    }

    void ConstrainPosToCylinder()
    {
        transform.position = GetCylinderPos();
    }

    public Vector3 GetCylinderPos()
    {
        offset = transform.position - new Vector3(origin.x, transform.position.y, origin.z);
        offset.Normalize();
        offset = offset * radius;

        return new Vector3(offset.x, transform.position.y, offset.z);
    }

    public Vector3 GetCylinderPos(Vector3 pos)
    {
        offset = pos - new Vector3(origin.x, pos.y, origin.z);
        offset.Normalize();
        offset = offset * radius;



        return new Vector3(offset.x, pos.y, offset.z);
    }

    public Vector3 GetOffset()
    {
        return offset;
    }
}
