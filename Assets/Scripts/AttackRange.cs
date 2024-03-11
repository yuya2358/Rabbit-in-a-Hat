using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public GameObject target;

    bool usingDirection;
    Vector3 targetDirection;

    private void Start()
    {
        PlayerAttack playerController = target.GetComponent<PlayerAttack>();
        RobinController robinController = target.GetComponent<RobinController>();

        usingDirection = (playerController != null) ? playerController.usingDirection :
                   (robinController != null) ? robinController.usingDirection : false;
    }

    void Update()
    {
        targetDirection = usingDirection ? new Vector3(GetTargetDirection().x, GetTargetDirection().y, 0) : Vector2.zero;

        transform.position = target.transform.position + targetDirection;
    }

    public Vector2 GetTargetDirection() //left, right, up, down
    {
        Vector2 direction = Vector2.zero;
        Rigidbody2D rb = target.GetComponent<Rigidbody2D>();

        if (rb.velocity != Vector2.zero)
        {
            if (Mathf.Abs(rb.velocity.x) > Mathf.Abs(rb.velocity.y))
            {
                if (rb.velocity.x > 0)
                {
                    direction = new Vector2(1, 0);
                }
                else direction = new Vector2(-1, 0);
            }
            else
            {
                if (rb.velocity.y > 0)
                {
                    direction = new Vector2(0, 1);
                }
                else direction = new Vector2(0, -1);
            }
        }
        return direction;
    }
}
