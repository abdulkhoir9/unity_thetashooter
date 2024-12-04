using System.Collections;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [SerializeField]
    private GameObject _bulletPrefab;

    [SerializeField]
    private float _bulletSpeed;

    public Transform _firePoint;

    [SerializeField]
    protected float _timeBetweenShots;

    [SerializeField]
    private float _bulletDamage;

    [SerializeField]
    protected int _maxBulletCount;

    [SerializeField]
    protected float _reloadTime;

    public int _bulletCount;
    public string _sfx;
    //public bool _fireContinuously;
    protected float _lastFireTime;

    [HideInInspector]
    public bool _isReloading;

    protected void FireBullet()
    {
        //Debug.LogWarning(_bulletCount);
        //_bulletCount--;
        //Debug.Log(_bulletCount);
        //OnBulletCountChanged.Invoke();
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, transform.rotation);
        AudioManager.instance.PlaySFX(_sfx);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.SetDamage(_bulletDamage);

        Rigidbody2D rigidbody = bullet.GetComponent<Rigidbody2D>();
        rigidbody.velocity = _bulletSpeed * transform.up;
    }

    //private void OnFire(InputValue inputValue)
    //{
    //    _fireContinuously = inputValue.isPressed;
    //}

    protected virtual void UpdateBulletCount()
    {
        _bulletCount--;
    }

    protected virtual IEnumerator Reload()
    {
        _isReloading = true;
        //Debug.LogError("Reloading...");

        yield return new WaitForSeconds(_reloadTime);
        _bulletCount = _maxBulletCount;

        _isReloading = false;
    }
}
