using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fragment : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private float _rotateSpeed, _rotateCount, _alphaCount, _time, _moveSpeed;
    private Vector3 moveVec;

    public void Update()
    {
        //Moving
        transform.position += moveVec * Time.deltaTime * _moveSpeed;

        //Spinning
        _rotateCount += _rotateSpeed;
        if (_rotateCount > 1.0f) { _rotateCount -= 1.0f; }

        float newRotation = Mathf.Lerp(0.0f, 6.2831f, _rotateCount);
        // from 0 to 0.62831
        _renderer.material.SetFloat("_RotateUvAmount", newRotation);
        
        //Fading
        ///speed = distance / time
        _alphaCount += (1.0f / _time) * Time.deltaTime;
        
        float newAlpha = Mathf.Lerp(1.0f, 0f, _alphaCount);
        _renderer.material.SetFloat("_Alpha", newAlpha);
    }

    public void Initialization(float spinTime, SpriteRenderer renderer, float fragmentSize, float lastingTime, int x, int y)
    {
        _rotateCount = 0.0f;
        _rotateSpeed = spinTime;
        _renderer = renderer;
        _renderer.material.SetFloat("_Alpha", 1.0f);
        _renderer.material.EnableKeyword("CLIPPING_ON");
        _renderer.material.EnableKeyword("ROTATEUV_ON");
        _renderer.material.EnableKeyword("OFFSETUV_ON");
        _renderer.material.DisableKeyword("HITEFFECT_ON");
        _renderer.material.SetFloat("_ClipUvLeft", 0.5f - fragmentSize);
        _renderer.material.SetFloat("_ClipUvRight", 0.5f - fragmentSize);
        _renderer.material.SetFloat("_ClipUvUp", 0.5f - fragmentSize);
        _renderer.material.SetFloat("_ClipUvDown", 0.5f - fragmentSize);
        _renderer.material.SetFloat("_OffsetUvX", x * 0.035f);
        _renderer.material.SetFloat("_OffsetUvY", y * 0.035f);

        moveVec = (new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), 0.0f)).normalized;
        _moveSpeed = Random.Range(12.0f, 13.0f);
        // = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0.0f);

        _time = lastingTime;
        Destroy(gameObject, lastingTime);
    }
}
