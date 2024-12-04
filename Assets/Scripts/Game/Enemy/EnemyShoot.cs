using UnityEngine;

public class EnemyShoot : Shoot
{
    [SerializeField]
    private float _fireDistance;

    private Transform _player;

    private void Awake()
    {
        _bulletCount = _maxBulletCount;
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    private void Update()
    {
        try
        {
            Vector2 enemyPosition = transform.position;
            Vector2 playerPosition = _player.position;

            if (EnemyCanFire() && WithinFireDistance(enemyPosition, playerPosition) && (_bulletCount > 0))
            {
                float timeSinceLastFire = Time.time - _lastFireTime;

                if (timeSinceLastFire >= _timeBetweenShots && GetComponent<HealthController>().RemainingHealthPercentage > 0)
                {
                    _bulletCount--;
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
        catch (MissingReferenceException)
        {
            Debug.Log("Player died, stopping enemy shooting...");
            enabled = false;
        }
    }

    public bool WithinFireDistance(Vector2 enemy, Vector2 player)
    {
        return (Vector2.Distance(transform.position, _player.position) < _fireDistance);
    }

    private bool NoWallsInFiringLine(Transform origin)
    {
        // Checks line of sight in forward direction using Raycast
        RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.up, Vector2.Distance(origin.position, _player.position), LayerMask.GetMask("Walls"));
        Debug.DrawRay(origin.position, origin.up * Vector2.Distance(origin.position, _player.position), Color.red);

        return hit.collider == null;
    }

    private bool PlayerInFiringLine(Transform origin)
    {
        // Checks line of sight in forward direction using Raycast
        RaycastHit2D hit = Physics2D.Raycast(origin.position, origin.up, Vector2.Distance(origin.position, _player.position), LayerMask.GetMask("Default"));
        Debug.DrawRay(origin.position, origin.up * Vector2.Distance(origin.position, _player.position), Color.red);

        if (hit.collider != null && hit.collider.gameObject.name == "Player")
            return true;
        else
            return false;
    }

    public bool EnemyCanFire()
    {
        return NoWallsInFiringLine(_firePoint) && PlayerInFiringLine(_firePoint);
    }
}
