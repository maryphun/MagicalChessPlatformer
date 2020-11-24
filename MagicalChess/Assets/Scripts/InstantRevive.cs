using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantRevive : MonoBehaviour
{
    private Camera cam = null;
    private Collider2D collider = null;
    private Rigidbody2D rb = null;
    private TrailRenderer trail = null;

    private void Awake()
    {
        cam = Camera.main;
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        trail = GetComponentInChildren<TrailRenderer>();
    }

    private void LateUpdate()
    {
        Vector3 viewPos = cam.WorldToViewportPoint(new Vector3(transform.position.x, transform.position.y + collider.bounds.extents.y, transform.position.z));
        if (viewPos.y < -0.0f)
        {
            //character is below the screen

            transform.position = new Vector2(transform.position.x, transform.position.y + 10f + (collider.bounds.extents.y * 4));
            rb.velocity = new Vector2(rb.velocity.x, 5.001f);  //not set velocity.y to exact 0 because that will allow player to jump
            trail.Clear();
        }
    }
    


        //private bool IsObjectInSight(Transform transform, Camera cam)
        //{
        //    Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
        //    if (viewPos.x >= 0 && viewPos.x <= 1
        //        && viewPos.y >= 0 && viewPos.y <= 1
        //        && viewPos.z > 0)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
