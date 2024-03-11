using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public GameObject target;

    public void playAnimation(string animationClip)
    {
        Animator animator = target.GetComponent<Animator>();
        animator.Play(animationClip);
    }

    public void ActiveMenu(GameObject target)
    {
        StartCoroutine(ActivateMenuWait(target, 0.8f));
    }

    IEnumerator ActivateMenuWait(GameObject target, float time)
    {
        yield return new WaitForSeconds(time);
        target.SetActive(true);
    }

}
