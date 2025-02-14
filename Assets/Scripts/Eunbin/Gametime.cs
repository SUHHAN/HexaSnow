using System;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTime : MonoBehaviour
{
    public static GameTime Instance { get; private set; }
    public TextMeshProUGUI timerText;
    private float gameTime = 360f;
    public float currentTime;
    public event Action<float> OnTimeUpdate; // 시간 업데이트 이벤트
    public event Action OnSpecialTimeReached; // 특정 시간 도달 이벤트
    private bool isGameRunning = false;
    public DayChange daychange;
    private Coroutine timerCoroutine; // 코루틴을 저장할 변수
    private bool specialEventTriggered = false;

     [SerializeField] private GameData GD = new GameData();
    void Start()
    {
    }

    public void StartGameTimer()
    {
        if (isGameRunning && timerCoroutine != null)
        {
             StopCoroutine(timerCoroutine); // 기존 코루틴 중단
        }
        isGameRunning = true;
        timerCoroutine = StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        Loadtime();
      
        currentTime=gameTime;
        
        while (currentTime > 0)
        {
            yield return null; // 매 프레임마다 실행
            
            currentTime -= Time.deltaTime; // 경과된 시간만큼 감소
            Savetime();
            OnTimeUpdate?.Invoke(currentTime); // 시간 업데이트 이벤트 트리거

            if (currentTime <= 340 && !specialEventTriggered)
            {
                OnSpecialTimeReached?.Invoke(); // 특정 시간 도달 이벤트 트리거
                specialEventTriggered = true;
            }
            UpdateTimerUI(currentTime);
        }

        isGameRunning = false;
        timerCoroutine = null; // 코루틴 초기화
        OnTimerEnd(); // 타이머 종료 처리
    }

    private void UpdateTimerUI(float currentTime)
    {
        int minutes = 16-(Mathf.CeilToInt(currentTime / 60));
        int seconds = (360 - Mathf.CeilToInt(currentTime)) % 60 / 10 * 10; 

        timerText.text = $"{minutes:D2}:{seconds:D2}"; 
    }
    public void StopTimer()
{
    if (timerCoroutine != null)
    {
        StopCoroutine(timerCoroutine); // 코루틴 중지
        timerCoroutine = null; // 변수 초기화
    }
}

    public void OnTimerEnd()
    {
        Debug.Log("6분이 끝났습니다");
        specialEventTriggered = false;
        SceneManager.LoadScene("Deadline");
        daychange.OnDayChange();
        StopTimer();
        
    }
    private void Loadtime() {

        GD = DataManager.Instance.LoadGameData();

        // !! 일차 업데이트하기
        currentTime = GD.time;
    }

    private void Savetime() {
        DataManager.Instance.gameData.time = currentTime;

        DataManager.Instance.SaveGameData();
    }
}


