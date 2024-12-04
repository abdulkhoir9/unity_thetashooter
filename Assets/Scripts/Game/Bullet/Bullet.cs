using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField]
    private float _bulletDistance;

    protected float _bulletDamage;

    public void SetDamage(float damage)
    {
        _bulletDamage = damage;
    }

    private Vector2 _bulletFirePosition;

    private void Awake()
    {
        _bulletFirePosition = gameObject.transform.position;
    }
    private void Update()
    {
        if (Vector2.Distance(_bulletFirePosition, gameObject.transform.position) > _bulletDistance)
        {
            Destroy(gameObject);
        }
    }
    protected abstract void OnTriggerEnter2D(Collider2D collision);
}
