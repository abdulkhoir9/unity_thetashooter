using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [SerializeField]
    private float _healthToAdd;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerMovement>())
        {
            AudioManager.instance.PlaySFX("HealthPickUp");
            collision.GetComponent<PlayerHealthController>().AddHealth(_healthToAdd);

            Destroy(gameObject);
        }
    }
}
