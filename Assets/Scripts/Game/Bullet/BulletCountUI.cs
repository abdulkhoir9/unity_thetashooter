using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BulletCountUI : MonoBehaviour
{
    private TMP_Text _bulletCountText;

    private void Awake()
    {
        _bulletCountText = GetComponent<TMP_Text>();
    }

    public void UpdateBulletCountText(PlayerShoot playerShootScript)
    {
        if(playerShootScript._isReloading)
        {
            if (playerShootScript._bulletCount == 0)
                _bulletCountText.text = "Bullet empty, Reloading...";
            else
                _bulletCountText.text = "Reloading...";
        }
        else
        {
            _bulletCountText.text = $"Bullet: { playerShootScript._bulletCount } ";
        }
    }
}
