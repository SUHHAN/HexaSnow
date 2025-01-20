using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class GameTime : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private float gameTime = 360f;
    public float currentTime;
    public event Action<float> OnTimeUpdate; // 시간 업데이트 이벤트
    public event Action OnSpecialTimeReached; // 특정 시간 도달 이벤트
    private bool isGameRunning = false;
    public DayChange daychange;

    void Start()
    {
        StartGameTimer(); // 게임 타이머 시작
    }

    public void StartGameTimer()
    {
        isGameRunning = true;
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        currentTime=gameTime;
        while (currentTime > 0)
        {
            yield return null; // 매 프레임마다 실행
            currentTime -= Time.deltaTime; // 경과된 시간만큼 감소
            OnTimeUpdate?.Invoke(currentTime); // 시간 업데이트 이벤트 트리거

            if (Mathf.Abs(currentTime - 350f) < 0.1f) // 5초 근처 확인
            {
                OnSpecialTimeReached?.Invoke(); // 특정 시간 도달 이벤트 트리거
            }
            UpdateTimerUI(currentTime);
        }

        isGameRunning = false;
        OnTimerEnd(); // 타이머 종료 처리
    }

    private void UpdateTimerUI(float currentTime)
    {
        int minutes = (6-Mathf.FloorToInt(currentTime / 60f))+9;
        int seconds = (60-Mathf.FloorToInt(currentTime% 60f));

        timerText.text = $"{minutes:D2}:{seconds:D2}"; 
    }

    private void OnTimerEnd()
    {
        Debug.Log("6분이 끝났습니다");
        daychange.OnDayChange();
        StartGameTimer();
    }
}

