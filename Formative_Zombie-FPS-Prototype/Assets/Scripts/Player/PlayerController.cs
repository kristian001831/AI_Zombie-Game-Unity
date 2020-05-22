using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private bool jumpingEnabled = true;
    [SerializeField] private float jumpSpeed;
    
    [Range(0.0f, 0.5f)]
    [SerializeField] private float fallRate;
    
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isLanding = false;
    private bool canJump = true;
    private bool prevGrounded = false;
    private bool grounded;

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private float horizontalMovement;
    private float verticalMovement;
    private Vector3 moveDirection;
    private float distanceToPoints;
    private Vector3 YAxisGravity;
    
    public bool IsRunning
    {
        get
        {
            return isRunning;
        }

        set
        {
            isRunning = value;
        }
    }

    public bool IsLanding
    {
        get
        {
            return isLanding;
        }

        set
        {
            isLanding = value;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
    }
    
    void Update()
    {
        //jumping
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && jumpingEnabled && canJump)
        {
            isJumping = true;
            Jump();
        }

        horizontalMovement = 0; verticalMovement = 0;

        //Calculate FPcontroller movement direction through WASD and arrows Input
        if (AllowMovement(transform.right * Input.GetAxis("Horizontal")))
        {
            horizontalMovement = Input.GetAxis("Horizontal");
        }
        if (AllowMovement(transform.forward * Input.GetAxis("Vertical")))
        {
            verticalMovement = Input.GetAxis("Vertical");
        }
      
        moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;

        //Toggle run & jump
        IsRunning = Input.GetKey(KeyCode.LeftShift);

        prevGrounded = IsGrounded();
    }

    private void FixedUpdate()
    {
        /* When calculating the moveDirection , the Y velocity always stays 0. 
         As a result the player is falling very slowy. 
         To solve this we add to the rb velocity the Y axis velocity */

        YAxisGravity = new Vector3(0, rb.velocity.y - fallRate, 0);
        if (!isJumping) { Move(); }
        
        rb.velocity += YAxisGravity;
    }

    public void Move()
    {
        if (!IsRunning)
        {
            rb.velocity = moveDirection * walkSpeed * Time.fixedDeltaTime * 100;
        }
        else
        {
            rb.velocity = moveDirection * runSpeed * Time.fixedDeltaTime * 100;
        }
    }

    public void Jump()
    {
        if (canJump)
        {
            rb.AddForce(new Vector3(0, jumpSpeed, 0), ForceMode.Impulse);
        }
    }

    // make a capsule cast to check weather there is an obstavle in front of the player ONLY when jumping.
    public bool AllowMovement(Vector3 castDirection)
    {
        Vector3 point1;
        Vector3 point2;
        if (!IsGrounded())
        {
            // The distance from the bottom and top of the capsule
            distanceToPoints = capsule.height / 2 - capsule.radius;
            /*Top and bottom capsule points respectively, transform.position is used to get points relative to 
               local space of the capsule. */
            point1 = transform.position + capsule.center + Vector3.up * distanceToPoints;
            point2 = transform.position + capsule.center + Vector3.down * distanceToPoints;
            float radius = capsule.radius * .95f;
            float capsuleCastDist = 0.1f;

            if (Physics.CapsuleCast(point1, point2, radius, castDirection, capsuleCastDist))
            {
                return false;
            }
        }
        canJump = true;
        return true;
    }

    // Make a sphere cast with down direction to define weather the player is touching the ground.
    public bool IsGrounded()
    {
        Vector3 capsule_bottom = transform.position + capsule.center + Vector3.down * distanceToPoints;
        float radius = 0.1f;
        float maxDist = 1.0f;
        RaycastHit hitInfo;
        if (Physics.SphereCast(capsule_bottom, radius, Vector3.down, out hitInfo, maxDist))
        {
            isJumping = false;
            return true;
        }
        return false;
    }
}
