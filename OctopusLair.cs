using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusLair : MonoBehaviour
{
    [Header("- Prefab")]
    public Octopus octopusPrefab;
    public GameObject endPoint;


    [Header("- Components")]
    public Animator animator;


    [Header("- Octopus")]
    public int maxOctopus = 8;
    public float speed = 1.0f;

    [Range(0, 100)]
    public int probability = 50;

    [Range(2.0f, 10f)]
    public float spawnTimer = 3.0f;
    private Vector3 direction = Vector3.up;
    private float distance = 10f;
    private List<Octopus> octopusModum;


    private void Awake()
    {
        octopusModum = new List<Octopus>();
    }

    private void OnEnable()
    {
        direction = endPoint.transform.position - transform.position;
        distance = Vector3.Distance(endPoint.transform.localPosition, transform.localPosition);
        distance = Mathf.Abs(distance);

        if (octopusModum.Count < maxOctopus)
        {
            StartCoroutine(CorBornOctopus());
        }
        else
        {
            StartCoroutine(CorResetOctopus());
        }
        StartCoroutine(CorSortieOctopus());
    }

    IEnumerator CorBornOctopus()
    {
        while (octopusModum.Count < maxOctopus)
        {
            Octopus temp = Instantiate(octopusPrefab, transform.position, Quaternion.identity, transform);
            temp.gameObject.SetActive(false);
            octopusModum.Add(temp);

            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator CorResetOctopus()
    {
        if (octopusModum.Count == 0) yield break;

        int i = 0;

        while (i < octopusModum.Count)
        {
            octopusModum[i].transform.position = transform.position;
            octopusModum[i++].gameObject.SetActive(false);
            yield return null;
        }
    }

    Octopus sortie = null;
    IEnumerator CorSortieOctopus()
    {
        yield return new WaitForSeconds(spawnTimer);

        while (true)
        {
            sortie = null;
            for (int i = 0; i < octopusModum.Count; i++)
            {
                if (octopusModum[i].gameObject.activeSelf == false)
                {
                    sortie = octopusModum[i];
                    break;
                }
            }
            if (sortie == null)
            {
                yield return new WaitForSeconds(spawnTimer);
            }
            else
            {
                if (probability > Random.Range(0, 100))
                    animator.SetBool("Create", true);
            }
            yield return new WaitForSeconds(spawnTimer);
        }
    }

    private void SortieOctopus()
    {
        sortie.transform.position = transform.position;
        sortie.InitOctopus(direction.normalized, speed, endPoint.transform.localPosition.y);
        animator.SetBool("Create", false);
    }
}
