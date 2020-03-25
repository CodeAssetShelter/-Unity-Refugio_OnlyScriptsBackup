using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alkanoider : MonoBehaviour
{
    public GameObject ball, begin, end;
    public float speed = 1.0f;

    private float gap = 0;

    private void OnEnable()
    {
        SetGap();
        StartCoroutine(CorMove());
    }

    IEnumerator CorMove()
    {
        float myFixedDeltaTime = Time.fixedDeltaTime;

        Vector3 position = ball.transform.position;
        while (true)
        {
            if (ball.activeSelf == true)
            {
                position = transform.position;
                position.x = ball.transform.position.x + gap;
                transform.position = position;


                if (transform.position.x > end.transform.position.x)
                {
                    position.x = end.transform.position.x;
                    transform.position = position;
                }

                if (transform.position.x < begin.transform.position.x)
                {
                    position.x = begin.transform.position.x;
                    transform.position = position;
                }

                //// Location
                //// Alkanoid Ball
                //// Go Right
                //if (transform.position.x < ball.transform.position.x)
                //{
                //    if (transform.position.x < end.transform.position.x)
                //        transform.Translate(Vector3.right * myFixedDeltaTime * speed, Space.Self);
                //}
                //// Location
                //// Ball Alkanoid
                //// Go Left
                //else
                //{
                //    if (transform.position.x > begin.transform.position.x)
                //        transform.Translate(Vector3.left * myFixedDeltaTime * speed, Space.Self);
                //}
            }
            yield return new WaitForSeconds(myFixedDeltaTime);
        }
    }

    private void SetGap()
    {
        gap = Random.Range(-0.3f, 0.3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Ball")
        {
            SetGap();
        }
    }

}
