using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusBullet : MonoBehaviour
{
    private float speed = 1.0f;
    private float distance = 5.0f;

    private Vector3 myStartPosition;
    private Vector3 direction = Vector3.left;

    private void OnEnable()
    {
        myStartPosition = transform.position;
    }

    public void InitBullet(float speed, float distance, Vector3 direction)
    {
        this.speed = speed;
        this.distance = distance;
        this.direction = direction;
        StartCoroutine(CorMove());
    }

    IEnumerator CorMove()
    {
        while (true)
        {
            if (Mathf.Abs(Vector3.Distance(transform.position, myStartPosition)) >= distance)
            {
                gameObject.SetActive(false);
                yield break;
            }
            transform.Translate(direction * Time.fixedDeltaTime, Space.Self);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Map") == true || collision.CompareTag("Player") == true)
        {
            gameObject.SetActive(false);
        }
    }
}
