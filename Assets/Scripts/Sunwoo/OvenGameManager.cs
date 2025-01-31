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
    public Image temperatureBar;
    public Image gaugeIndicator;
    public Image targetZone;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI finalScoreText;

    // 외부 점수 연동
    public IngredientSelectManager ingredientSelectManager;
    public MixingGameManager mixingGameManager;

    // 게임 설정
    public float gaugeSpeed = 2f;
    public float targetZoneWidth = 100f;
    public float gaugeWidth = 10f;

    private float minGaugePosition;
    private float maxGaugePosition;
    private float gaugePosition;
    private float targetZoneStart;
    private float targetZoneEnd;

    private bool isGaugeMoving = false;
    private bool isGaugeIncreasing = true;

    private int ovenScore = 0;
    private int totalScore = 0;

    void Start()
    {
        ovenStartPanel.SetActive(true);
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(false);
        toppingPanel.SetActive(false);

        startOvenButton.onClick.AddListener(StartOvenGame);
        stopButton.onClick.AddListener(StopGauge);
        nextButton.onClick.AddListener(GoToNextPanel);

        float barWidth = temperatureBar.rectTransform.rect.width;
        minGaugePosition = -barWidth / 2f;
        maxGaugePosition = barWidth / 2f;
    }

    public void StartOvenGame()
    {
        ovenStartPanel.SetActive(false);
        ovenGamePanel.SetActive(true);

        SetTargetZone();
        StartGaugeMovement();
    }

    private void SetTargetZone()
    {
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        float fixedY = targetZone.rectTransform.anchoredPosition.y;
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, fixedY);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
    }

    private void StartGaugeMovement()
    {
        isGaugeMoving = true;
        gaugePosition = minGaugePosition;
        UpdateGaugePosition();
        StartCoroutine(MoveGauge());
    }

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

            UpdateGaugePosition();
            yield return null;
        }
    }

    private void UpdateGaugePosition()
    {
        float fixedY = gaugeIndicator.rectTransform.anchoredPosition.y;
        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, fixedY);
    }

    public void StopGauge()
    {
        if (!isGaugeMoving) return;

        isGaugeMoving = false;
        CheckGaugePosition();
    }

    private void CheckGaugePosition()
    {
        float gaugeLeftEdge = gaugePosition - (gaugeWidth / 2f);
        float gaugeRightEdge = gaugePosition + (gaugeWidth / 2f);

        if ((gaugeLeftEdge <= targetZoneEnd && gaugeRightEdge >= targetZoneStart) ||
            (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd))
        {
            ovenScore = 5;
            resultText.text = "통과!";
            Debug.Log("오븐 게임 성공: +5점");
        }
        else
        {
            ovenScore = 0;
            resultText.text = "실패!";
            Debug.Log("오븐 게임 실패: +0점");
        }

        CalculateTotalScore();
        EndOvenGame();
    }

    private void CalculateTotalScore()
    {
        totalScore = ingredientSelectManager.ingredientScore + mixingGameManager.mixingScore + ovenScore;

        Debug.Log($"현재까지 총점: {totalScore}/50");

        if (finalScoreText != null)
        {
            finalScoreText.text = $"총 점수: {totalScore}/50";
        }
    }

    private void EndOvenGame()
    {
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(true);
    }

    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false);
        ovenPanel.SetActive(false);
        toppingPanel.SetActive(true);
    }
}
