using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : HealthController
{
    [SerializeField]
    private int _point;

    public override void TakeDamage(float damageValue)
    {
        // do nothing if no health left
        if (_currentHealth == 0)
            return;

        // decrease health by damage value
        _currentHealth -= damageValue;

        // change health value to 0 if it's negative
        if (_currentHealth < 0)
            _currentHealth = 0;

        // if health = 0, set enemy death position as the healthbox spawn position, then invoke OnDeath event
        if (_currentHealth == 0)
        {
            LevelController.AddScore(_point);
            ScoreUI.UpdateScoreText();
            LevelController.SetHealthBoxSpawnPosition(transform.position);
            if(LevelController._score >= (LevelController._spawnCount + 1) * LevelController._scoreToSpawn)
            {
                LevelController.SpawnHealthBox();
            }
            OnDeath.Invoke();
        }
    }
}
