using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneMan : MonoBehaviour
{
    public float circleSize = 3;
    public GameObject head;

    public GameObject stonesParent;
    public float stoneSpeed = 2.0f;
    public float rotateSpeed = 5.0f;

    private Transform[] stoneChildren;

    private void Awake()
    {
        stoneChildren = new Transform[stonesParent.transform.childCount];
        for(int i = 0; i < stonesParent.transform.childCount; i++)
        {
            stoneChildren[i] = stonesParent.transform.GetChild(i);
        }
    }

    private void OnEnable()
    {        
        StartCoroutine(CorMove());
        StartCoroutine(CorRotateStones());
    }

    int rotatePlay = 1;
    IEnumerator CorRotateStones()
    {
        Invoke("SetRotateState", 1.0f);
        Vector3 vector = Vector3.up;

        for (int i = 0; i < stoneChildren.Length; i++)
        {
            stoneChildren[i].transform.Rotate(0, 0, (360/stoneChildren.Length) * i, Space.Self);
        }

        while (true)
        {
            stonesParent.transform.Rotate(0, 0, rotateSpeed * Time.fixedDeltaTime);
            vector.y = rotatePlay;
            for (int i = 0; i < stoneChildren.Length; i++)
            {
                stoneChildren[i].transform.Translate(stoneSpeed * Time.fixedDeltaTime * vector, Space.Self);
            }                   
            yield return new WaitForFixedUpdate();
        }
    }
    int move = 1;
    private void SetRotateState()
    {
        switch (rotatePlay)
        {
            case -1: move = 1; Invoke("SetRotateState", 1.0f); break;
            case 0:
                Invoke("SetRotateState", 1.0f);
                break;
            case 1: move = -1; Invoke("SetRotateState", 1.0f); break;
        }
        rotatePlay += move;
    }

    IEnumerator CorMove()
    {
        Vector3 fixedPoint = transform.localPosition;
        float angularSpeed = 1f;
        float currentAngle = 0;

        while (true)
        {
            currentAngle += angularSpeed * Time.deltaTime;
            Vector3 offset = new Vector3 (Mathf.Sin(currentAngle), Mathf.Cos(currentAngle)) * circleSize;
            transform.localPosition = fixedPoint + offset;
            yield return new WaitForFixedUpdate();
        }
    }
}
