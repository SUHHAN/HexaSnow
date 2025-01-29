using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // 패널 관리
    public GameObject ovenPanel;
    public GameObject ovenStartPanel;
    public GameObject ovenGamePanel;
    public GameObject ovenFinishPanel;
    public GameObject toppingPanel;

    // UI 요소
    public Button startOvenButton;
    public Button stopButton;
    public Button nextButton;
    public Image temperatureBar; // 가로 막대기
    public Image gaugeIndicator; // 눈금 표시
    public Image targetZone; // 랜덤 붉은색 구역
    public TextMeshProUGUI resultText; // 결과 텍스트

    // 게임 설정
    public float gaugeSpeed = 2f; // 눈금 이동 속도
    public float targetZoneWidth = 100f; // 붉은 구역 너비
    public float gaugeWidth = 10f; // 눈금 너비

    private float minGaugePosition; // 눈금 최소 x좌표
    private float maxGaugePosition; // 눈금 최대 x좌표
    private float gaugePosition; // 눈금 위치
    private float targetZoneStart; // 붉은 구역 시작 위치
    private float targetZoneEnd; // 붉은 구역 끝 위치

    private bool isGaugeMoving = false; // 눈금 이동 여부
    private bool isGaugeIncreasing = true; // 눈금 이동 방향 (true: 오른쪽, false: 왼쪽)

    void Start()
    {
        // 초기 상태 설정
        ovenStartPanel.SetActive(true);
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(false);
        toppingPanel.SetActive(false);

        // 버튼 이벤트 등록
        startOvenButton.onClick.AddListener(StartOvenGame);
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

        SetTargetZone(); // 붉은 구역 설정
        StartGaugeMovement(); // 눈금 이동 시작
    }

    // 랜덤 붉은 구역 설정
    private void SetTargetZone()
    {
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        Debug.Log($"Target Zone 설정: Start={targetZoneStart}, End={targetZoneEnd}");

        // 타겟존의 y축 위치는 Hierarchy에서 설정한 위치 유지
        float fixedY = targetZone.rectTransform.anchoredPosition.y;

        // x축만 랜덤 배치
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, fixedY);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
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
        // Indicator의 y축 위치는 Hierarchy에서 설정한 값 유지
        float fixedY = gaugeIndicator.rectTransform.anchoredPosition.y;

        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, fixedY);
    }

    // Stop 버튼 클릭 (눈금 멈추기)
    public void StopGauge()
    {
        if (!isGaugeMoving) return;

        isGaugeMoving = false; // 눈금 이동 중지

        Debug.Log("[STOP 버튼 클릭] StopGauge 메서드 호출!");
        Debug.Log($"Gauge Position: {gaugePosition}, Left: {gaugePosition - gaugeWidth / 2f}, Right: {gaugePosition + gaugeWidth / 2f}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        CheckGaugePosition(); // 성공 여부 판정
    }

    // 눈금 위치 확인 (성공 or 실패 판정)
    private void CheckGaugePosition()
    {
        // **게이지 인디케이터의 중심 위치를 기준으로 판단**
        float gaugeLeftEdge = gaugePosition - (gaugeWidth / 2f);
        float gaugeRightEdge = gaugePosition + (gaugeWidth / 2f);

        // 현재 위치 디버깅
        Debug.Log($"[STOP 버튼 클릭] Gauge Position: {gaugePosition}");
        Debug.Log($"Gauge Left Edge: {gaugeLeftEdge}, Gauge Right Edge: {gaugeRightEdge}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        // 위치를 다시 계산해 제대로 판정되는지 확인
        if ((gaugeLeftEdge <= targetZoneEnd && gaugeRightEdge >= targetZoneStart) ||
            (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd))
        {
            resultText.text = "통과!";
            Debug.Log("성공");
        }
        else
        {
            resultText.text = "실패!";
            Debug.Log("실패");
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

    // 다음 패널로 이동 (OvenPanel 비활성화)
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false);
        ovenPanel.SetActive(false); // OvenPanel 완전히 비활성화
        toppingPanel.SetActive(true); // ToppingPanel 활성화
    }
}
