using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

    public void ToGetReady()
    {
        SceneManager.LoadScene("GetReady");
    }
    public void ToTitle()
    {
        SceneManager.LoadScene("Title");
    }
    public void ToRobinTheRobber()
    {
        SceneManager.LoadScene("RobinTheRobber");
    }
}
