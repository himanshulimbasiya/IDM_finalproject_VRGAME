using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class FlatlanderControllerMono : MonoBehaviour
{
    //public variables
    public Rigidbody rb;
    public float moveSpeed, 
        maxFallSpeed, 
        airSpeed, 
        fallMultiplier,
        wallSlideSpeedMax,
        wallSlideDistance = .5f,
        airSpeedMax;
    public Vector2 wallHop,
        wallJump,
        wallLeap;
    public float jumpForce, 
        wallStickTime = .25f,
        dashDuration,
        dashForce,
        dashDistance,
        dashCooldown,
        regenRate;
    public bool isInteractable = true;
    public Transform groundCheck;
    //public Transform[] groundCheckArray = new Transform[3];
    public LayerMask whatIsGround,
        whatIsWall;
    [SerializeField]
    [Range(1f, 100f)] float resistance = 100f;
    [SerializeField]
    FlatlanderUIMono flatUI;
    [SerializeField]
    Color dashRefresh, damage;
    public PlayerVRInterfaceMono vri;
    public GameObject voluminiumShard;
    public GameObject PlayerCamera;
    public AudioClip jumpAudio, dashAudio, damageAudio, pickUp, dashRefreshAudio;


    //private variables
    private Vector3 jumpVelocity = Vector3.zero,
        origin,
        jumpVel,
        move,
        dashDir;
    [SerializeField]
    PlayerIndex index;
    [SerializeField]
    hypersquare_target target;

    GamePadState state;
    float groundRadius = 0.45f, 
        horizontal,
        vertical,
        timeToUnstick,
        timeUntilDash = 0,
        timeDashing;
    bool grounded = false, 
        wallSliding,
        isDashing = false,
        facingRight = true,
        isVulnerable,
        hasVoluminium = false,
        jumpBuffered = false;
    int wallDir = 0, aDownCount, startDownCount, previousWallDir;
    Collider col;
    SpriteRenderer spriteRenderer;

    private Animator flatlanderAnim;

    



    //debugging variables
    private Vector3 velocityBefore = Vector3.zero, 
        velocityAfter = Vector3.zero;


    // Use this for initialization
    void Start()
    {

            PlayerCamera.SetActive(true);

            ///initialization assignments. Find the centerpoint of the cylinder at start and assign it to a variable.
            ///assign find the sprite renderer and assign it.
            origin = GameManagerMono.instance.GetOrigin();


        //Starting this coroutine will give you debugging information about whether the players feet are touching the ground
        //StartCoroutine(ReportGrounded());

        dashDistance = Mathf.Pow(dashDistance, 2);
        col = GetComponent<Collider>();
        flatUI.UpdateResistance(resistance);
        spriteRenderer = GetComponent<SpriteRenderer>();
        flatlanderAnim = GetComponent<Animator>();
    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
    }
    


    private void OnDisable()
    {
        GamePad.SetVibration(index, 0f, 0f);
    }

    private void OnDestroy()
    {
        GamePad.SetVibration(index, 0f, 0f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        //foreach (Transform groundCheck in groundCheckArray)
        //{
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
        //}

        Gizmos.color = Color.red;
        if (facingRight)
        {
            Gizmos.DrawLine(transform.position, transform.position + (transform.right * wallSlideDistance));
        }
        else
        {
            Gizmos.DrawLine(transform.position, transform.position - (transform.right * wallSlideDistance));
        }
    }


    void Update()
    {
        state = GamePad.GetState(index);

        if(GetStartButtonDown())
        {
            EventManager.TriggerEvent("Paused");
            return;
        }

        if (isInteractable)
        {
                           
            //Check Conditions
            CheckGrounded();
            horizontal = state.ThumbSticks.Left.X;
            vertical = state.ThumbSticks.Left.Y;
            move = new Vector3(horizontal, vertical, 0);

            

            if (!grounded && rb.velocity.y < 0)
            {
                WallSliding();
            }
            else
            {
                wallSliding = false;
            }


            FlipSprite();

                


            //input for jumping
            if (GetAButtonDown())
            {
                if (wallSliding)
                {
                    SoundManager.instance.FlatlanderPlayClip(jumpAudio, (int)index);
                    if (wallDir == GetHoriizontalRaw())
                    {
                        Vector3 force = new Vector3(-wallDir * wallHop.x, wallHop.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        //Debug.Log("Wall Hop");
                    }
                    else if (GetHoriizontalRaw() == 0)
                    {
                        Vector3 force = new Vector3(-wallDir * wallJump.x, wallJump.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        facingRight = !facingRight;
                        spriteRenderer.flipX = !facingRight;
                        //Debug.Log("Wall Jump");
                    }
                    else
                    {
                        Vector3 force = new Vector3(-wallDir * wallLeap.x, wallLeap.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        //Debug.Log("Wall Leap");
                    }

                    //jumpAudio.mySource.Play();
                }
                else if (grounded)
                {
                    //jumpAudio.mySource.Play();
                    grounded = false;
                    jumpVelocity = new Vector3(0, jumpForce, 0);
                    SoundManager.instance.FlatlanderPlayClip(jumpAudio, (int)index);
                }

            }
            else if ((state.Buttons.A == ButtonState.Released) && rb.velocity.y > 0)
            {
                //cancel jump

                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * .2f, rb.velocity.z);
                //Debug.Log("Jump Canceled.");
            }


            //check for dash input
            if ((state.Triggers.Left > 0 || state.Buttons.B == ButtonState.Pressed) && timeUntilDash == 0)
            {
                //Debug.Log("Dashing.");
                StartCoroutine(Dashing(new Vector3(horizontal, vertical, 0)));
                    
            }
        }



        //stay oriented towards the camera
        if (!vri.isHeld)
        {
            transform.LookAt(new Vector3(origin.x, transform.position.y, origin.z));
        }

        if (!GameManagerMono.instance.IsCountingDown())
        {

            target.Target = GameManagerMono.instance.GetClosestHypersquare(transform.position);
        }

        PassiveRegeneration();


        flatlanderAnim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        flatlanderAnim.SetBool("hasAir", !grounded);
        flatlanderAnim.SetBool("onWall", wallSliding);
        flatlanderAnim.SetBool("dashing", isDashing);

    }



    // Update is called once per physics update 
    void FixedUpdate()
    {


        if (isDashing)
        {
            Vector3 force = dashDir * dashForce;
            rb.velocity = Vector3.zero;
            rb.AddRelativeForce(force, ForceMode.VelocityChange);

        }


        if (!isInteractable)
            return;
        
        //physics for jumping
        rb.AddForce(jumpVelocity, ForceMode.Impulse);
        jumpVelocity = Vector3.zero;

        //physics for movement
        if (grounded && isInteractable)
        {
            //while grounded
            Vector3 localVel = transform.InverseTransformDirection(rb.velocity);
            rb.AddRelativeForce(new Vector3(horizontal * moveSpeed, rb.velocity.y, 0) - localVel, ForceMode.Impulse);

            

            flatlanderAnim.SetFloat("speed", Mathf.Abs(horizontal));
        }
        else if(!wallSliding)
        {
            //while airborne
            rb.AddRelativeForce(new Vector3(horizontal*airSpeed, 0, 0));

            Vector2 lateralVelocity = new Vector2(rb.velocity.x, rb.velocity.z);

            if(lateralVelocity.sqrMagnitude > Mathf.Pow(airSpeedMax, 2f))
            {
                lateralVelocity.Normalize();
                lateralVelocity = lateralVelocity * airSpeedMax;
                rb.velocity = new Vector3(lateralVelocity.x, rb.velocity.y, lateralVelocity.y);
            }

            //Debug.Log("Lateral Velocuty: "+ lateralVelocity.magnitude);

        }

        if (rb.velocity.y < -.1)
        {
            //velocityBefore = rb.velocity;
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            //velocityAfter = rb.velocity;
            //Debug.Log("Increasing gravity on player. Velocity before: " + velocityBefore + " Velocity after: " + velocityAfter);
        }

       


        //multiply the force of gravity on a player by 9 times
        //if (!grounded)
            rb.AddForce(Physics.gravity * 4f);



        // wall sliding and walljumping code
        //********* Rethink this I think ***********************


        if (wallSliding && rb.velocity.y < -wallSlideSpeedMax)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeedMax, rb.velocity.z);
        }
        else if (rb.velocity.y < -maxFallSpeed)
        {
            //Do this properly
            rb.velocity = new Vector3(rb.velocity.x, -maxFallSpeed, rb.velocity.z);
        }




    }



    private void OnCollisionStay(Collision collision)
    {
        if (!isDashing)
            return;

        if (collision.gameObject.CompareTag("Hypersquare"))
        {
            Collider[] colliders = GetComponents<Collider>();
            foreach (Collider col in colliders)
            {
                Physics.IgnoreCollision(col, collision.collider, true);
                StartCoroutine(ReEnableCollilsion(col, collision.collider));
            }
        }

       
    }

    IEnumerator ReEnableCollilsion(Collider col1, Collider col2)
    {

        while (isDashing)
            yield return null;


        if(col1 != null && col2 != null)
            Physics.IgnoreCollision(col1, col2, false);
    }

    bool JumpBuffered()
    {
        if(GetAButtonDown())
        {

        }

        return jumpBuffered;
    }

    bool GetAButtonDown()
    {
        if(state.Buttons.A == ButtonState.Pressed)
        {
            if (aDownCount == 0)
            {
                aDownCount++;
                return true;
            }
            else
            {

                aDownCount++;
                return false;
            }
        }
        else
        {

            aDownCount = 0;
            return false;
        }
    }

    bool GetStartButtonDown()
    {
        if (state.Buttons.Start == ButtonState.Pressed)
        {
            if (startDownCount == 0)
            {
                startDownCount++;
                return true;
            }
            else
            {

                startDownCount++;
                return false;
            }
        }
        else
        {

            startDownCount = 0;
            return false;
        }
    }

    IEnumerator BufferTime(float duration)
    {
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            yield return null;
        }
        jumpBuffered = false;
    }


    void WallSliding()
    {
        wallSliding = false;
        wallDir = 0;
        Vector3 headHeight = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
        Vector3 kneeHeight = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z); ;

        //checks to see what walls are around
        if (Physics.Raycast(headHeight, transform.right, wallSlideDistance, whatIsWall) || Physics.Raycast(kneeHeight, transform.right, wallSlideDistance, whatIsWall))
        {
            //there's a wall to our right
                wallDir = 1;
        }
        
        if (Physics.Raycast(headHeight, -transform.right, wallSlideDistance, whatIsWall) || Physics.Raycast(kneeHeight, -transform.right, wallSlideDistance, whatIsWall))
            {
            //there's a wall to our left
                wallDir = -1;
        }

        //wallsticking
        if (timeToUnstick > 0 && wallDir != 0)
        {
            if (horizontal != wallDir && horizontal != 0)
            {
                timeToUnstick -= Time.deltaTime;
                if (timeToUnstick > 0)
                    wallSliding = true;
                else
                    wallSliding = false;
            }
            else
            {
                timeToUnstick = wallStickTime;
                wallSliding = true;
            }

            if(wallSliding)
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        else
        {
            timeToUnstick = wallStickTime;
        }

        if(wallDir != 0 && wallDir != previousWallDir)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }

        previousWallDir = wallDir;
    }

    void PassiveRegeneration()
    {
        if (resistance < 100)
        {
            resistance += regenRate * Time.deltaTime;
            flatUI.UpdateResistance(resistance);
        }

    }

    void CheckGrounded()
    {
        Collider[] colliders = Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround);

        if( colliders.Length > 0)
        {   
            
            grounded = true;
            //foreach (Collider collider in colliders)
            //{
            //    if (collider.gameObject.CompareTag("Platform"))
            //    {
            //        PlatformController platform = collider.gameObject.GetComponent<PlatformController>();

            //        if (platform.GetIgnoredColliders().Contains(col))
            //        {
            //            Debug.Log("I'm being ignored.");
            //            grounded = false;
            //        }
            //    }
            //}


        }
        else
            grounded = false;


    }

    float GetHoriizontalRaw()
    {
        if(horizontal > 0)
        {
            return 1;
        } else if(horizontal < 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    IEnumerator FlashColour(Color colour, float duration)
    {
        float startDuration = duration;
        spriteRenderer.color = colour;
        while(duration > 0)
        {
            duration -= Time.deltaTime;
            spriteRenderer.color = Color.Lerp(Color.white, colour, duration/startDuration);
            yield return null;
        }

        spriteRenderer.color = Color.white;
    }

    

    void FlipSprite()
    {

        if (wallSliding)
        {
            if(wallDir == 1)
            {
                facingRight = true;
                spriteRenderer.flipX = false;
            }
            else
            {
                facingRight = false;
                spriteRenderer.flipX = true;
            }

            return;
        }


        if (horizontal < 0)
        {
            facingRight = false;
            spriteRenderer.flipX = true;

            
            FlipSprite(true);
            if (voluminiumShard != null)
                voluminiumShard.transform.position = transform.position + (Vector3.right *.5f);
        }
        else if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
            facingRight = true;
            FlipSprite(false);
            if (voluminiumShard != null)
                voluminiumShard.transform.position = transform.position + (Vector3.left * .5f);
        }

    }

    public void AttachVoluminium(GameObject voluminium)
    {
        if (voluminiumShard == null)
        {
            voluminiumShard = voluminium;

            voluminiumShard.transform.parent = transform;
            SoundManager.instance.FlatlanderPlayClip(pickUp, (int)index);
        }
    }

    public void RemoveVoluminium()
    {
        voluminiumShard = null;
    }

    public bool GetIsDashing()
    {
        return isDashing;
    }

    public float GetVerticalAxis()
    {
        return vertical;
    }

    public bool GetJumpButton()
    {
        if (state.Buttons.A == ButtonState.Pressed)
            return true;
        else
            return false;
    }


    //Coroutines
    IEnumerator ReportGrounded()
    {
        while (true)
        {
            Debug.Log("Player is Grounded? "+ grounded);
            yield return new WaitForSeconds(2);
        }
    }

    IEnumerator Dashing(Vector3 dir)
    {
        isDashing = true;
        if (dir == Vector3.zero)
        {
            Debug.Log("Dash had no direction.");
            isDashing = false;
            yield break;
        }

        

        Vector3 startPos = transform.position;
        Vector3 distanceFromStartPos = Vector3.zero;

        SoundManager.instance.FlatlanderPlayClip(dashAudio, (int)index);
        timeUntilDash = dashCooldown;
        timeDashing = dashDuration;
        rb.useGravity = false;
        isInteractable = false;
        vri.isInteractable = false;
        isVulnerable = false;
        rb.velocity = Vector3.zero;
        dashDir = Vector3.Normalize(dir);

        //Debug.Log("Dashing Vector" + dir);

        while (timeDashing > 0 && distanceFromStartPos.sqrMagnitude < dashDistance)
        {
            distanceFromStartPos = transform.position - startPos;
            

            timeDashing -= Time.deltaTime;
            timeUntilDash -= Time.deltaTime;
            yield return null;
        }

        isInteractable = true;
        vri.isInteractable = true;
        isVulnerable = true;
        isDashing = false;
        rb.useGravity = true;
        rb.velocity = rb.velocity * .1f;
        //Debug.Log("Done Dashing.");

        while (timeUntilDash > 0)
        {
            timeUntilDash -= Time.deltaTime;
            yield return null;
        }

        StartCoroutine(FlashColour(dashRefresh, .3f));
        SoundManager.instance.FlatlanderPlayClip(dashRefreshAudio, (int)index);

        timeUntilDash = 0;
        //Debug.Log("Dash has Cooled Down");
    }

    public void ResetFlatlander()
    {
        resistance = 100f;

        if(voluminiumShard != null)
        {
            Destroy(voluminiumShard);
            voluminiumShard = null;
        }

        flatUI.UpdateResistance(resistance);
        GetComponent<CylinderObject>().enabled = true;
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        isInteractable = true;
    }

    void FlipSprite(bool value)
    {
        spriteRenderer.flipX = value;
    }

    public PlayerIndex GetIndex()
    {
        return index;
    }

    public void SetIndex(int value)
    {
        switch (value)
        {
            case 0:
                index = PlayerIndex.One;
                break;
            case 1:
                index = PlayerIndex.Two;
                break;
            case 2:
                index = PlayerIndex.Three;
                break;
            case 3:
                index = PlayerIndex.Four;
                break;

            default:
                Debug.LogError("Invalid Player index");
                break;
        }
    }

    public float GetRightTrigger()
    {
        return state.Triggers.Right;
    }

    public float GetResistance()
    {
        return resistance;
    }

    public void DamageResistance(float amount)
    {
        resistance -= amount;
        if(resistance < 0)
        {
            resistance = 0;
        }
        flatUI.UpdateResistance(resistance);

        StartCoroutine(RumblePulse(.1f, 1));
        StartCoroutine(FlashColour(damage, .3f));
        
        //Debug.Log("Ouch!");
    }

    IEnumerator RumblePulse(float duration, float strength)
    {
        GamePad.SetVibration(index, 1f, 1f);
        yield return new WaitForSeconds(duration);
        GamePad.SetVibration(index, 0f, 0f);
    }

}