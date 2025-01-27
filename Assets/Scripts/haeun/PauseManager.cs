using System.Collections;
using UnityEngine;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private player_h player_h; // 플레이어 스크립트 참조
    [SerializeField] private GameObject pausePanel; // 일시 정지 UI 패널
    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private TextMeshProUGUI GoText;

    private bool isPaused = false;

    void Start()
    {
        // 게임 시작 시 일시 정지 패널 비활성화
        pausePanel.SetActive(false);
    }

    public void PauseGame()
    {
<<<<<<< HEAD
        if(!ingreGameManager_h.Instance.IsGameStarting()) {
            if (isPaused) return; // 이미 정지 상태면 아무 작업도 하지 않음

            isPaused = true;
            Time.timeScale = 0; // 게임의 모든 동작 멈춤
            pausePanel.SetActive(true); // 일시 정지 UI 표시
            player_h.SetPauseState(true); // 플레이어 입력 활성화
        }
=======
        if (isPaused) return; // 이미 정지 상태면 아무 작업도 하지 않음

        isPaused = true;
        Time.timeScale = 0; // 게임의 모든 동작 멈춤
        pausePanel.SetActive(true); // 일시 정지 UI 표시
        player_h.SetPauseState(true); // 플레이어 입력 활성화

>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
    }

    public void ResumeGame()
    {
<<<<<<< HEAD
        if(!ingreGameManager_h.Instance.IsGameStarting()) {
            if (!isPaused) return; // 이미 실행 중이면 아무 작업도 하지 않음

            pausePanel.SetActive(false); // 일시 정지 UI 숨김
            player_h.SetPauseState(false); // 플레이어 입력 활성화
            
            StartCoroutine(ResumeReadyGoRoutine());
        }        
=======
        if (!isPaused) return; // 이미 실행 중이면 아무 작업도 하지 않음

        pausePanel.SetActive(false); // 일시 정지 UI 숨김
        player_h.SetPauseState(false); // 플레이어 입력 활성화
        StartCoroutine(ResumeReadyGoRoutine());
        
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
    }

    private IEnumerator ResumeReadyGoRoutine()
    {
        // Ready 표시
        ReadyText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f); // 실시간 기준으로 대기
        ReadyText.gameObject.SetActive(false);

        // Go 표시
        GoText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f); // 실시간 기준으로 대기
        GoText.gameObject.SetActive(false);

        // 모든 것이 다시 시작
        Time.timeScale = 1;
        isPaused = false;
    }
}
