using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public OctopusBullet bulletPrefab;
    public float moveSpeed = 1.0f;
    public float bulletSpeed = 1.0f;
    public float distance = 5.0f;

    private SpriteRenderer spriteRenderer;
    private Vector3 mySpawnPosition;

    private int maxBullet = 3;
    private List<OctopusBullet> bullets;

    private void Awake()
    {
        bullets = new List<OctopusBullet>();

        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        mySpawnPosition = transform.localPosition;
    }

    private void OnEnable()
    {
        gameObject.SetActive(true);
        transform.localPosition = mySpawnPosition;
        StartCoroutine(CorMove());
        if (bullets.Count < maxBullet)
            StartCoroutine(CorCreateBullet());
        else
        {
            StartCoroutine(CorResetBullet());
        }
    }
    IEnumerator CorResetBullet()
    {
        int i = 0;
        while (i < bullets.Count)
        {
            bullets[i++].gameObject.SetActive(false);
            yield return null;
        }
    }


    IEnumerator CorCreateBullet()
    {
        while(bullets.Count < maxBullet)
        {
            OctopusBullet temp = Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform.parent);
            temp.gameObject.SetActive(false);
            bullets.Add(temp);
            yield return null;
        }
    }

    bool moving = false;
    bool isCollideToMap = false;
    float angle = 0;
    IEnumerator CorMove()
    {
        Vector3 direction = Vector3.up ;
        moving = false;

        while (true)
        {
            if (moving == false)
            {
                if (isCollideToMap == false)
                {
                    angle = Random.Range(0, 360);
                }
                if (angle > 180 && angle < 360)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
                direction = Quaternion.Euler(0, 0, angle) * Vector3.up;
                isCollideToMap = false;

                MoveStop();
            }
            if (moving == true)
            {
                transform.Translate(direction * Time.fixedDeltaTime * moveSpeed, Space.Self);                
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void MoveStop()
    {
        moving = true;
        moveSpeed = 0;
        OctopusBullet bullet = null;
        for (int i = 0; i < bullets.Count; i++)
        {
            bullet = bullets[i];
            if (bullet.gameObject.activeSelf == false)
            {
                bullet.transform.position = transform.position;
                bullet.gameObject.SetActive(true);
                bullet.InitBullet(bulletSpeed, distance,
                    Quaternion.Euler(0, 0, Random.Range(-45, 46)) * Vector3.down);
                Invoke("SetMove", 1.0f);
                return;
            }
        }
        Invoke("SetMove", 1.0f);
    }
    private void SetMove()
    {
        moveSpeed = 1;
        moving = true;
        Invoke("SetStop", 1.0f);
    }
    private void SetStop()
    {
        moveSpeed = 0;
        moving = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Map") == true)
        {
            isCollideToMap = true;
            if (angle < 90 || angle > 270)
            {
                angle = Random.Range(91, 269);
            }
            else
            {
                angle = Random.Range(271, 445);
            }
        }
    }
}
