using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour
{
    // Missile Mode (Not Use)
    //public GameObject guardianMissilePrefab;
    //public int maxMissile = 8;

    //public GameObject guardianEye;

    //private List<GuardianMissile> guardianMissiles;

    //public float shotSpeed = 1.2f;
    //public float shotDelay = 0.5f;
    //public float gapRotation = 30f;

    // TEMP
    private Transform player;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;

        // Missile Mode (Not Use)
        // guardianMissiles = new List<GuardianMissile>();
    }

    private void OnEnable()
    {
        // Missile Mode (Not Use)
        //if (guardianMissiles.Count < maxMissile)
        //    StartCoroutine(CorCreateMissile());
        //StartCoroutine(CorFireMissile());
    }

    // Missile Mode (Not Use)
    //IEnumerator CorCreateMissile()
    //{
    //    while (guardianMissiles.Count < maxMissile)
    //    {
    //        guardianMissiles.Add
    //            (Instantiate(guardianMissilePrefab, guardianEye.transform.position,
    //                         Quaternion.identity, transform).GetComponent<GuardianMissile>());
    //        yield return null;
    //    }
    //}

    // Missile Mode (Not Use)
    //IEnumerator CorFireMissile()
    //{
    //    Vector3 direction = Vector3.up;
    //    Vector3 trackingVector = direction - new Vector3(0,0,0);
    //    float xDir = gapRotation * -2.5f;
    //    yield return new WaitForSeconds(1.0f);
    //    while (true)
    //    {
    //        if (guardianMissiles.Count != 0)
    //        {
    //            for(int i = 0; i < guardianMissiles.Count; i++)
    //            {
    //                if (guardianMissiles[i].gameObject.activeSelf == false)
    //                {
    //                    direction = Vector3.up;
    //                    // newVector.x += 
    //                    // mapSpeed * (estimate collide frame * mapSpeed) * fixedDeltaTime
    //                    float temp = xDir + (gapRotation * Random.Range(0, 5));
    //                    Debug.Log(temp);
    //                    direction = Quaternion.Euler(0, 0, temp) * direction;
    //                    guardianMissiles[i].SetMissile(guardianEye.transform, direction, shotSpeed);
    //                    yield return new WaitForSeconds(shotDelay);
    //                }
    //            }
    //        }
    //        yield return new WaitForSeconds(shotDelay);
    //    }
    //}
}
