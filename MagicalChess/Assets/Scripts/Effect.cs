using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    private Animator anim = null;

    [SerializeField, Range(0f, 2f)] private float animationSpeed = 1.0f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.speed = animationSpeed;
    }

    public void AnimationEnd()
    {
        Destroy(gameObject);
    }
}
