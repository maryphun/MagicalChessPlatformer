using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float maxHP = 25f;
    [SerializeField] float shakingThreshold = 0.4f;
    [SerializeField] float regenerateRate = 0.25f;   //regenerate speed. the value stick to 1
    [SerializeField] float shakingMaxMagnitude = 0.05f;
    [SerializeField] bool regenerateDuringHit = false;

    [SerializeField] private float currentHP;
    private EnemyRenderer rendererScript;
    private Collider2D collider;
    private float regenerateCounter = 0.0f;
    private bool isAlive;

    // Start is called before the first frame update
    void Start()
    {
        currentHP = maxHP;
        isAlive = true;
        rendererScript = GetComponentInChildren<EnemyRenderer>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) { return; }

        if (currentHP < maxHP)
        {
            regenerateCounter += Time.deltaTime;
        }
        if (regenerateCounter > regenerateRate)
        {
            regenerateCounter = 0.0f;
            HitPointChange(1f);
        }
    }

    public void TakeDamage()
    {
        HitPointChange(-1f);
        if (!regenerateDuringHit)
        {
            regenerateCounter = 0.0f;
        }

        // Death
        if (currentHP <= 0.0f)
        {
            Death();
        }
    }

    private void HitPointChange(float value)
    {
        currentHP = Mathf.Clamp(currentHP + value, 0.0f, maxHP);
        rendererScript.SetRedAlpha(0.75f * (1f - (currentHP / maxHP)));
        if ((currentHP / maxHP) < shakingThreshold)
        {
            rendererScript.SetShakeValue(shakingMaxMagnitude * (1f - (currentHP / maxHP)));
        }
        else
        {
            rendererScript.SetShakeValue(0.0f);
        }
    }

    private void Death()
    {
        isAlive = false;
        collider.enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        rendererScript.DeathEffect();
    }
}
