using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hypersquare_target : MonoBehaviour
{
    public Transform Target;
    [SerializeField] GameObject arrow;
    [SerializeField]Camera cam;


    void Update()
    {
        if(Target == null)
        {
            arrow.SetActive(false);
            return;
        }
        else
        {
            
            arrow.SetActive(true);
        }

        var dir = cam.WorldToViewportPoint(Target.position) - cam.WorldToViewportPoint(transform.position);

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
       
    }
}
