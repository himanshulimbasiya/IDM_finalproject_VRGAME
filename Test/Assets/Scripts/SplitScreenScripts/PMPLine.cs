using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PMPLine : MonoBehaviour {

    [SerializeField] Transform to, from;
    [SerializeField] LineRenderer renderer;

    private void Start()
    {
        Vector3[] positions = new Vector3[] { to.transform.position, from.transform.position };

        renderer.SetPositions(positions);
    }

}
