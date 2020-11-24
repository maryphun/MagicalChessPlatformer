using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    [SerializeField, Range(0.0f, 10.0f)] float lastingTime = 3.0f;
    [SerializeField] private ParticleSystem[] particleList;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf(lastingTime));
    }

    IEnumerator DestroySelf(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void SetParticleColor(Color targetColor)
    {
        foreach (ParticleSystem particle in particleList)
        {
            particle.startColor = targetColor;
        }
    }
}
