using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerShoot : Shoot
{
    public bool _fireContinuously;

    public UnityEvent OnBulletCountChanged;

    private Animator _animator;

    private void Awake()
    {
        _bulletCount = _maxBulletCount;
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_fireContinuously && (_bulletCount > 0) && !_isReloading)
        {
            float timeSinceLastFire = Time.time - _lastFireTime;

            if (timeSinceLastFire >= _timeBetweenShots && !PauseMenu._isPaused && !LevelController._isStopped)
            {
                UpdateBulletCount();
                FireBullet();

                _lastFireTime = Time.time;
            }
        }

        // Reload if there's no bullet left
        if (_bulletCount == 0 && !_isReloading)
        {
            StartCoroutine(Reload());
        }
    }

    private void FixedUpdate()
    {
        SetAnimation(_fireContinuously && (_bulletCount > 0) && !_isReloading);
    }

    private void SetAnimation(bool isFiring)
    {
        _animator.SetBool("isFiring", isFiring);
    }

    private void OnFire(InputValue inputValue)
    {
        _fireContinuously = inputValue.isPressed;
    }

    private void OnReload(InputValue inputValue)
    {
        if (!_isReloading && _bulletCount < _maxBulletCount)
            StartCoroutine(Reload());
    }

    protected override void UpdateBulletCount()
    {
        _bulletCount--;
        OnBulletCountChanged.Invoke();
    }

    protected override IEnumerator Reload()
    {
        _isReloading = true;
        AudioManager.instance.PlaySFX("Reload");
        OnBulletCountChanged.Invoke();
        //Debug.LogError("Reloading...");

        yield return new WaitForSeconds(_reloadTime);
        _bulletCount = _maxBulletCount;
        
        _isReloading = false;
        OnBulletCountChanged.Invoke();
    }
}
