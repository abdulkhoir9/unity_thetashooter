using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _enemyArray;

    private void Start()
    {
        SetEnemyActiveState(false);
    }

    private void SetEnemyActiveState(bool activeState)
    {
        for (int i = 0; i < _enemyArray.Length; i++)
        {
            _enemyArray[i].SetActive(activeState);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<EnemyHealthController>() == null && collision.GetComponent<Bullet>() == null)
        {
            SetEnemyActiveState(true);
            gameObject.SetActive(false);
        }
    }
}
