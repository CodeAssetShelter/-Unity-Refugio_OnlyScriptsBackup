using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalObstacleTrigger : MonoBehaviour
{
    public GameObject body;

    [Range(-0.8f, 0.8f)]
    public float upValue = 0;
    [Range(0.5f, 2.0f)]
    public float upSpeed = 0.5f;

    private Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = body.transform.localPosition;
    }

    private void OnEnable()
    {
        body.transform.localPosition = originalPos;
    }

    IEnumerator CActiveObstacle()
    {
        float upProcess = 0;
        Vector3 start = body.transform.localPosition;
        Vector3 end = body.transform.localPosition;
        end.y = upValue;
        while (true)
        {
            upProcess += Time.fixedDeltaTime * upSpeed;
            body.transform.localPosition = Vector3.Lerp(start, end, upProcess);

            if (upProcess >= 1.0f)
            {
                upProcess = 0;
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameManager") == true)
        {
            StartCoroutine(CActiveObstacle());
        }
    }
}
