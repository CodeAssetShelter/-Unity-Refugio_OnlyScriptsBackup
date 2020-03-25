using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    private float fallDownSpeed = 1.0f;
    private float fallDownMinSpeed = 0.5f;
    private float fallDownMaxSpeed = 5.0f;
    public SpriteRenderer spriteRenderer;
    private Vector3 startPos = Vector3.zero;
    private struct XPos { public float x1, x2; }
    private XPos xPos;

    public void Init(float x1, float x2, float speed, Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        fallDownMaxSpeed = speed;
        fallDownSpeed = Random.Range(fallDownMinSpeed, fallDownMaxSpeed);

        startPos = transform.localPosition;

        xPos.x1 = x1;
        xPos.x2 = x2;
        Debug.Log(xPos.x1 + " // " + xPos.x2);
        StartCoroutine(CFallDown());
    }

    IEnumerator CFallDown()
    {
        while (true)
        {
            transform.Translate(0, Time.fixedDeltaTime * fallDownSpeed * (-1), 0);
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BlockEndLine") == true)
        {
            fallDownSpeed = Random.Range(fallDownMinSpeed, fallDownMaxSpeed);
            startPos.x = Random.Range(xPos.x1, xPos.x2);
            transform.localPosition = startPos;
        }
    }
}
