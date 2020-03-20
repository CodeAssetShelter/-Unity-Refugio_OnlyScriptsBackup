using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    public enum State { Idle = 0, Up, Down, Fire, StateCount }
    private State state;


    public Rigidbody2D rigid2D;
    public OctopusBullet octopusBulletPrefab;
    public int maxBullet = 3;
    public float bulletSpeed = 1.0f;
    public float bulletReach = 4f;


    private List<OctopusBullet> octopusBullets = new List<OctopusBullet>();

    private float moveSpeed = 1.0f;
    private Vector3 direction = Vector3.up;
    private float distance = 10.0f;
    private Vector3 myStartPos;

    private void OnEnable()
    {
        StartCoroutine(CorResetBullet());
    }

    public void InitOctopus(Vector3 direction, float speed, float distance)
    {
        gameObject.SetActive(true);
        this.direction = direction;
        moveSpeed = speed;
        this.distance = distance;
        myStartPos = transform.localPosition;

        StartCoroutine(CorMoveOctopus());
        StartCoroutine(CorCreateBullet());
    }

    IEnumerator CorResetBullet()
    {
        if (octopusBullets.Count == 0) yield break;

        int i = 0;

        while (i < octopusBullets.Count)
        {
            octopusBullets[i].transform.position = transform.position;
            octopusBullets[i++].gameObject.SetActive(false);
            yield return null;
        }
    }

    IEnumerator CorCreateBullet()
    {
        while (octopusBullets.Count < maxBullet)
        {

            OctopusBullet temp = Instantiate(octopusBulletPrefab, transform.position, Quaternion.identity, transform);
            temp.gameObject.SetActive(false);

            octopusBullets.Add(temp);
            yield return null;
        }

    }

    IEnumerator CorMoveOctopus()
    {
        float fixedDeltatime = Time.fixedDeltaTime;
        while (true)
        {

            //if (Mathf.Abs(Vector3.Distance(transform.localPosition, myStartPos)) > distance)
            //{
            //    gameObject.SetActive(false);
            //    yield break;
            //}
            if (transform.localPosition.y >= distance)
            {
                gameObject.SetActive(false);
                yield break;
            }

            switch (state)
            {
                case State.Up:
                    rigid2D.velocity = Vector3.zero;
                    transform.Translate(direction * fixedDeltatime * moveSpeed, Space.World);
                    break;

                case State.Down:
                    transform.Translate(Vector3.down * fixedDeltatime * moveSpeed * 0.8f, Space.World);
                    break;

                case State.Fire:
                    OctopusBullet octopus = null;
                    for(int i = 0; i < octopusBullets.Count; i++)
                    {
                        octopus = octopusBullets[i];
                        if (octopus.gameObject.activeSelf == false)
                        {
                            octopus.transform.position = transform.position;
                            octopus.gameObject.SetActive(true);
                            octopus.InitBullet(bulletSpeed, bulletReach);
                        }
                    }
                    rigid2D.AddForce(Vector3.right * 0.05f, ForceMode2D.Impulse);
                    break;

                case State.Idle:
                    transform.Translate(Vector3.down * fixedDeltatime * moveSpeed * 0.1f, Space.World);
                    break;
                default:
                    break;
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetTrigger(State state)
    {
        this.state = state;
    }
}
