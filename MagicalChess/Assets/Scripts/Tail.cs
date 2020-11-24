using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tail : MonoBehaviour
{
    private float time;
    private float lerp;
    private float initAlpha;
    private SpriteRenderer renderer;
    
    void Update()
    {
        ///speed = distance / time
        lerp += (1.0f / time) * Time.deltaTime;

        Color tmp = renderer.color;
        tmp.a = Mathf.Lerp(initAlpha, 0f, lerp);
        renderer.color = tmp;
    }

    public void Initialization(float tailTime, SpriteRenderer tailRenderer, float initialAlpha)
    {
        time = tailTime;
        renderer = tailRenderer;
        initAlpha = initialAlpha;
        lerp = 0.0f;
    }
}
