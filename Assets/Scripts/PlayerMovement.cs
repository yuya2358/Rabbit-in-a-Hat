using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    normal,
    teleport,
}

public class PlayerMovement : MonoBehaviour
{
    Camera _camera;
    Rigidbody2D _rigidbody;
    Vector2 velocity;
    float inputAxis;
    TrailRenderer trailRenderer;
    Animator animator;
    CapsuleCollider2D capsuleCollider;

    [Header("Movement Variables")]
    public float movementSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float Gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);
    public bool Grounded { get; private set; }
    public bool Jumping { get; private set; }

    [Header("Dashing")]
    float dashingVelocity = 16f;
    float dashingTime = 0.1f;
    Vector2 dashingDir;
    bool isDashing = false;
    bool canDash = true;

    PlayerState currentState;
    int teleportDistance = 3;
    bool isIdle;
    bool isCharging;
    bool isCharged;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        trailRenderer = GetComponent<TrailRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        currentState = PlayerState.normal;
    }

    private void Update()
    {
        SetDirection();
        switch (currentState)
        {
            case PlayerState.normal:

                HorizontalMovement();

                Grounded = _rigidbody.Raycast(Vector2.down);

                if (Grounded)
                {
                    GroundedMovement();
                    canDash = true;
                }

                capsuleCollider.enabled = true;

                ApplyGravity();

                var dashInput = Input.GetButtonDown("Dash");

                if (dashInput && canDash)
                {
                    currentState = PlayerState.teleport;
                }

                break;

            case PlayerState.teleport:

                TeleportPreparing();

                capsuleCollider.enabled = false;

                break;
        }

        UpdateAnimation();
    }

    private void HorizontalMovement()
    {
        inputAxis = Input.GetAxis("Horizontal");
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * movementSpeed, movementSpeed * Time.deltaTime * 3);

        if (_rigidbody.Raycast(Vector2.right * velocity.x))
        {
            velocity.x = 0f;
        }

        if (velocity.x == 0f)
        {
            isIdle = true;
        }
        else isIdle = false;
    }

    private void GroundedMovement()
    {
        velocity.y = Mathf.Max(velocity.y, 0f);
        Jumping = velocity.y > 0f;

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = JumpForce;
            Jumping = true;
        }
    }

    void TeleportPreparing()
    {
        velocity = Vector3.zero;
        trailRenderer.emitting = true;
        dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }
    }

    void Teleport()
    {
        transform.position = new Vector2(dashingDir.x * teleportDistance, dashingDir.y * teleportDistance);
    }

    void TeleportEnd()
    {
        trailRenderer.emitting = false;
        currentState = PlayerState.normal;
    }

    private void DashMovement()
    {
        var dashInput = Input.GetButtonDown("Dash");

        if (dashInput && canDash)
        {
            isDashing = true;
            canDash = false;
            trailRenderer.emitting = true;
            dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (dashingDir == Vector2.zero)
            {
                dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (isDashing)
        {
            velocity = dashingDir.normalized * dashingVelocity;
            return;
        }

    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        trailRenderer.emitting = false;
        isDashing = false;
    }

    private void ApplyGravity()
    {
        bool falling = velocity.y < 0f || !Input.GetButton("Jump");
        float multiplier = falling ? 2f : 1f;
        velocity.y += Gravity * multiplier * Time.deltaTime;
        velocity.y = Mathf.Max(velocity.y, Gravity / 2f);
    }

    private void FixedUpdate()
    {
        Vector2 position = _rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = _camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        _rigidbody.MovePosition(position);
    }

    void UpdateAnimation()
    {
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();

        animator.SetInteger("PlayerState", (int)currentState);
        animator.SetBool("isIdle", isIdle);
        animator.SetBool("isCharging", playerAttack.isRightMouse);
        animator.SetBool("isCharged", playerAttack.chargingPower >= playerAttack.chargingPowerMax);
    }

    void SetDirection()
    {
        RobinController robinController = FindObjectOfType<RobinController>();
        float direction = Mathf.Sign(robinController.transform.position.x - transform.position.x);
        transform.localScale = new Vector3(direction * 0.28f, 0.28f, 0.28f);
    }
}
