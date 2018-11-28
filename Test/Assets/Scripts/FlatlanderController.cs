using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlatlanderController : NetworkBehaviour
{
    //public variables
    public Rigidbody rb;
    public float moveSpeed, 
        maxSpeed, 
        airSpeed, 
        fallMultiplier,
        wallSlideSpeedMax,
        wallSlideDistance = .5f;
    public Vector2 wallHop,
        wallJump,
        wallLeap;
    public float jumpForce, 
        wallStickTime = .25f,
        dashDuration,
        dashForce,
        dashCooldown;
    public bool isInteractable = true;
    public Transform groundCheck;
    //public Transform[] groundCheckArray = new Transform[3];
    public LayerMask whatIsGround;
    [Range(1f, 100f)] public float resistance = 100f;
    public PlayerVRInterface vri;
    public GameObject voluminiumShard;
    public GameObject PlayerCamera;


    //private variables
    private Vector3 jumpVelocity = Vector3.zero,
        origin,
        jumpVel,
        move;
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
        hasVoluminium = false;
    int wallDir = 0;
    SpriteRenderer spriteRenderer;

    private Animator flatlanderAnim;



    //debugging variables
    private Vector3 velocityBefore = Vector3.zero, 
        velocityAfter = Vector3.zero;


    // Use this for initialization
    void Start()
    {
        if (isLocalPlayer == true)
        {
            PlayerCamera.SetActive(true);

            ///initialization assignments. Find the centerpoint of the cylinder at start and assign it to a variable.
            ///assign find the sprite renderer and assign it.
            origin = GameManager.instance.GetOrigin();
            

            //Starting this coroutine will give you debugging information about whether the players feet are touching the ground
            //StartCoroutine(ReportGrounded());
        }
        else
            PlayerCamera.SetActive(false);

        spriteRenderer = GetComponent<SpriteRenderer>();
        flatlanderAnim = GetComponent<Animator>();
    }

    void Reset()
    {
        rb = GetComponent<Rigidbody>();
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

        if (isInteractable)
        {
                           
            //Check Conditions
            CheckGrounded();
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            //move = new Vector3(horizontal, vertical, 0);

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
            if (Input.GetButtonDown("Jump"))
            {
                if (wallSliding)
                {
                    if (wallDir == horizontal)
                    {
                        Vector3 force = new Vector3(-wallDir * wallHop.x, wallHop.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        Debug.Log("Wall Hop");
                    }
                    else if (horizontal == 0)
                    {
                        Vector3 force = new Vector3(-wallDir * wallJump.x, wallJump.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        facingRight = !facingRight;
                        spriteRenderer.flipX = !facingRight;
                        Debug.Log("Wall Jump");
                    }
                    else
                    {
                        Vector3 force = new Vector3(-wallDir * wallLeap.x, wallLeap.y, 0);
                        rb.AddRelativeForce(force, ForceMode.Impulse);
                        wallSliding = false;
                        timeToUnstick = 0;
                        Debug.Log("Wall Leap");
                    }
                }
                else if (grounded)
                {
                    grounded = false;
                    jumpVelocity = new Vector3(0, jumpForce, 0);
                }

            }
            else if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
            {
                //cancel jump

                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y * .2f, rb.velocity.z);
                //Debug.Log("Jump Canceled.");
            }


            //check for dash input
            if (Input.GetButtonDown("Dash") && timeUntilDash == 0)
            {
                Debug.Log("Dashing.");
                StartCoroutine(Dashing(new Vector3(horizontal, vertical, 0)));
                    
            }
            
         


        //stay oriented towards the camera
        transform.LookAt(new Vector3(origin.x, transform.position.y, origin.z));

        }

        if (hasAuthority)
        {
            flatlanderAnim.SetFloat("speed", rb.velocity.x);
            flatlanderAnim.SetBool("hasAir", !grounded);
            flatlanderAnim.SetBool("onWall", wallSliding);
            flatlanderAnim.SetBool("dashing", isDashing);

            //flatlanderAnim.SetBool("dashing", );
        }
    }



    // Update is called once per physics update 
    void FixedUpdate()
    {
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
            rb.AddRelativeForce(new Vector3(horizontal * airSpeed, rb.velocity.y, 0));

        }

        if (rb.velocity.y < -.1)
        {
            //velocityBefore = rb.velocity;
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            //velocityAfter = rb.velocity;
            //Debug.Log("Increasing gravity on player. Velocity before: " + velocityBefore + " Velocity after: " + velocityAfter);
        }

        //check for wall sliding
        


        //multiply the force of gravity on a player by 9 times
        if (!grounded)
            rb.AddForce(Physics.gravity * 9f);



        // wall sliding and walljumping code
        //********* Rethink this I think ***********************


        if (wallSliding && rb.velocity.y < -wallSlideSpeedMax)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallSlideSpeedMax, rb.velocity.z);
        }


    }


    void WallSliding()
    {
        wallSliding = false;
        wallDir = 0;

        //checks to see what walls are around
        if (Physics.Raycast(transform.position, transform.right, wallSlideDistance, whatIsGround))
            {
            //there's a wall to our right
                wallDir = 1;
        }
        
        if (Physics.Raycast(transform.position, -transform.right, wallSlideDistance, whatIsGround))
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
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        else
        {
            timeToUnstick = wallStickTime;
        }

    }

    void CheckGrounded()
    {
        //grounded = false;

        //foreach (Transform groundCheck in groundCheckArray)
        //{
        if (Physics.OverlapSphere(groundCheck.position, groundRadius, whatIsGround).Length > 0)
            grounded = true;
        else
            grounded = false;
        //}

    }

    void FlipSprite()
    {
        if (!hasAuthority)
            return;

        if (horizontal < 0)
        {
            facingRight = false;
            spriteRenderer.flipX = true;

            
            CmdFlipSprite(true);
            if (voluminiumShard != null)
                voluminiumShard.transform.position = transform.position + (Vector3.right *.5f);
        }
        else if (horizontal > 0)
        {
            spriteRenderer.flipX = false;
            facingRight = true;
            CmdFlipSprite(false);
            if (voluminiumShard != null)
                voluminiumShard.transform.position = transform.position + (Vector3.left * .5f);
        }

    }

    public void AttachVoluminium(GameObject voluminium)
    {
        if (!voluminiumShard != null)
        {
            voluminiumShard = voluminium;

            voluminiumShard.transform.parent = transform;
           
        }
    }

    public void RemoveVoluminium()
    {
        voluminiumShard = null;
    }



    public bool IsDashing()
    {
        return isDashing;
    }

    public float GetVerticalAxis()
    {
        return vertical;
    }

    public bool GetJumpButton()
    {
        return Input.GetButtonDown("Jump");
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
            yield break;
        }
            
        timeUntilDash = dashCooldown;
        timeDashing = dashDuration;
        rb.useGravity = false;
        isInteractable = false;
        vri.isInteractable = false;
        isVulnerable = false;
        rb.velocity = Vector3.zero;
        dir = Vector3.Normalize(dir);
        //Debug.Log("Dashing Vector" + dir);

        while (timeDashing > 0)
        {
            //rb.AddForce(dir * dashForce * Time.deltaTime, ForceMode.VelocityChange);
            Vector3 force = dir * dashForce * Time.deltaTime;
            rb.velocity = Vector3.zero;
            rb.AddRelativeForce(force, ForceMode.VelocityChange);
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

        
        
        timeUntilDash = 0;
        //Debug.Log("Dash has Cooled Down");
    }


    [Command]
    void CmdFlipSprite(bool value)
    {
        spriteRenderer.flipX = value;
        RpcFlipSprite(value);
    }

    [ClientRpc]
    void RpcFlipSprite(bool value)
    {
        spriteRenderer.flipX = value;
    }


}