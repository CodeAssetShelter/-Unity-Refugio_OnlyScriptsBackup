using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(GameManager)) as GameManager;

                if (_instance == null)
                {
                    //Debug.LogError("No Active GameManager!");
                }
            }

            return _instance;
        }
    }

    // Score Value
    [Header("- Score")]
    public TextMeshProUGUI scoreText;
    private int myScore = 0;

    // Speed Rivision value
    // 맵 한개를 지날때마다 0.002f 증가
    // 추가예정
    private float speedRivision = 1.0f;
    private float speedMaxRivision = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        scoreText.text = "0";
    }

    public void OnGUI()
    {
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        style.fontSize = (int)(Screen.height * 0.06);
        style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        //float fps = 1.0f / Time.deltaTime;
        float fps = Time.realtimeSinceStartup;
        string text = string.Format("{0:0.}", fps);
        GUI.Label(rect, text, style);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ScoreAdd(int value)
    {
        myScore += value;
        scoreText.text = "" + myScore;
    }
}
