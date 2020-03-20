using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    public GameObject platformBody;

    public GameObject start, end;
    public float speed = 1.0f;
    private void OnEnable()
    {
        platformBody.transform.localPosition = (start.transform.position + end.transform.position) * 0.5f;
        StartCoroutine(CMovePlatform());
    }

    IEnumerator CMovePlatform()
    {
        float moveProcess = 0;
        float moveDirection = 1;
        while (true)
        {
            moveProcess += Time.fixedDeltaTime * moveDirection * speed;
            platformBody.transform.localPosition = Vector3.Lerp(start.transform.localPosition, end.transform.localPosition, moveProcess);


            if (moveProcess >= 1.0f) moveDirection = -1;
            if (moveProcess <= 0) moveDirection = 1;

            yield return new WaitForFixedUpdate();
        }
    }
}
