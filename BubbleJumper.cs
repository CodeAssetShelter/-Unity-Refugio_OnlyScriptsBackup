using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleJumper : MonoBehaviour
{
    public Bubble bubblePrefab;
    public GameObject summonGate;
    public GameObject gravity;

    public int maxBubble = 10;
    [Range(1.0f, 3.0f)]
    public float maxMoveSpeed = 3.0f;
    [Range(1.0f, 5.0f)]
    public float maxJumpPower = 5.0f;

    private const float minMoveSpeed = 1.0f;
    public GameObject start, end;


    private Vector3 myGravity = Vector3.down;

    private List<Bubble> bubbles = new List<Bubble>();
    // Start is called before the first frame update

    void OnEnable()
    {
        myGravity = (summonGate.transform.position - gravity.transform.position).normalized;

        if (bubbles.Count < maxBubble)
            StartCoroutine(CorCreateBubbles());
        else
        {
            StartCoroutine(CorResetBubbles());
        }
    }

    IEnumerator CorCreateBubbles()
    {
        while (true)
        {
            Bubble temp = Instantiate(bubblePrefab, summonGate.transform.position, Quaternion.identity, transform);
            temp.InitBubble(start, end, maxMoveSpeed, maxJumpPower, myGravity);
            Debug.Log(name + " : " + myGravity);
            temp.gameObject.SetActive(true);
            bubbles.Add(temp);

            if (bubbles.Count > maxBubble)
                yield break;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CorResetBubbles()
    {
        bool done = false;
        int idx = 0;
        while (true)
        {
            if (done == false)
            {
                bubbles[idx].gameObject.transform.position = summonGate.transform.position;
                bubbles[idx].gameObject.SetActive(false);
                idx++;
                Debug.Log(idx + " : " + bubbles.Count);

                if (idx >= bubbles.Count)
                {
                    idx = 0;
                    done = true;
                }

                yield return null;
            }
            if (done == true)
            {

                bubbles[idx].gameObject.SetActive(true);
                bubbles[idx].InitBubble(start, end, maxMoveSpeed, maxJumpPower, myGravity);
                idx++;

                if (idx >= bubbles.Count)
                    yield break;

                yield return new WaitForSeconds(0.8f);
            }
        }
    }
}
