using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyBullet : Bullet
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Bullet collided with: " +  collision.gameObject);
        if (collision.GetComponent<PlayerMovement>())
        {
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(_bulletDamage);
            AudioManager.instance.PlaySFX("HitFlesh");

            Destroy(gameObject);
        }
        else if (collision.GetComponent<TilemapCollider2D>())
        {
            AudioManager.instance.PlaySFX("HitWalls");
            Destroy(gameObject);
        }
    }
}
