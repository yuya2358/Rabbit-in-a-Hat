using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CardController : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    float moveSpeed = 30f;
    Vector3 initialMousePosition;
    Vector3 direction;
    GameObject player;

    public int cardType;

    void Start()
    {
        initialMousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
        direction = (initialMousePosition - transform.position);
        direction.z = 0; //IMPORTANT!!
        direction.Normalize();
        
        Destroy(gameObject, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
