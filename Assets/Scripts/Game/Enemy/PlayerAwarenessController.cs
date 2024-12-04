using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAwarenessController : MonoBehaviour
{
    public bool AwareOfPlayer;

    //public Vector2 DirectionToPlayer;
    public Vector2 enemyToPlayerVector;

    [SerializeField]
    private float _playerAwarenessDistance;

    private Transform _player;
    public Transform getPlayer() { return _player; }

    private void Awake()
    {
        _player = FindObjectOfType<PlayerMovement>().transform;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_player == null)
        {
            AwareOfPlayer = false;

            return;
        } 

        enemyToPlayerVector = _player.position - transform.position;
        //DirectionToPlayer = enemyToPlayerVector.normalized;

        if(enemyToPlayerVector.magnitude <= _playerAwarenessDistance)
        {
            AwareOfPlayer = true;
        } 
        else
        {
            AwareOfPlayer= false;
        }
    }
}
