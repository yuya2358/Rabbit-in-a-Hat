using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [HideInInspector] public float i_hp;
    [HideInInspector] public float hp;

    private Camera _camera;
    private Rigidbody2D _rigidbody;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _camera = Camera.main;

        i_hp = 500f;
        hp = i_hp;
    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (hp <= i_hp)
        {

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            CardController cardController = collision.gameObject.GetComponent<CardController>();
            switch (cardController.cardType)
            {
                case 0:
                    hp--;
                    break;
                case 1:
                    hp -= 20;
                    break;
            }
        }

        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
