using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // 패널 관리
    public GameObject ovenStartPanel;
    public GameObject ovenGamePanel;
    public GameObject ovenFinishPanel;
    public GameObject toppingPanel;

    // UI 요소
    public Button startButton;
    public Button stopButton;
    public Button nextButton;
    public Image temperatureBar; // 가로 막대기
    public Image gaugeIndicator; // 눈금 표시
    public Image targetZone; // 랜덤 붉은색 구역

    // 결과 텍스트
    public TextMeshProUGUI resultText;

    // 게임 설정
    public float gaugeSpeed = 2f; // 눈금 이동 속도
    public float targetZoneWidth = 100f; // 붉은 구역 너비
    public float gaugeWidth = 10f; // 눈금 너비
    private float minGaugePosition; // 눈금 최소 x좌표
    private float maxGaugePosition; // 눈금 최대 x좌표

    private bool isGaugeMoving = false; // 눈금 이동 여부
    private bool isGaugeIncreasing = true; // 눈금 이동 방향 (true: 오른쪽, false: 왼쪽)

    private float gaugePosition; // 눈금 위치
    private float targetZoneStart; // 붉은 구역 시작 위치
    private float targetZoneEnd; // 붉은 구역 끝 위치

    void Start()
    {
        // 초기 상태 설정
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(false);
        toppingPanel.SetActive(false);

        // 버튼 이벤트 등록
        startButton.onClick.AddListener(StartOvenGame);
        stopButton.onClick.RemoveAllListeners();
        stopButton.onClick.AddListener(StopGauge);
        nextButton.onClick.AddListener(GoToNextPanel);

        // 눈금 이동 범위 설정 (온도계 크기에 맞게)
        float barWidth = temperatureBar.rectTransform.rect.width;
        minGaugePosition = -barWidth / 2f; // 왼쪽 끝
        maxGaugePosition = barWidth / 2f;  // 오른쪽 끝
    }

    // 오븐 게임 시작
    public void StartOvenGame()
    {
        ovenStartPanel.SetActive(false);
        ovenGamePanel.SetActive(true);

        StartGaugeMovement();
        SetTargetZone(); // 붉은 구역 설정
    }

    // 눈금 이동 시작
    private void StartGaugeMovement()
    {
        isGaugeMoving = true;
        gaugePosition = minGaugePosition; // 눈금 초기 위치
        UpdateGaugePosition(); // 눈금 위치 초기화
        StartCoroutine(MoveGauge());
    }

    // 눈금 이동
    private IEnumerator MoveGauge()
    {
        while (isGaugeMoving)
        {
            // 이동 방향에 따라 눈금 위치 변경
            if (isGaugeIncreasing)
            {
                gaugePosition += gaugeSpeed * Time.deltaTime;
                if (gaugePosition >= maxGaugePosition)
                {
                    gaugePosition = maxGaugePosition;
                    isGaugeIncreasing = false;
                }
            }
            else
            {
                gaugePosition -= gaugeSpeed * Time.deltaTime;
                if (gaugePosition <= minGaugePosition)
                {
                    gaugePosition = minGaugePosition;
                    isGaugeIncreasing = true;
                }
            }

            UpdateGaugePosition(); // 눈금 위치 업데이트
            yield return null;
        }
    }

    // 눈금 위치 업데이트
    private void UpdateGaugePosition()
    {
        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, 0f);
    }

    // 랜덤 붉은 구역 설정
    private void SetTargetZone()
    {
        // 붉은 구역 범위 계산
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        Debug.Log($"Target Zone: Start={targetZoneStart}, End={targetZoneEnd}");

        // 붉은 구역 이미지 위치 및 크기 설정
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, 0f);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
    }

    // Stop 버튼 클릭
    public void StopGauge()
    {
        if (!isGaugeMoving) return; // 눈금 이동 중이 아니면 호출 차단

        Debug.Log($"[STOP 버튼 클릭] StopGauge 메서드 호출!");
        isGaugeMoving = false; // 눈금 이동 중지

        // Stop 버튼 눌렀을 때의 현재 Gauge 위치 출력
        Debug.Log($"Gauge Position: {gaugePosition}, Gauge Left Edge: {gaugePosition - gaugeWidth / 2f}, Right Edge: {gaugePosition + gaugeWidth / 2f}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        CheckGaugePosition(); // 눈금 위치 확인
    }

    // 눈금 위치 확인
    private void CheckGaugePosition()
    {
        float gaugeLeftEdge = gaugePosition - gaugeWidth / 2f;
        float gaugeRightEdge = gaugePosition + gaugeWidth / 2f;

        if (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd)
        {
            resultText.text = "통과! Perfect!";
            Debug.Log("통과! Perfect!");
        }
        else
        {
            resultText.text = "실패! Fail!";
            Debug.Log("실패! Fail!");
        }

        // 게임 종료 처리
        EndOvenGame();
    }

    // 오븐 게임 종료
    private void EndOvenGame()
    {
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(true);
    }

    // 다음 패널로 이동
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false); // FinishMixing 패널 비활성화
        toppingPanel.SetActive(true); // 토핑 패널 활성화
    }
}
