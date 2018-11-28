using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject targetPlayer;
    public FlatlanderController fc;
    public CylinderObject playerCylinderObject;
    public GameManager gm;
    float rot, radius, cameraDistance, angle;
    Vector3 origin, modifiedOrigin, cameraOffset;


    private void Awake()
    {

    }

    private void Reset()
    {
        gm = FindObjectOfType<GameManager>();
        fc = targetPlayer.GetComponent<FlatlanderController>();
        playerCylinderObject = targetPlayer.GetComponent<CylinderObject>();
    }


    // Use this for initialization
    void Start () {

        if (gm == null)
            gm = GameManager.instance;

        origin = gm.GetOrigin();
        radius = gm.GetRadius();
        cameraDistance = gm.GetCameraRadius();


    }

    // Update is called once per frame
    void Update () {

    }

    void LateUpdate()
    {
        modifiedOrigin = new Vector3(origin.x, targetPlayer.transform.position.y, origin.z);

        transform.LookAt(modifiedOrigin);
    }

}
