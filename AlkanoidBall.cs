using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlkanoidBall : MonoBehaviour
{
    enum MoveX { Right = 1, Left = -1}
    MoveX moveX;

    public float ballSpeed = 1.0f;

    private Vector3 onEnablePosition;
    private Vector3 vector = Quaternion.Euler(0, 0, -45) * Vector3.up;

    private void Awake()
    {
        onEnablePosition = transform.localPosition;
    }

    private void OnEnable()
    {
        transform.localPosition = onEnablePosition;
        vector = Quaternion.Euler(0, 0, -45) * Vector3.up;
        moveX = MoveX.Right;

        StartCoroutine(CorMove());
    }

    IEnumerator CorMove()
    {
        while (true)
        {
            transform.Translate(vector * Time.fixedDeltaTime * ballSpeed, Space.Self);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Alkanoider") == true)
        {
            vector = collision.transform.position - transform.position;
            vector = vector.normalized;
            vector *= -1;
        }
        if (collision.CompareTag("AlkanoidWall") == true)
        {
            switch (moveX)
            {
                case MoveX.Right:
                    moveX = MoveX.Left;
                    break;
                case MoveX.Left:
                    moveX = MoveX.Right;
                    break;
            }
            vector.x *= -1;
        }
        if (collision.CompareTag("AlkanoidBlock") == true)
        {
            vector.y *= -1;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("AlkanoidWall") == true)
        {
            switch (moveX)
            {
                case MoveX.Right:
                    vector.x = 1;
                    break;
                case MoveX.Left:
                    vector.x = -1;
                    break;
            }
        }
    }
}
