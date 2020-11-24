using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRenderer : MonoBehaviour
{
    private SpriteRenderer renderer;
    private bool shakeEnabled = false;
    private float shakeMagnitude;

    private SpriteRenderer Renderer
    {
        get
        {
            if (renderer != null) { return renderer; }
            return renderer = GetComponent<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shakeEnabled)
        {
            transform.localPosition = new Vector2(Random.Range(-shakeMagnitude, shakeMagnitude), Random.Range(-shakeMagnitude, shakeMagnitude));
        }
    }

    /// <summary>
    /// 0f~5f
    /// </summary>
    public void SetShakeValue(float magnitude)
    {
        shakeEnabled = (magnitude > 0.0f);
        if (!shakeEnabled)
        {
            transform.localPosition = new Vector2(0.0f, 0.0f);
        }

        shakeMagnitude = magnitude;
    }

    public void SetRedAlpha(float value)
    {
        Renderer.material.SetFloat("_HitEffectBlend", value);
    }


    /// <summary>
    /// Graphical effect during death
    /// </summary>
    public void DeathEffect()
    {
        //hide renderer
        Renderer.material.SetFloat("_Alpha", 0.0f);

        //create fragment
        GameObject parent = new GameObject("fragment parent");
        Destroy(parent, 1.0f);
        for (int x = -3; x <= 3; x++)
        {
            for (int y = -3; y <= 3; y++)
            {
                CreateFragments(parent.transform, x, y);
            }
        }

        //camera shake
        StartCoroutine(Camera.main.GetComponent<CameraController>().CameraShake(0.3f, 0.25f));
        
    }

    public void CreateFragments(Transform parentObject, int x, int y)
    {
        //--- spawning new empty object, copying tranform ---
        GameObject fragment = new GameObject("fragment");
        fragment.transform.position = transform.position;
        fragment.transform.rotation = transform.rotation;
        fragment.transform.localScale = transform.localScale;
        fragment.gameObject.layer = 0;
        fragment.transform.parent = parentObject;
        //--- copying spriterenderer ---
        SpriteRenderer newRenderer = fragment.AddComponent<SpriteRenderer>();
        SpriteRenderer originalRenderer = GetComponent<SpriteRenderer>();
        newRenderer.sortingOrder = originalRenderer.sortingOrder + 1;
        newRenderer.sortingLayerID = originalRenderer.sortingLayerID;
        newRenderer.sprite = originalRenderer.sprite;
        newRenderer.color = originalRenderer.color;
        newRenderer.flipX = Renderer.flipX;
        newRenderer.material = originalRenderer.material;
        //--- initiating tail ---
        fragment.AddComponent<Fragment>();
        fragment.GetComponent<Fragment>().Initialization(Random.Range(0.15f, 0.25f), newRenderer, 0.04f, 1.0f, x, y);
    }
}
