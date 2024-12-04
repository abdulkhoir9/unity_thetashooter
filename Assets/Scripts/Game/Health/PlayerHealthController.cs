using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealthController : HealthController
{
    public bool isInvincible;

    public UnityEvent OnDamaged;
    public UnityEvent OnHealthChanged;

    public override void TakeDamage(float damageValue)
    {
        // do nothing if no health left
        if (_currentHealth == 0)
            return;

        // decrease health by damage value
        if (isInvincible)
            return;

        _currentHealth -= damageValue;

        OnHealthChanged.Invoke();

        // change health value to 0 if it's negative
        if (_currentHealth < 0)
            _currentHealth = 0;

        // invoke OnDeath event if health = 0, otherwise start the coroutine only if this script is attached to a player object
        if (_currentHealth == 0)
            OnDeath.Invoke();
        else
            StartCoroutine(Damaged());
    }

    public void AddHealth(float addValue)
    {
        if (_currentHealth == _maximumHealth)
            return;

        _currentHealth += addValue;

        OnHealthChanged.Invoke();

        if (_currentHealth > _maximumHealth)
            _currentHealth = _maximumHealth;
    }

    public IEnumerator Damaged()
    {
        AudioManager.instance.PlaySFX("HitFlesh");
        OnDamaged.Invoke();

        SpriteRenderer renderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        renderer.color = new Color(1f, .5f, .5f, 1f);
        yield return new WaitForSeconds(.75f);
        renderer.color = Color.white;
    }
}
