using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public float movementSpeed = 7f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float JumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float Gravity => (-2f * maxJumpHeight) / Mathf.Pow((maxJumpTime / 2f), 2);
    public bool Grounded { get; private set; }
    Vector2 initialStartPosition = new Vector2(-6f, -3f);

    [Header("Dashing")]
    float dashingVelocity = 16f;
    float dashingTime = 0.1f;
    Vector2 dashingDir;
    bool isDashing = false;
    bool canDash = true;

    [Header("Teleport")]
    int teleportDistance = 5;
    public GameObject SlowMoBG;

    [SerializeField] public PlayerState currentState;
    bool isIdle;
    public TextMeshProUGUI debugText;

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

                capsuleCollider.enabled = true;
                SlowMoBG.SetActive(false);
                FindObjectOfType<GameManager>().SetSlowDownNormal();

                Grounded = _rigidbody.Raycast(Vector2.down);
                if (Grounded)
                {
                    GroundedMovement();
                    canDash = true;
                }

                var dashInput = Input.GetButtonDown("Dash");
                if (dashInput && canDash)
                {
                    currentState = PlayerState.teleport;
                }

                HorizontalMovement();
                ApplyGravity();

                break;

            case PlayerState.teleport:

                TeleportPreparing();
                capsuleCollider.enabled = false;
                SlowMoBG.SetActive(true);
                FindObjectOfType<GameManager>().SlowDown();

                break;
        }

        UpdateAnimation();
    }

    void LateUpdate()
    {
        if (debugText != null)
        debugText.text = dashingDir.ToString();
    }

    void FixedUpdate()
    {
        Vector2 position = _rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        Vector2 leftEdge = _camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        position.x = Mathf.Clamp(position.x, leftEdge.x + 0.5f, rightEdge.x - 0.5f);

        if (Grounded && velocity.y < 0) position.y = initialStartPosition.y; //Fixed the issue where the gravity gradually decreases noticeably as the falling rabbit almost reaches the ground.

        _rigidbody.MovePosition(position);
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

        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = JumpForce;
        }
    }

    void TeleportPreparing()
    {
        velocity = Vector3.zero;
        trailRenderer.emitting = true;
        dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical") - 1);
        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0);
        }
    }

    void Teleport()
    {
        transform.position = new Vector2(_rigidbody.position.x + dashingDir.x * teleportDistance, _rigidbody.position.y);
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



    void UpdateAnimation()
    {
        PlayerAttack playerAttack = GetComponent<PlayerAttack>();

        animator.SetInteger("PlayerState", (int)currentState);
        animator.SetBool("isIdle", isIdle);
        animator.SetInteger("chargingState", playerAttack.isRightMouse ? (playerAttack.chargingPower < playerAttack.chargingPowerMax ? 1 : 2) : 0);
    }

    void SetDirection()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            if (gameManager.boss != null)
            {
                float direction = Mathf.Sign(gameManager.boss.transform.position.x - transform.position.x);
                transform.localScale = new Vector3(direction * 0.28f, 0.28f, 0.28f);
            }
        }
    }
}
