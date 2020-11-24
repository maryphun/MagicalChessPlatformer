using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Collider2D)),
    RequireComponent(typeof(PlayerRenderer)), RequireComponent(typeof(PlayerControl))]
public class Controller : MonoBehaviour
{
    [Header("Configiuration")]
    [SerializeField] private float moveSpeed = 2f, jumpInitiateVelocity = 2f, tailSpawnInterval = 0.05f, tailSpawnVelocity = 5.0f, velocityLimit = 30f;
    [SerializeField] private float jumpPressedRememberTime = 0.2f, groundedRememberTime = 0.2f;
    [SerializeField] private LayerMask groundMask = default;

    [Header("Debug")]
    [SerializeField] private bool ControlOn = true;

    private PlayerControl playerCtrl = null;
    private Rigidbody2D rigidbody;
    private Collider2D collider;
    private PlayerRenderer playerRenderer;
    private float jumpPressedRemember, groundedRemember, tailTimeCount;
    private Vector2 newPos;
    private bool StillJumping; // remain true if the jump key still not released

    private float previousInput;
    private bool previousGrounded;  // check if this character was grounded last frame


    private PlayerControl PlayerCtrl
    {
        get
        {
            if (playerCtrl != null) { return playerCtrl; }
            return playerCtrl = new PlayerControl();
        }
    }

    public void Awake()
    {
        // only enable this control when it get authority
        enabled = true;

        // Initialization
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        playerRenderer = GetComponentInChildren<PlayerRenderer>();
        jumpPressedRemember = 0f;
        StillJumping = false;
        
        // register the control
        // register Jump
        PlayerCtrl.Default.Jump.performed += _ => JumpPress();
        PlayerCtrl.Default.Jump.canceled += _ => JumpRelease();

        // register movement
        PlayerCtrl.Default.Move.performed += ctx => SetMovement(ctx.ReadValue<float>());
        PlayerCtrl.Default.Move.canceled += ctx => ResetMovement();

        if (!ControlOn)
        {
            PlayerCtrl.Disable();
        }
    }

    private void OnEnable()
    {
        // ControlOn is used for debugging
        if (ControlOn)
        {
            PlayerCtrl.Enable();
        }
    }

    private void OnDisable()
    {
        PlayerCtrl.Disable();
    }
    
    private void SetMovement(float movement) => previousInput = movement;
    private void ResetMovement() => previousInput = 0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Player visual that doesn't care if player own this character or not.
        Visuals();

        // Remember time. Even client side need to know if this player is grounded or not.
        jumpPressedRemember = Mathf.Clamp(jumpPressedRemember - Time.deltaTime, 0.0f, jumpPressedRememberTime);
        groundedRemember = Mathf.Clamp(groundedRemember - Time.deltaTime, 0.0f, groundedRememberTime);

