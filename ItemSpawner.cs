using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Range(0, 100)]
    public int itemActiveChance = 100;

    Transform myTransform;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        myTransform = transform;
        StartCoroutine(CorActiveItems());
    }

    IEnumerator CorActiveItems()
    {
        int childCount = transform.childCount;
        int i = 0;
        while (i < childCount)
        {
            if (Random.Range(0, 100) < itemActiveChance)
                myTransform.GetChild(i).gameObject.SetActive(true);
            else myTransform.GetChild(i).gameObject.SetActive(false);

            i++;
            yield return null;
        }
    }
}
