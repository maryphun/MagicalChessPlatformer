using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    private float lerp = 1.0f, lerpIncrement;
    private Vector2 originalPoint, targetPoint;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        //disable update
        enabled = false;
    }

    /// <summary>
    /// move camera to target position over time. smooth movement with lerp
    /// </summary>
    public void MoveCameraOverTime(Vector2 targetPosition, float time)
    {
        lerp = 0.0f;
        lerpIncrement = (1.0f / time) * Time.deltaTime;
        originalPoint = transform.position;
        targetPoint = targetPosition;
        //enable update
        enabled = true;
    }

    private void LateUpdate()
    {
        if (lerp != 1.0f)
        {
            Vector2 tempPos = Vector2.Lerp(originalPoint, targetPoint, lerp);
            transform.position = new Vector3(tempPos.x, tempPos.y, transform.position.z);
            lerp = Mathf.Clamp(lerp + lerpIncrement, 0.0f, 1.0f);
        }
        else
        {
            //disable update
            transform.position = new Vector3(targetPoint.x, targetPoint.y, transform.position.z); ;
            enabled = false;
        }
    }

    public IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