        // Check if player is grounded
        GroundedRemember();
        
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, Mathf.Clamp(rigidbody.velocity.y, -velocityLimit, velocityLimit));
        
        // Read the input value
        float movementInput = previousInput;
        // calculate new position
        newPos = transform.position;
        newPos.x += movementInput * moveSpeed * Time.deltaTime;
        // Visual renderer
        playerRenderer.FlipSide(newPos.x - transform.position.x);
        playerRenderer.MoveAnimation(newPos.x != transform.position.x);
        // Apply new position
        Move(newPos, movementInput);

        // Key Remembered
        if (jumpPressedRemember > 0.0f)
        {
            if (IsGrounded())
            {
                Jump();
            }
        }
    }
    
    private void JumpPress()
    {
        jumpPressedRemember = jumpPressedRememberTime;
    }

    private void JumpRelease()
    {
        StillJumping = false;
        if (rigidbody.velocity.y > 0.0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y / 2);
        }
    }
    
    private void Jump()
    {
        rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0.0f);
        rigidbody.AddForce(new Vector2(0, jumpInitiateVelocity), ForceMode2D.Impulse);
        jumpPressedRemember = 0.0f;
        groundedRemember = 0.0f;
        StillJumping = true;
        playerRenderer.EnableTrail(true);
        playerRenderer.JumpDustPlay();
    }

    /// <summary>
    /// called when velocity > 0
    /// </summary>
    private void JumpUpward()
    {
        // ----- Check Collision of head to wall -----
        Vector2 topLeftPoint = new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y + collider.bounds.extents.y);
        Vector2 topRightPoint = new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y + collider.bounds.extents.y);

        if (CollisionCheck(topLeftPoint, groundMask) || CollisionCheck(topRightPoint, groundMask))
        {
            // head collided while jumping
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, (rigidbody.velocity.y * -1f) * 0.55f);
        }
    }

    /// <summary>
    /// called when velocity < 0
    /// </summary>
    private void JumpDownward()
    {
        // Check Collision of Leg
        Vector2 bottomLeftPoint = new Vector2(transform.position.x - collider.bounds.extents.x, transform.position.y - collider.bounds.extents.y);
        Vector2 bottomRightPoint = new Vector2(transform.position.x + collider.bounds.extents.x, transform.position.y - collider.bounds.extents.y);

        // this function is not necessary for now.
    }
    

    /// <summary>
    /// determine if the player is on ground. have some delay time default 0.2f
    /// </summary>
    /// <returns></returns>
    public bool IsGrounded()
    {
        return (groundedRemember > 0.0f);
    }

    private void GroundedRemember()
    {
        // Code Inside here only run locally
        bool groundedtmp = (rigidbody.velocity.y == 0.0f);

        playerRenderer.IsGroundedAnimParameter(groundedtmp);

        if (groundedtmp)
        {
            if (groundedtmp != previousGrounded)    // the inside of this if statement is the exact frame where player reach the ground.
            {
                // Reach the ground
                StillJumping = false;
                playerRenderer.EnableTrail(false);
            }
            groundedRemember = groundedRememberTime;
            playerRenderer.IsGroundedAnimParameter(true);
        }
        else
        {
            // On The Air
            if (rigidbody.velocity.y > 0.0f)
            {
                // Upward
                JumpUpward();
            }
            else
            {
                // Downward
                JumpDownward();
            }
        }

        previousGrounded = groundedtmp;
    }

    private void Move(Vector2 newPosition, float direction)
    {
        // change flip side (commented out because it's already done in the GameManagerPhoton script)
        //playerRenderer.FlipSide(direction); 

        // collision check before actually move the player
        Vector2 destinationTop = new Vector2(newPosition.x + (direction * collider.bounds.extents.x), transform.position.y + collider.bounds.extents.y - 0.05f);
        Vector2 destinationMid = new Vector2(newPosition.x + (direction * collider.bounds.extents.x), transform.position.y);
        Vector2 destinationBottom = new Vector2(newPosition.x + (direction * collider.bounds.extents.x), transform.position.y - collider.bounds.extents.y + 0.05f);

        // assign new position to the character 
        if (!CollisionCheck(destinationTop, groundMask)     // collision with wall
            && !CollisionCheck(destinationBottom, groundMask)
            && !CollisionCheck(destinationMid, groundMask))
        {
            transform.position = newPosition;
        }
    }

    private bool CollisionCheck(Vector2 point, LayerMask layerMask)
    {
        return (Physics2D.OverlapPoint(point, layerMask));
    }
    
    #region PlayerManagerPhoton call
    // Calling from PlayerManagerPhotonScript to get input data and synchronize the result with other player 
    public float GetInputData()
    {
        return previousInput;
    }

    // Calling from PlayerManagerPhotonScript to get reference of player renderer to do visual synchronization
    public PlayerRenderer GetPlayerRenderer()
    {
        return playerRenderer;
    }
    #endregion

    #region Visuals
    // Code in here isn't synchronized between players. That mean no important logic that would affect the gameplay should be here.
    private void Visuals()
    {
        // Counter
        tailTimeCount = Mathf.Clamp(tailTimeCount - Time.deltaTime, 0.0f, tailSpawnInterval);
        
        // Draw aferimage
        if (tailTimeCount == 0.0f && rigidbody.velocity.y > tailSpawnVelocity)
        {
            tailTimeCount = tailSpawnInterval; //reset timer
            playerRenderer.CreateAfterImage();
        }
    }
    #endregion
}
