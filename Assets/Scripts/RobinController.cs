using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum RobinState
{
    Idle,
    Walk,
    Stab,
    Dash,
    Throw,
}

public class RobinController : MonoBehaviour
{
    RobinState currentState = RobinState.Idle;

    int randomNumber;

    float delayTimer;
    float delayInterval = 3f;

    float moveSpeed;
    float moveSpeedWalk;
    float moveSpeedDash;
    float walkOffset = 1f;

    float walkTimer;
    float walkTimerMax = 2f;
    float throwForce = 5f;
    bool needDashDirection = true;

    bool speedBreak = false;

    Camera _camera;

    Rigidbody2D rb;

    Vector3 playerPosition;
    Vector2 direction;

    public Transform attackPos;
    public LayerMask playerLayer;
    public TextMeshProUGUI bossState;
    public GameObject moneyBag;
    [HideInInspector] public bool usingDirection;
    [HideInInspector] public float d_attackRange; //debugging perpose

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _camera = Camera.main;
        moveSpeed = 0f;
        moveSpeedWalk = 2f;
        moveSpeedDash = moveSpeedWalk * 3;
    }

    void Update()
    {
        switch (currentState)
        {
            case RobinState.Idle:
                Delay();
                break;
            case RobinState.Walk:
                Walk();
                break;
            case RobinState.Stab:
                Stab();
                break;
            case RobinState.Dash:
                Dash();
                break;
            case RobinState.Throw:
                Throw();
                break;
        }

        if (currentState != RobinState.Dash)
        {
            setDirection();
            needDashDirection = true;
        }
        
        Debugging();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
    }

    private void Attack(float attackRange, bool isUsingDirection)
    {
        d_attackRange = attackRange;
        usingDirection = isUsingDirection;

        Collider2D[] targetToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, playerLayer);
        for (int i = 0; i < targetToDamage.Length; i++)
        {
            targetToDamage[i].GetComponent<PlayerAttack>().TakeDamage(1);
        }
    }

    private void Delay()
    {
        moveSpeed = 0f;

        delayTimer += Time.deltaTime;

        if (delayTimer >= delayInterval)
        {
            delayTimer = 0f;
            randomNumber = Random.Range(2, System.Enum.GetValues(typeof(RobinState)).Length + 1);
            switch (randomNumber)
            {
                case 2:
                    currentState = RobinState.Walk;
                    break;
                case 3:
                    currentState = RobinState.Dash;
                    break;
                case 4:
                    currentState = RobinState.Throw;
                    break;
                case 5:
                    currentState = RobinState.Throw;
                    break;
            }
        }
    }

    private void Walk()
    {
        walkTimer += Time.deltaTime;

        if (speedBreak || walkTimer >= walkTimerMax)
        {
            currentState = RobinState.Stab;
        }
        else moveSpeed = moveSpeedWalk;
    }

    private void Stab()
    {
        moveSpeed = 0f;

        Attack(1f, true);

        currentState = RobinState.Idle;
    }

    private void Dash()
    {
        moveSpeed = moveSpeedDash;

        if (needDashDirection)
        {
            setDirection();
            needDashDirection = false;
        }

        Vector2 leftEdge = _camera.ScreenToWorldPoint(Vector2.zero);
        Vector2 rightEdge = _camera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

        
        if (Mathf.Abs(leftEdge.x - transform.position.x) <= walkOffset * 2
            || Mathf.Abs(rightEdge.x - transform.position.x) <= walkOffset * 2)
        {
            currentState = RobinState.Idle;
        }
        

        if (speedBreak)
        {
            currentState = RobinState.Stab;
        }

        Attack(1f, false);
    }

    private void Throw()
    {
        GameObject moneyBagObj = Instantiate(moneyBag, transform.position, Quaternion.identity);

        if (moneyBagObj.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
        {
            Vector2 direction = (FindObjectOfType<PlayerMovement>().transform.position - moneyBagObj.transform.position).normalized;
            rb.AddForce(new Vector2(direction.x, 3f) * throwForce, ForceMode2D.Impulse);
        }

        currentState = RobinState.Idle;
    }

    void setDirection()
    {
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;
        direction = playerPosition - transform.position;
        direction.Normalize();
        direction.y = 0f;
    }

    private void Debugging()
    {
        bossState.text = currentState.ToString();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPos.position, d_attackRange);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            speedBreak = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            speedBreak = false;
        }
    }
}
