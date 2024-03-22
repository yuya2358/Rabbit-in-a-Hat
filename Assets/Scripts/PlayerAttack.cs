using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    float hp = 5;

    public GameObject card;
    public GameObject bigShot;
    public TextMeshProUGUI hpText;
    [HideInInspector] public bool usingDirection;

    MouseState currentState = MouseState.Idle;

    [HideInInspector]public bool isRightMouse;
    bool spawningCards;

    float spawnTimer = 0f;
    float spawnInterval = 0.2f;
    [HideInInspector] public float chargingPower = 0f;
    [HideInInspector] public float chargingPowerMax = 0.5f;

    float delayTimer = 0f;
    float delayInterval = 0.2f;

    float invincibleTime;
    float invincibleTimeInterval = 0.5f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        isRightMouse = Input.GetMouseButton(1);

        switch (currentState)
        {
            case MouseState.Idle:
                AutoCards();
                break;

            case MouseState.RightHold:
                Charging();
                break;
            case MouseState.Delay:
                Delay();
                break;
        }

        if (invincibleTime <= invincibleTimeInterval)
        {
            invincibleTime += Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (hpText != null)
        hpText.text = "HP: " + hp;
    }


    void Charging()
    {
        if (isRightMouse)
        {
            if (chargingPower < chargingPowerMax)
            {
                chargingPower += Time.deltaTime;
            }
        }
        else
        {
            if (chargingPower >= chargingPowerMax)
            {
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                Instantiate(bigShot, playerMovement.transform.position, Quaternion.identity);
            }
            chargingPower = 0;
            currentState = MouseState.Delay;
        }

        if (FindObjectOfType<PlayerMovement>().currentState == PlayerState.teleport)
        {
            chargingPower = 0;
        }
    }

    void AutoCards()
    {
        if (isRightMouse)
        {
            currentState = MouseState.RightHold;
        }
        else
        {
            spawnTimer += Time.deltaTime;

            if (spawnTimer >= spawnInterval)
            {
                PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
                Instantiate(card, playerMovement.transform.position, Quaternion.identity);
                spawnTimer = 0f;
            }

        }
    }

    void Delay()
    {
        delayTimer += Time.deltaTime;

        if (delayTimer >= delayInterval)
        {
            delayTimer = 0f;
            currentState = MouseState.Idle;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("E_Bullet"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        if (invincibleTime >= invincibleTimeInterval)
        {
            hp -= damage;
            invincibleTime = 0f;
        }
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
