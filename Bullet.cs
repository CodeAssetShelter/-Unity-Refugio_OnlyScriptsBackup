using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 1.5f;
    public Transform baseTransform;

    private Vector3 direction;
    private Transform myParent;

    private void Awake()
    {
        myParent = transform.parent;        
    }
    void OnEnable()
    {

        StartCoroutine(CMoveBullet());

        direction = Vector3.zero;
        direction.y = -1 * speed * Time.fixedDeltaTime;
        Vector3 t = myParent.TransformDirection(direction);
        transform.SetParent(baseTransform);
    }

    IEnumerator CMoveBullet()
    {
        while (true)
        {
            transform.Translate(direction, myParent);
            //transform.Translate(0, -1 * speed * Time.fixedDeltaTime, 0, baseTransform);
            yield return new WaitForFixedUpdate();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") == true || collision.CompareTag("Map") == true)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
    }
}
