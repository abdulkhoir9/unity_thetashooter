using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    private float _damageValue;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerMovement>())
        {
            HealthController healthController = collision.gameObject.GetComponent<HealthController>();

            healthController.TakeDamage(_damageValue);
        }
    }
}
