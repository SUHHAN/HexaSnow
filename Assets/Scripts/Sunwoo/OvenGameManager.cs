using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // �г� ����
    public GameObject ovenStartPanel;
    public GameObject ovenGamePanel;
    public GameObject ovenFinishPanel;
    public GameObject toppingPanel;

    // UI ���
    public Button startButton;
    public Button stopButton;
    public Button nextButton;
    public Image temperatureBar; // ���� �����
    public Image gaugeIndicator; // ���� ǥ��
    public Image targetZone; // ���� ������ ����

    // ��� �ؽ�Ʈ
    public TextMeshProUGUI resultText;

    // ���� ����
    public float gaugeSpeed = 2f; // ���� �̵� �ӵ�
    public float targetZoneWidth = 100f; // ���� ���� �ʺ�
    public float gaugeWidth = 10f; // ���� �ʺ�
    private float minGaugePosition; // ���� �ּ� x��ǥ
    private float maxGaugePosition; // ���� �ִ� x��ǥ

    private bool isGaugeMoving = false; // ���� �̵� ����
    private bool isGaugeIncreasing = true; // ���� �̵� ���� (true: ������, false: ����)

    private float gaugePosition; // ���� ��ġ
    private float targetZoneStart; // ���� ���� ���� ��ġ
    private float targetZoneEnd; // ���� ���� �� ��ġ

    void Start()
    {
        // �ʱ� ���� ����
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(false);
        toppingPanel.SetActive(false);

        // ��ư �̺�Ʈ ���
        startButton.onClick.AddListener(StartOvenGame);
        stopButton.onClick.RemoveAllListeners();
        stopButton.onClick.AddListener(StopGauge);
        nextButton.onClick.AddListener(GoToNextPanel);

        // ���� �̵� ���� ���� (�µ��� ũ�⿡ �°�)
        float barWidth = temperatureBar.rectTransform.rect.width;
        minGaugePosition = -barWidth / 2f; // ���� ��
        maxGaugePosition = barWidth / 2f;  // ������ ��
    }

    // ���� ���� ����
    public void StartOvenGame()
    {
        ovenStartPanel.SetActive(false);
        ovenGamePanel.SetActive(true);

        StartGaugeMovement();
        SetTargetZone(); // ���� ���� ����
    }

    // ���� �̵� ����
    private void StartGaugeMovement()
    {
        isGaugeMoving = true;
        gaugePosition = minGaugePosition; // ���� �ʱ� ��ġ
        UpdateGaugePosition(); // ���� ��ġ �ʱ�ȭ
        StartCoroutine(MoveGauge());
    }

    // ���� �̵�
    private IEnumerator MoveGauge()
    {
        while (isGaugeMoving)
        {
            // �̵� ���⿡ ���� ���� ��ġ ����
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
        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, 0f);
    }

    // ���� ���� ���� ����
    private void SetTargetZone()
    {
        // ���� ���� ���� ���
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        Debug.Log($"Target Zone: Start={targetZoneStart}, End={targetZoneEnd}");

        // ���� ���� �̹��� ��ġ �� ũ�� ����
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, 0f);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
    }

    // Stop ��ư Ŭ��
    public void StopGauge()
    {
        if (!isGaugeMoving) return; // ���� �̵� ���� �ƴϸ� ȣ�� ����

        Debug.Log($"[STOP ��ư Ŭ��] StopGauge �޼��� ȣ��!");
        isGaugeMoving = false; // ���� �̵� ����

        // Stop ��ư ������ ���� ���� Gauge ��ġ ���
        Debug.Log($"Gauge Position: {gaugePosition}, Gauge Left Edge: {gaugePosition - gaugeWidth / 2f}, Right Edge: {gaugePosition + gaugeWidth / 2f}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        CheckGaugePosition(); // ���� ��ġ Ȯ��
    }

    // ���� ��ġ Ȯ��
    private void CheckGaugePosition()
    {
        float gaugeLeftEdge = gaugePosition - gaugeWidth / 2f;
        float gaugeRightEdge = gaugePosition + gaugeWidth / 2f;

        if (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd)
        {
            resultText.text = "���! Perfect!";
            Debug.Log("���! Perfect!");
        }
        else
        {
            resultText.text = "����! Fail!";
            Debug.Log("����! Fail!");
        }

        // ���� ���� ó��
        EndOvenGame();
    }

    // ���� ���� ����
    private void EndOvenGame()
    {
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(true);
    }

    // ���� �гη� �̵�
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false); // FinishMixing �г� ��Ȱ��ȭ
        toppingPanel.SetActive(true); // ���� �г� Ȱ��ȭ
    }
}
