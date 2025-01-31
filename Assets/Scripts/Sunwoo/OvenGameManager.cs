using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // ���� ���� �г� ����
    public GameObject ovenPanel; // ���� ��ü �г�
    public GameObject ovenStartPanel; // ���� ���� ���� �г�
    public GameObject ovenGamePanel; // ���� ���� ���� �г�
    public GameObject ovenFinishPanel; // ���� ���� ���� �г�
    public GameObject toppingPanel; // ���� �г�

    // UI ���
    public Button startOvenButton; // ���� ���� ���� ��ư
    public Button stopButton; // ���� ��ư (������ ���߱�)
    public Button nextButton; // ���� ��ư (���� �гη� �̵�)
    public Image temperatureBar; // �µ� ������ ��
    public Image gaugeIndicator; // �̵��ϴ� ���� ǥ��
    public Image targetZone; // ������ ��ǥ ���� (���� ����)
    public TextMeshProUGUI resultText; // ��� �ؽ�Ʈ (����/����)
    public TextMeshProUGUI finalScoreText; // ���� ���� ǥ��

    // �ܺ� ���� ���� (��� ����, ���� ����)
    public IngredientSelectManager ingredientSelectManager; // ��� ���� �Ŵ���
    public MixingGameManager mixingGameManager; // ���� ���� �Ŵ���

    // ���� ���� ����
    public float gaugeSpeed = 2f; // ������ �̵� �ӵ�
    public float targetZoneWidth = 100f; // ��ǥ ���� �ʺ�
    public float gaugeWidth = 10f; // ���� �ʺ�

    private float minGaugePosition; // ���� �ּ� x��ǥ
    private float maxGaugePosition; // ���� �ִ� x��ǥ
    private float gaugePosition; // ���� ���� ��ġ
    private float targetZoneStart; // ��ǥ ���� ���� x��ǥ
    private float targetZoneEnd; // ��ǥ ���� �� x��ǥ

    private bool isGaugeMoving = false; // ���� �̵� ����
    private bool isGaugeIncreasing = true; // ���� �̵� ���� (true: ������, false: ����)

    private int ovenScore = 0; // ���� ���� ���� (���� �� 5��, ���� �� 0��)
    private int totalScore = 0; // ���� ����

    void Start()
    {
        // �ʱ� UI ���� ����
        ovenStartPanel.SetActive(true); // ���� �г� Ȱ��ȭ
        ovenGamePanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        ovenFinishPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        toppingPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ

        // ��ư �̺�Ʈ ���
        startOvenButton.onClick.AddListener(StartOvenGame); // ���� ���� ����
        stopButton.onClick.AddListener(StopGauge); // ���� ����
        nextButton.onClick.AddListener(GoToNextPanel); // ���� �гη� �̵�

        // ���� �̵� ���� ���� (�µ� �� ũ�⿡ �°� ����)
        float barWidth = temperatureBar.rectTransform.rect.width;
        minGaugePosition = -barWidth / 2f; // ���� ��
        maxGaugePosition = barWidth / 2f;  // ������ ��
    }

    // ���� ���� ����
    public void StartOvenGame()
    {
        ovenStartPanel.SetActive(false); // ���� �г� �����
        ovenGamePanel.SetActive(true); // ���� �г� Ȱ��ȭ

        SetTargetZone(); // ��ǥ ���� ����
        StartGaugeMovement(); // ���� �̵� ����
    }

    // ������ ��ǥ ���� ����
    private void SetTargetZone()
    {
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        // ��ǥ ���� ��ġ ������Ʈ
        float fixedY = targetZone.rectTransform.anchoredPosition.y;
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, fixedY);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
    }

    // ���� �̵� ����
    private void StartGaugeMovement()
    {
        isGaugeMoving = true;
        gaugePosition = minGaugePosition; // ���� �ʱ� ��ġ ����
        UpdateGaugePosition(); // ���� ��ġ ������Ʈ
        StartCoroutine(MoveGauge()); // ���� �̵� ����
    }

    // ���� �̵� �ִϸ��̼�
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

            UpdateGaugePosition(); // ���� ��ġ ������Ʈ
            yield return null;
        }
    }

    // ���� ��ġ ������Ʈ
    private void UpdateGaugePosition()
    {
        float fixedY = gaugeIndicator.rectTransform.anchoredPosition.y;
        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, fixedY);
    }

    // ���� ��ư Ŭ�� �� ���� ����
    public void StopGauge()
    {
        if (!isGaugeMoving) return; // �̹� �������� ���� �� ��

        isGaugeMoving = false; // ���� �̵� ����
        CheckGaugePosition(); // ��ǥ ���� �� ���� ���� Ȯ��
    }

    // ���� ��ġ Ȯ�� (���� ���� ����)
    private void CheckGaugePosition()
    {
        float gaugeLeftEdge = gaugePosition - (gaugeWidth / 2f);
        float gaugeRightEdge = gaugePosition + (gaugeWidth / 2f);

        // ������ ��ǥ ������ ��ġ���� Ȯ��
        if ((gaugeLeftEdge <= targetZoneEnd && gaugeRightEdge >= targetZoneStart) ||
            (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd))
        {
            ovenScore = 5; // ���� �� 5��
            resultText.text = "���!";
            Debug.Log("���� ���� ����: +5��");
        }
        else
        {
            ovenScore = 0; // ���� �� 0��
            resultText.text = "����!";
            Debug.Log("���� ���� ����: +0��");
        }

        CalculateTotalScore(); // ���� ���
        EndOvenGame(); // ���� ����
    }

    // ���� ���
    private void CalculateTotalScore()
    {
        // ��� ���� ���� + ���� ���� ���� + ���� ���� ���� �ջ�
        totalScore = ingredientSelectManager.ingredientScore + mixingGameManager.mixingScore + ovenScore;

        Debug.Log($"������� ����: {totalScore}/50");

        // ���� ������ UI�� ǥ��
        if (finalScoreText != null)
        {
            finalScoreText.text = $"�� ����: {totalScore}/50";
        }
    }

    // ���� ���� ����
    private void EndOvenGame()
    {
        ovenGamePanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        ovenFinishPanel.SetActive(true); // ���� �г� Ȱ��ȭ
    }

    // ���� �гη� �̵�
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false); // ���� �г� �����
        ovenPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        toppingPanel.SetActive(true); // ���� �г� Ȱ��ȭ
    }

    // ToppingManager���� ������ ������ �� �ֵ��� Getter �߰�
    public int GetTotalScore()
    {
        return totalScore;
    }
}
