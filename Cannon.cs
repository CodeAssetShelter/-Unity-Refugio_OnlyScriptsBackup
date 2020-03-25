using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    public Bullet bulletPrefab;

    [Header("- Must link Type_Type_000")]
    public Transform baseTransform;

    [Space(20)]
    public Animator efxFireAnimation;

    public int maxBullet = 5;

    [Range(0.5f, 3.5f)]
    public float fireDelay = 1.0f;

    private Vector3 startPosition;
    private List<Bullet> bullets;
    private List<int> reloadedBullets;
    // Start is called before the first frame update
    void Awake()
    {
        bullets = new List<Bullet>();
        reloadedBullets = new List<int>();

        startPosition = efxFireAnimation.transform.localPosition;
    }

    private void OnEnable()
    {
        if (bullets.Count < maxBullet)
            StartCoroutine(CCreateBullet());
        StartCoroutine(CFireBullet());
    }

    IEnumerator CCreateBullet()
    {
        int createdBullet = 0;
        while (createdBullet < maxBullet)
        {
            bullets.Add(Instantiate(bulletPrefab, transform.position, Quaternion.identity, transform));
            bullets[bullets.Count - 1].baseTransform = baseTransform;
            createdBullet++;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CFireBullet()
    {
        while (true)
        {
            if (bullets.Count != 0)
            {
                for (int i = 0; i < bullets.Count; i++)
                {
                    if (bullets[i].gameObject.activeSelf == false)
                    {
                        bullets[i].transform.position = efxFireAnimation.transform.position; 
                        bullets[i].gameObject.SetActive(true);

                        efxFireAnimation.Play(efxFireAnimation.GetLayerIndex("Base Layer"));
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(fireDelay);
        }
    }
}
