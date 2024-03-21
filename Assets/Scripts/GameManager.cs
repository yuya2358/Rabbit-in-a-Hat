using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum MouseState
{
    Idle,
    RightHold,
    Delay,
}

public class GameManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Image BossHP;
    

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Default"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Player"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("Enemy"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Bullet"), LayerMask.NameToLayer("E_Bullet"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("E_Bullet"), true);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("E_Bullet"), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("E_Bullet"), true);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))
        {
            SlowDown();
        }
        else SetSlowDownNormal();
    }

    private void LateUpdate()
    {
        if (FindObjectOfType<EnemyController>() != null)
        BossHP.fillAmount = FindObjectOfType<EnemyController>().hp / FindObjectOfType<EnemyController>().i_hp;

        
    }


    //Quit Menu
    public void QuitGame()
    {
        Application.Quit();
    }

    //Option Menu
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    //Slow Motion
    public void SlowDown()
    {
        Time.timeScale = Mathf.Lerp(1, 0.1f, 5);

    }
    public void SetSlowDownNormal()
    {
        Time.timeScale = Mathf.Lerp(0.1f, 1f, 5);

    }

}
