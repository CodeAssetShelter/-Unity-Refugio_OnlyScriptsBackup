using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBlackFog : MonoBehaviour
{
    public bool isLeftToRight = true;

    private RectTransform rectTransform;
    private float start, goal;
    private void OnEnable()
    {
        rectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        if (isLeftToRight == true)
        {
            // Use offsetMax
            start = rectTransform.offsetMax.x;
            goal = start * 15f;

            StartCoroutine(CorFadeIn());
        }
        else
        {
            start = rectTransform.offsetMin.x;
            goal = 0;

            StartCoroutine(CorFadeOut());
        }
        
    }

    IEnumerator CorFadeIn()
    {
        Vector2 newOffset = rectTransform.offsetMax;
        float timer = 0;
        while (timer < 1.0f)
        {
            newOffset.x = Mathf.Lerp(start, goal, timer);
            rectTransform.offsetMax = newOffset;

            timer += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator CorFadeOut()
    {
        Vector2 newOffset = rectTransform.offsetMin;
        float timer = 0;
        while (timer < 1.0f)
        {
            newOffset.x = Mathf.Lerp(start, goal, timer);
            rectTransform.offsetMin = newOffset;

            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.SetActive(false);
    }


}
