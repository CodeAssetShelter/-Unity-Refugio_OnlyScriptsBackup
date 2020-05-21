using UnityEngine;
using UnityEditor;
using PathCreation;
using System.Collections;
using System.Collections.Generic;

public class ItemLocator : MonoBehaviour
{
    public PathCreator pathCreator;
    public GameObject tester;

    public float distance = 1.0f;

    //private void OnEnable()
    //{
    //    Vector3 prevPos = new Vector3(-20, -20, -20);
    //    float dest = 0;
    //    while (true)
    //    {
    //        Vector3 newPos = pathCreator.path.GetPointAtDistance(distance * dest, EndOfPathInstruction.Stop);
    //        //Vector3 newPos = pathCreator.path.GetPoint(i);
    //        if (newPos == prevPos) return;
    //        Instantiate(tester, newPos, Quaternion.identity, this.transform);
    //        prevPos = newPos;
    //        Debug.Log(newPos + " // " + dest + " // " + pathCreator.path.NumPoints);

    //        dest++;
    //    }
    //}

    public void SetItem(List<SpriteRenderer> scoreItems, Sprite sprite)
    {
        //Vector3 prevPos = new Vector3(-20, -20, -20);
        //float dest = 0;
        //int idx = 0;
        //while (true)
        //{
        //    Vector3 newPos = pathCreator.path.GetPointAtDistance(distance * dest, EndOfPathInstruction.Stop);
        //    if (newPos == prevPos ||
        //        newPos == pathCreator.path.GetPointAtTime(1, EndOfPathInstruction.Stop))
        //    {
        //        Debug.Log("return");
        //        return;
        //    }

        //    if (idx >= scoreItems.Count) {idx = 0;}

        //    if (scoreItems[idx].activeSelf == false)
        //    {
        //        scoreItems[idx].transform.position = newPos;
        //        scoreItems[idx].transform.SetParent(this.transform);
        //        prevPos = newPos;
        //        //Debug.Log(newPos + " // " + dest + " // " + pathCreator.path.NumPoints);
        //        scoreItems[idx].SetActive(true);
        //        dest++;
        //    }
        //    idx++;
        //}
        StartCoroutine(CoroutineSetItem(scoreItems, sprite));
    }

    IEnumerator CoroutineSetItem(List<SpriteRenderer> scoreItems, Sprite sprite)
    {
        float deltaTimer = Time.deltaTime;
        Vector3 prevPos = new Vector3(-20, -20, -20);
        Vector3 newPos = prevPos;
        float dest = 0;
        int idx = 0;

        while (true)
        {
            newPos = pathCreator.path.GetPointAtDistance(distance * dest, EndOfPathInstruction.Stop);
            if (newPos == prevPos ||
                newPos == pathCreator.path.GetPointAtTime(1, EndOfPathInstruction.Stop))
            {
                //Debug.Log("return");
                yield break;
            }

            if (idx >= scoreItems.Count) { idx = 0; }

            if (scoreItems[idx].gameObject.activeSelf == false)
            {
                scoreItems[idx].sprite = sprite;
                scoreItems[idx].transform.position = newPos;
                scoreItems[idx].transform.SetParent(this.transform);
                prevPos = newPos;
                //Debug.Log(newPos + " // " + dest + " // " + pathCreator.path.NumPoints);
                scoreItems[idx].gameObject.SetActive(true);
                dest++;
            }
            idx++;
            yield return null;
        }
    }
}