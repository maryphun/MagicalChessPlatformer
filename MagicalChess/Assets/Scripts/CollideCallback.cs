using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CollideCallback : MonoBehaviour
{
    [SerializeField] private bool enabled = true, disableAfterCollide = true;
    [SerializeField] private UnityEvent collisionEvent;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!enabled) { return; }

        Controller controller = collision.GetComponent<Controller>();

        if (controller != null)
        {
            // It is a player
            collisionEvent.Invoke();
        }

        enabled = !disableAfterCollide;
    }

    public bool SetEnabled(bool boolean)
    {
        enabled = boolean;
        return enabled;
    }
}
