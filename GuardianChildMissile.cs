using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianChildMissile : MonoBehaviour
{
    public GuardianMissile parentScript;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Map") == true || collision.CompareTag("Player"))
        {
            parentScript.DestroyChildMissile();
            gameObject.SetActive(false);
        }
    }
}
