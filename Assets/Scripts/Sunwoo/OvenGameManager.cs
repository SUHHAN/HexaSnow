using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // 오븐 게임 패널 관리
    public GameObject ovenPanel; // 오븐 전체 패널
    public GameObject ovenStartPanel; // 오븐 게임 시작 패널
    public GameObject ovenGamePanel; // 오븐 게임 진행 패널
    public GameObject ovenFinishPanel; // 오븐 게임 종료 패널
    public GameObject toppingPanel; // 토핑 패널

    // UI 요소
    public Button startOvenButton; // 오븐 게임 시작 버튼
    public Button stopButton; // 스톱 버튼 (게이지 멈추기)
    public Button nextButton; // 다음 버튼 (토핑 패널로 이동)
    public Image temperatureBar; // 온도 게이지 바
    public Image gaugeIndicator; // 이동하는 눈금 표시
    public Image targetZone; // 랜덤한 목표 지점 (붉은 영역)
    public TextMeshProUGUI resultText; // 결과 텍스트 (성공/실패)
    public TextMeshProUGUI finalScoreText; // 최종 점수 표시

    // 외부 점수 연동 (재료 선택, 반죽 게임)
    public IngredientSelectManager ingredientSelectManager; // 재료 선택 매니저
    public MixingGameManager mixingGameManager; // 반죽 게임 매니저

    // 오븐 게임 설정
    public float gaugeSpeed = 2f; // 게이지 이동 속도
    public float targetZoneWidth = 100f; // 목표 영역 너비
    public float gaugeWidth = 10f; // 눈금 너비

    private float minGaugePosition; // 눈금 최소 x좌표
    private float maxGaugePosition; // 눈금 최대 x좌표
    private float gaugePosition; // 현재 눈금 위치
    private float targetZoneStart; // 목표 영역 시작 x좌표
    private float targetZoneEnd; // 목표 영역 끝 x좌표

    private bool isGaugeMoving = false; // 눈금 이동 여부
    private bool isGaugeIncreasing = true; // 눈금 이동 방향 (true: 오른쪽, false: 왼쪽)

    private int ovenScore = 0; // 오븐 게임 점수 (성공 시 5점, 실패 시 0점)
    private int totalScore = 0; // 최종 점수

    void Start()
    {
        // 초기 UI 상태 설정
        ovenStartPanel.SetActive(true); // 시작 패널 활성화
        ovenGamePanel.SetActive(false); // 게임 패널 비활성화
        ovenFinishPanel.SetActive(false); // 종료 패널 비활성화
        toppingPanel.SetActive(false); // 토핑 패널 비활성화

        // 버튼 이벤트 등록
        startOvenButton.onClick.AddListener(StartOvenGame); // 오븐 게임 시작
        stopButton.onClick.AddListener(StopGauge); // 눈금 정지
        nextButton.onClick.AddListener(GoToNextPanel); // 토핑 패널로 이동

        // 눈금 이동 범위 설정 (온도 바 크기에 맞게 조정)
        float barWidth = temperatureBar.rectTransform.rect.width;
        minGaugePosition = -barWidth / 2f; // 왼쪽 끝
        maxGaugePosition = barWidth / 2f;  // 오른쪽 끝
    }

    // 오븐 게임 시작
    public void StartOvenGame()
    {
        ovenStartPanel.SetActive(false); // 시작 패널 숨기기
        ovenGamePanel.SetActive(true); // 게임 패널 활성화

        SetTargetZone(); // 목표 영역 설정
        StartGaugeMovement(); // 눈금 이동 시작
    }

    // 랜덤한 목표 영역 설정
    private void SetTargetZone()
    {
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        // 목표 영역 위치 업데이트
        float fixedY = targetZone.rectTransform.anchoredPosition.y;
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, fixedY);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
    }

    // 눈금 이동 시작
    private void StartGaugeMovement()
    {
        isGaugeMoving = true;
        gaugePosition = minGaugePosition; // 눈금 초기 위치 설정
        UpdateGaugePosition(); // 눈금 위치 업데이트
        StartCoroutine(MoveGauge()); // 눈금 이동 시작
    }

    // 눈금 이동 애니메이션
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
        float fixedY = gaugeIndicator.rectTransform.anchoredPosition.y;
        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, fixedY);
    }

    // 스톱 버튼 클릭 시 눈금 멈춤
    public void StopGauge()
    {
        if (!isGaugeMoving) return; // 이미 멈췄으면 실행 안 함

        isGaugeMoving = false; // 눈금 이동 중지
        CheckGaugePosition(); // 목표 영역 내 정지 여부 확인
    }

    // 눈금 위치 확인 (성공 여부 판정)
    private void CheckGaugePosition()
    {
        float gaugeLeftEdge = gaugePosition - (gaugeWidth / 2f);
        float gaugeRightEdge = gaugePosition + (gaugeWidth / 2f);

        // 눈금이 목표 영역과 겹치는지 확인
        if ((gaugeLeftEdge <= targetZoneEnd && gaugeRightEdge >= targetZoneStart) ||
            (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd))
        {
            ovenScore = 5; // 성공 시 5점
            resultText.text = "통과!";
            Debug.Log("오븐 게임 성공: +5점");
        }
        else
        {
            ovenScore = 0; // 실패 시 0점
            resultText.text = "실패!";
            Debug.Log("오븐 게임 실패: +0점");
        }

        CalculateTotalScore(); // 총점 계산
        EndOvenGame(); // 게임 종료
    }

    // 총점 계산
    private void CalculateTotalScore()
    {
        // 재료 게임 점수 + 반죽 게임 점수 + 오븐 게임 점수 합산
        totalScore = ingredientSelectManager.ingredientScore + mixingGameManager.mixingScore + ovenScore;

        Debug.Log($"현재까지 총점: {totalScore}/50");

        // 최종 점수를 UI에 표시
        if (finalScoreText != null)
        {
            finalScoreText.text = $"총 점수: {totalScore}/50";
        }
    }

    // 오븐 게임 종료
    private void EndOvenGame()
    {
        ovenGamePanel.SetActive(false); // 게임 패널 비활성화
        ovenFinishPanel.SetActive(true); // 종료 패널 활성화
    }

    // 토핑 패널로 이동
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false); // 종료 패널 숨기기
        ovenPanel.SetActive(false); // 오븐 패널 비활성화
        toppingPanel.SetActive(true); // 토핑 패널 활성화
    }

    // ToppingManager에서 총점을 가져갈 수 있도록 Getter 추가
    public int GetTotalScore()
    {
        return totalScore;
    }
}
