using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianMissile : MonoBehaviour
{
    public GameObject center;

    private Transform parent;
    private Vector3 direction;
    private float shotSpeed;
    private int myChilds;

    public void SetMissile(Transform eyeTransform, Vector3 direction, float shotSpeed)
    {
        gameObject.SetActive(true);

        this.parent = eyeTransform;
        this.direction = direction;
        this.shotSpeed = shotSpeed;
        myChilds = center.transform.childCount;

        for(int i = 0; i < myChilds; i++)
        {
            center.transform.GetChild(i).gameObject.SetActive(true);
        }

        StartCoroutine(CorMoveMissile());
    }

    public void DestroyChildMissile()
    {
        myChilds--;
        if (myChilds <= 0)
        {
            transform.localPosition = parent.localPosition;
            gameObject.SetActive(false);
        }
    }

    IEnumerator CorMoveMissile()
    {
        while (true)
        {
            transform.Translate(direction * shotSpeed * Time.fixedDeltaTime, Space.World);
            yield return new WaitForFixedUpdate();
        }
    }
}
