using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerMono : MonoBehaviour {

    public GameObject targetPlayer;
    public FlatlanderControllerMono fc;
    public CylinderObject playerCylinderObject;
    public GameManagerMono gm;

    [SerializeField]
    Camera playerCamera;
    float rot, radius, cameraDistance, angle;
    int playerIndex, playerCount;
    Vector3 origin, modifiedOrigin, cameraOffset;


    private void Awake()
    {

    }

    private void Reset()
    {
        gm = FindObjectOfType<GameManagerMono>();
        fc = targetPlayer.GetComponent<FlatlanderControllerMono>();
        playerCylinderObject = targetPlayer.GetComponent<CylinderObject>();
        playerCamera = GetComponent<Camera>();
    }


    // Use this for initialization
    void Start () {

        if (gm == null)
            gm = GameManagerMono.instance;


        origin = gm.GetOrigin();
        radius = gm.GetRadius();
        cameraDistance = gm.GetCameraRadius();
        playerIndex = (int)fc.GetIndex();
        playerCount = gm.GetPlayerCount();

        SetCameraLayout();

    }

    // Update is called once per frame
    void Update () {

    }

    void LateUpdate()
    {
        modifiedOrigin = new Vector3(origin.x, targetPlayer.transform.position.y, origin.z);

        if (!fc.vri.isHeld)
            transform.LookAt(modifiedOrigin);
        else
            transform.LookAt(targetPlayer.transform.position);
    }

    void SetCameraLayout()
    {

        switch (playerCount)
        {
            case 1:
                  playerCamera.rect = new Rect(Vector2.zero, Vector2.one);
                //Debug.Log("One Flatlander");
                break;


            case 2:
                switch (playerIndex)
                {
                    case 0:
                        playerCamera.rect = new Rect(new Vector2(0, .5f), new Vector2(1f, .5f));
                    break;

                    case 1:
                        playerCamera.rect = new Rect(Vector2.zero, new Vector2(1f, .5f));
                        break;

                    default:
                        Debug.LogError("Invalid player index.");
                        break;
                }
                break;


            case 3:
                switch (playerIndex)
                {
                    case 0:
                        playerCamera.rect = new Rect(new Vector2(0, .5f), new Vector2(1f, .5f));
                        break;

                    case 1:
                        playerCamera.rect = new Rect(Vector2.zero, new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0,0,-15));
                        break;

                    case 2:
                        playerCamera.rect = new Rect(new Vector2(.5f, 0f), new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0, 0, -15));
                        break;

                    default:
                        Debug.LogError("Invalid player index.");
                        break;

                }
                break;


            case 4:
                switch (playerIndex)
                {
                    case 0:
                        playerCamera.rect = new Rect(new Vector2(0, .5f), new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0, 0, -15));
                        break;

                    case 1:
                        playerCamera.rect = new Rect(new Vector2(.5f, .5f), new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0, 0, -15));

                        break;

                    case 2:
                        playerCamera.rect = new Rect(Vector2.zero, new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0, 0, -15));
                        break;

                    case 3:
                        playerCamera.rect = new Rect(new Vector2(.5f, 0), new Vector2(.5f, .5f));
                        playerCamera.transform.Translate(new Vector3(0, 0, -15));
                        break;

                    default:
                        Debug.LogError("Invalid player index.");
                        break;
                }
                break;

            default:
                Debug.LogError("Invalid player count.");
                break;
        }
    }

}
