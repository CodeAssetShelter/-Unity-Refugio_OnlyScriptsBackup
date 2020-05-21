using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public ControllerPlayer controllerPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") == true)
            controllerPlayer.ShieldCollideBonus();

        if (collision.CompareTag("Map") == true)
            controllerPlayer.ShieldCollideBonus();
    }
}
