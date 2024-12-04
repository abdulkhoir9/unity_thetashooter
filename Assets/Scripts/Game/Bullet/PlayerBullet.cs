using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBullet : Bullet
{
    [SerializeField]
    private float _hitLagTime;

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Bullet collided with: " + collision.gameObject);
        if (collision.GetComponent<EnemyMovement>())
        {
            HealthController healthController = collision.GetComponent<HealthController>();
            healthController.TakeDamage(_bulletDamage);

            if(collision.GetComponent<EnemyShoot>())
            {
                AudioManager.instance.PlaySFX("HitMetal");
            }
            else
            {
                AudioManager.instance.PlaySFX("HitFlesh");
            }

            StartCoroutine(HitLag(collision.gameObject, gameObject));
        }
        else if (collision.GetComponent<TilemapCollider2D>())
        {
            AudioManager.instance.PlaySFX("HitWalls");
            Destroy(gameObject);
        }
    }
    private IEnumerator HitLag(GameObject enemy, GameObject bullet)
    {
        Rigidbody2D rigidBodyEnemy = enemy.GetComponent<Rigidbody2D>();
        Rigidbody2D rigidBodyBullet = bullet.GetComponent<Rigidbody2D>();
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();

        enemyMovement.IsDamaged = true;
        rigidBodyEnemy.velocity = Vector2.zero;
        rigidBodyBullet.velocity = Vector2.zero;

        yield return new WaitForSeconds(_hitLagTime);
        enemyMovement.IsDamaged = false;

        Destroy(gameObject);
    }
}
