using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alkanoid : MonoBehaviour
{
    public GameObject[] alkanoidBlocks;
    private const int chance = 30;

    private void OnEnable()
    {
        StartCoroutine(CorReactiveBlock());
    }

    IEnumerator CorReactiveBlock()
    {
        int i = 0;
        while (i < alkanoidBlocks.Length)
        {
            if (Random.Range(0, 100) < chance)
                alkanoidBlocks[i++].SetActive(true);
            else
                alkanoidBlocks[i++].SetActive(false);

            yield return null;
        }
    }
}
