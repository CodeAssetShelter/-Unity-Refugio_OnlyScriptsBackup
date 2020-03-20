using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int myScore = 10;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("GameManager"))
        {
            transform.SetParent(MapManager.Instance.transform);
            transform.position = new Vector3(0, 40, 10);
            gameObject.SetActive(false);
        }
        if (collision.CompareTag("Player"))
        {
            transform.SetParent(MapManager.Instance.transform);
            transform.position = new Vector3(0, 40, 10);
            gameObject.SetActive(false);
            GameManager.Instance.ScoreAdd(myScore);
        }
    }
}
