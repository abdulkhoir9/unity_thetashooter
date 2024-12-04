using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public abstract class HealthController : MonoBehaviour
{
    [SerializeField]
    protected float _currentHealth;

    [SerializeField]
    protected float _maximumHealth;

    public string _sfx;

    public float RemainingHealthPercentage
    {
        get
        {
            return _currentHealth / _maximumHealth;
        }
    }

    public UnityEvent OnDeath;

    public abstract void TakeDamage(float damageValue);

    public void PlayDeathFade()
    {
        AudioManager.instance.PlaySFX(_sfx);
        StartCoroutine(DeathFade());
    }

    public IEnumerator DeathFade()
    {
        SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        Color col = renderer.color;

        for(float alpha = 1f; alpha >= 0; alpha -= .1f)
        {
            col.a = alpha;
            renderer.color = col;
            yield return new WaitForSeconds(.1f);
        }

        Destroy(gameObject);
    }
}
