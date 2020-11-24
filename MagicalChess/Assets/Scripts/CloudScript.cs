using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CloudScript : MonoBehaviour
{
    [SerializeField] private float speed = 0.5f, direction = 1f, maxDistance = 10f;
    private float distMoved;
    void FixedUpdate()
    {
        distMoved += speed * Time.deltaTime;
        if (distMoved > maxDistance)
        {
            distMoved = 0.0f;
            direction *= -1f;
        }

        transform.DOMoveX(transform.position.x - (speed * direction) * Time.deltaTime, Time.deltaTime, false);
    }
}
