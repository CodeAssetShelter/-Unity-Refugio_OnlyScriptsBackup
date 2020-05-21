using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeStage.AntiCheat.ObscuredTypes;

public class TimeBar : MonoBehaviour
{
    public GridLayoutGroup timeBarGrid;
    public RectTransform timeBarProgress;
    public Animator heartAnimator;
    public float heartAnimationSpeed = 3f;

    private float baseTimeBarLength = 50f;
    private Coroutine coRunTimeBar;
    private float lifeTime = 0;
    Vector3 fullHpPos;

    // 디버깅용
    //private ControllerPlayer controllerPlayer;

    System.Action callbackGameOver;

    public void InitTimeBar(float lifeTime, System.Action callbackGameOver)
    {
        ControllerPlayer.onGameOver += OnGameOver;
        this.callbackGameOver = callbackGameOver;

        fullHpPos = timeBarProgress.localPosition;

        timeBarGrid.cellSize = new Vector2(baseTimeBarLength + lifeTime, timeBarGrid.cellSize.y);
        Vector2 newSize = timeBarProgress.sizeDelta;
        newSize.x = timeBarGrid.cellSize.x;
        timeBarProgress.sizeDelta = newSize;

        this.lifeTime = lifeTime;

        coRunTimeBar = StartCoroutine(CorRunTimeBar());
    }

    //// 디버깅용
    //private void OnEnable()
    //{
    //    ControllerPlayer.onGameOver += OnGameOver;
    //    controllerPlayer = transform.root.GetComponent<ControllerPlayer>();
    //    //디버깅용
    //    this.callbackGameOver += controllerPlayer.OnGameOver;

    //    CorGetTimeBar = StartCoroutine(CorRunTimeBar());
    //}

    int count = 0;
    IEnumerator CorRunTimeBar()
    {
        float cellSize = timeBarGrid.cellSize.x;

        if (lifeTime <= 10) lifeTime = 10;

        float timeBarFallBackPerFrame =  cellSize / lifeTime;

        Vector3 pos = timeBarProgress.localPosition;
        heartAnimator.speed = heartAnimationSpeed;
        float substractSpeed = heartAnimationSpeed / lifeTime;

        count = 0;

        // 60 대신에 LifeTime 이 들어갈것
        while (count < lifeTime)
        {
            count++;
            pos = timeBarProgress.localPosition;
            pos.x = fullHpPos.x - (timeBarFallBackPerFrame * count);
            timeBarProgress.localPosition = pos;
            heartAnimator.speed -= substractSpeed;

            yield return new WaitForSeconds(1.0f);
        }

        callbackGameOver();
        heartAnimator.speed = 0;
    }

    public void AddTimerBonus(int count)
    {
        this.count -= count + 1;
        if (count < 0) count = 0;

        CorRunTimeBar().MoveNext();
    }

    private void OnGameOver()
    {
        ControllerPlayer.onGameOver -= OnGameOver;
    }
}