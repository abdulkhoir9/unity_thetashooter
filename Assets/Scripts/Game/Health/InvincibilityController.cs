using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvincibilityController : MonoBehaviour
{
    private PlayerHealthController _healthController;

    private void Awake()
    {
        _healthController = GetComponent<PlayerHealthController>();
    }

    public void StartInvincibility(float invincibilityDuration)
    {
        StartCoroutine(InvincibilityCoroutine(invincibilityDuration));
    }

    private IEnumerator InvincibilityCoroutine(float invincibilityDuration)
    {
        _healthController.isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        _healthController.isInvincible = false;
    }
}
