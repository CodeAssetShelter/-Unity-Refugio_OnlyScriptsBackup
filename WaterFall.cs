using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFall : MonoBehaviour
{
    public float waterFallSpeed = 1.0f;

    private GameObject player;

    IEnumerator CorActiveFalldown()
    {
        while (true)
        {
            if (player == null) yield break;

            player.transform.Translate(Vector3.down * Time.fixedDeltaTime * waterFallSpeed, Space.World);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(collision != null)
            {
                player = collision.gameObject;
                StartCoroutine(CorActiveFalldown());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision != null)
            {
                player = collision.gameObject;
                StopAllCoroutines();
            }
        }
    }
}
