using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MatrixBlender))]
public class PerspectiveSwitcher : MonoBehaviour
{
    [SerializeField]
    FlatlanderControllerMono fc;

    [SerializeField]
    GameObject cameraSpace, worldSpace;

    private Matrix4x4 ortho,
                        perspective;
    public float fov = 60f,
                        near = 1f,
                        far = 1000f,
                        startFar = 30f,
                        switchFar = 100f,
                        orthographicSize = 50f,
                        drainRate;
    private float aspect;
    private MatrixBlender blender;
    private bool orthoOn = true, inputConsumed = false;
    [SerializeField]
    GameObject backgroundCameraObject;
    [SerializeField]
    AudioClip switchSound, snatchedSound;

    Camera cam, backgroundCamera;
   
    float trigger, previousTrigger;

    void Start()
    {

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();
        cam = GetComponent<Camera>();
        aspect = (Screen.width * cam.rect.width) / (Screen.height * cam.rect.height);
        //Debug.Log("Aspect = " + aspect + ". Viewport width = " + cam.rect.width + ". Viewport height = " + cam.rect.height);
        ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
        perspective = Matrix4x4.Perspective(fov * cam.rect.width, aspect, near, far);
        cam.projectionMatrix = ortho;
        orthoOn = true;
        blender = (MatrixBlender)GetComponent(typeof(MatrixBlender));
    }

    void Update()
    {
        bool isHeld = fc.vri.isHeld;
        if (isHeld)
        {
            if (orthoOn)
            {
                orthoOn = false;
                cam.orthographic = false;
                SoundManager.instance.FlatlanderPlayClip(snatchedSound, (int)fc.GetIndex());

                worldSpace.SetActive(true);
                cameraSpace.SetActive(false);
                backgroundCameraObject.SetActive(false);
                
                cam.farClipPlane = switchFar;
                blender.BlendToMatrix(perspective, .5f);
                

            }
            cam.clearFlags = CameraClearFlags.Skybox;

            //Debug.Log("isHeld = " + isHeld + ". Clear Flags = "+ cam.clearFlags);
        }

        if (isHeld)
            return;

        trigger = fc.GetRightTrigger();
        //Debug.Log("isHeld = " + isHeld + ". Running after return statment.");

        if((trigger>0 && fc.GetResistance() > drainRate))
        {
            
            if (orthoOn)
            {
                orthoOn = false;
                cam.orthographic = false;
                SoundManager.instance.FlatlanderPlayClip(switchSound, (int)fc.GetIndex());

                worldSpace.SetActive(true);
                cameraSpace.SetActive(false);
                backgroundCameraObject.SetActive(false);
                cam.clearFlags = CameraClearFlags.Skybox;
                cam.farClipPlane = switchFar;
                blender.BlendToMatrix(perspective, .5f);
                
                StartCoroutine(ManageDrain());
       
            }

            
        }
        else if (trigger == 0 || fc.GetResistance() < drainRate/2)
        {
            if (!orthoOn)
            {
                orthoOn = true;
                cam.orthographic = true;
                worldSpace.SetActive(false);
                cameraSpace.SetActive(true);

                backgroundCameraObject.SetActive(true);
                StartCoroutine(TransitionBack());
           
                blender.BlendToMatrix(ortho, .5f);
                
                
            }

            if (fc.GetResistance() < drainRate / 2)
            {
                orthoOn = true;
                cam.orthographic = true;
                worldSpace.SetActive(false);
                cameraSpace.SetActive(true);
                backgroundCameraObject.SetActive(true);
                StartCoroutine(TransitionBack());


            }
            
        }

        
    }

    IEnumerator TransitionBack()
    {
        yield return new WaitForSeconds(.5f);
        cam.clearFlags = CameraClearFlags.Nothing;
        cam.farClipPlane = startFar;
    }
    
    IEnumerator ManageDrain()
    {
        while (!orthoOn)
        {
            fc.DamageResistance(drainRate/2);
            yield return new WaitForSeconds(.5f);
            yield return null;
        }
    }

}