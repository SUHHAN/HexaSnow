using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OvenGameManager : MonoBehaviour
{
    // �г� ����
    public GameObject ovenPanel;
    public GameObject ovenStartPanel;
    public GameObject ovenGamePanel;
    public GameObject ovenFinishPanel;
    public GameObject toppingPanel;

    // UI ���
    public Button startOvenButton;
    public Button stopButton;
    public Button nextButton;
    public Image temperatureBar; // ���� �����
    public Image gaugeIndicator; // ���� ǥ��
    public Image targetZone; // ���� ������ ����
    public TextMeshProUGUI resultText; // ��� �ؽ�Ʈ

    // ���� ����
    public float gaugeSpeed = 2f; // ���� �̵� �ӵ�
    public float targetZoneWidth = 100f; // ���� ���� �ʺ�
    public float gaugeWidth = 10f; // ���� �ʺ�

    private float minGaugePosition; // ���� �ּ� x��ǥ
    private float maxGaugePosition; // ���� �ִ� x��ǥ
    private float gaugePosition; // ���� ��ġ
    private float targetZoneStart; // ���� ���� ���� ��ġ
    private float targetZoneEnd; // ���� ���� �� ��ġ

    private bool isGaugeMoving = false; // ���� �̵� ����
    private bool isGaugeIncreasing = true; // ���� �̵� ���� (true: ������, false: ����)

    void Start()
    {
        // �ʱ� ���� ����
        ovenStartPanel.SetActive(true);
        ovenGamePanel.SetActive(false);
        ovenFinishPanel.SetActive(false);
        toppingPanel.SetActive(false);

        // ��ư �̺�Ʈ ���
        startOvenButton.onClick.AddListener(StartOvenGame);
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

        SetTargetZone(); // ���� ���� ����
        StartGaugeMovement(); // ���� �̵� ����
    }

    // ���� ���� ���� ����
    private void SetTargetZone()
    {
        float zoneStartRange = minGaugePosition + targetZoneWidth / 2f;
        float zoneEndRange = maxGaugePosition - targetZoneWidth / 2f;

        targetZoneStart = Random.Range(zoneStartRange, zoneEndRange);
        targetZoneEnd = targetZoneStart + targetZoneWidth;

        Debug.Log($"Target Zone ����: Start={targetZoneStart}, End={targetZoneEnd}");

        // Ÿ������ y�� ��ġ�� Hierarchy���� ������ ��ġ ����
        float fixedY = targetZone.rectTransform.anchoredPosition.y;

        // x�ุ ���� ��ġ
        targetZone.rectTransform.anchoredPosition = new Vector2(targetZoneStart, fixedY);
        targetZone.rectTransform.sizeDelta = new Vector2(targetZoneWidth, targetZone.rectTransform.sizeDelta.y);
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
        // Indicator�� y�� ��ġ�� Hierarchy���� ������ �� ����
        float fixedY = gaugeIndicator.rectTransform.anchoredPosition.y;

        gaugeIndicator.rectTransform.anchoredPosition = new Vector2(gaugePosition, fixedY);
    }

    // Stop ��ư Ŭ�� (���� ���߱�)
    public void StopGauge()
    {
        if (!isGaugeMoving) return;

        isGaugeMoving = false; // ���� �̵� ����

        Debug.Log("[STOP ��ư Ŭ��] StopGauge �޼��� ȣ��!");
        Debug.Log($"Gauge Position: {gaugePosition}, Left: {gaugePosition - gaugeWidth / 2f}, Right: {gaugePosition + gaugeWidth / 2f}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        CheckGaugePosition(); // ���� ���� ����
    }

    // ���� ��ġ Ȯ�� (���� or ���� ����)
    private void CheckGaugePosition()
    {
        // **������ �ε��������� �߽� ��ġ�� �������� �Ǵ�**
        float gaugeLeftEdge = gaugePosition - (gaugeWidth / 2f);
        float gaugeRightEdge = gaugePosition + (gaugeWidth / 2f);

        // ���� ��ġ �����
        Debug.Log($"[STOP ��ư Ŭ��] Gauge Position: {gaugePosition}");
        Debug.Log($"Gauge Left Edge: {gaugeLeftEdge}, Gauge Right Edge: {gaugeRightEdge}");
        Debug.Log($"Target Zone Start: {targetZoneStart}, End: {targetZoneEnd}");

        // ��ġ�� �ٽ� ����� ����� �����Ǵ��� Ȯ��
        if ((gaugeLeftEdge <= targetZoneEnd && gaugeRightEdge >= targetZoneStart) ||
            (gaugeLeftEdge >= targetZoneStart && gaugeRightEdge <= targetZoneEnd))
        {
            resultText.text = "���!";
            Debug.Log("����");
        }
        else
        {
            resultText.text = "����!";
            Debug.Log("����");
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

    // ���� �гη� �̵� (OvenPanel ��Ȱ��ȭ)
    public void GoToNextPanel()
    {
        ovenFinishPanel.SetActive(false);
        ovenPanel.SetActive(false); // OvenPanel ������ ��Ȱ��ȭ
        toppingPanel.SetActive(true); // ToppingPanel Ȱ��ȭ
    }
}
