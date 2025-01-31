using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager2 : MonoBehaviour
{
    // UI �г� �� ���
    public GameObject mixingPanel;
    public GameObject startMixingPanel;
    public GameObject mixingGamePanel;
    public GameObject finishMixingPanel;
    public GameObject ovenPanel;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI startText;
    public Button startButton;
    public Button nextButton;
    public TextMeshProUGUI scoreText;

    // Bowl �̹���
    public GameObject bowlBefore; // BowlBefore �̹���
    public GameObject bowlAfter; // BowlAfter �̹���
    private bool hasAnimationStarted = false; // �ִϸ��̼� ���� ���� Ȯ��

    // �����¿� ��ư
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;

    // �����ܰ� ���׶�� ��ġ
    public Transform iconSpawnPoint; // �������� �����Ǵ� ��ġ (������)
    public Transform targetCircle;   // ���׶�� ��ġ (����, ��ư�� ������ ��)

    public GameObject upIconPrefab;
    public GameObject downIconPrefab;
    public GameObject leftIconPrefab;
    public GameObject rightIconPrefab;

    private List<GameObject> activeIcons = new List<GameObject>();
    private float moveDuration = 1f; // �������� 1�� ���� �����ϵ��� ����
    private float gameDuration = 30f; // ���� ���� �ð�
    private float remainingTime;
    private bool isGameRunning = false;
    private int score = 0;

    // Mixing �ִϸ��̼� ����
    public Image doughImage; // UI Image (�ִϸ��̼��� ǥ���� ���)
    public Image doughImage2; // UI Image
    private Sprite[] doughSprites; // ��������Ʈ �迭
    private int currentFrame = 0;
    private float frameDuration = 30f / 85f; // 30�� ���� 17�� �ִϸ��̼�
    private bool isAnimating = false;

    private void Start()
    {
        // �ʱ� UI ����
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false);

        bowlBefore.SetActive(true);
        bowlAfter.SetActive(false);
        startButton.gameObject.SetActive(false); // Start ��ư �ʱ� ��Ȱ��ȭ
        startButton.gameObject.SetActive(false);

        // ��ư �̺�Ʈ ���
        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel);

        upButton.onClick.AddListener(() => CheckInput("Up"));
        downButton.onClick.AddListener(() => CheckInput("Down"));
        leftButton.onClick.AddListener(() => CheckInput("Left"));
        rightButton.onClick.AddListener(() => CheckInput("Right"));

        // �ִϸ��̼� ��������Ʈ �ε�
        LoadDoughSprites();
    }

    // Mixing ���� ����
    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false);
        mixingGamePanel.SetActive(true);
        StartCoroutine(StartGameSequence());
    }

    // `Resources` �������� ��������Ʈ �ε�
    private void LoadDoughSprites()
    {
        doughSprites = new Sprite[17];
        for (int i = 0; i < 17; i++)
        {
            string path = $"Sunwoo/Mixing/ani_paste_{i + 1}";
            doughSprites[i] = Resources.Load<Sprite>(path);

            if (doughSprites[i] == null)
            {
                Debug.LogError($"�ִϸ��̼� �̹��� �ε� ����: {path}");
            }
        }
    }

    public void ActivateMixingPanel()
    {
        mixingPanel.SetActive(true); // Mixing ��ü �г� Ȱ��ȭ
        startMixingPanel.SetActive(true); // StartMixingPanel Ȱ��ȭ

        if (!hasAnimationStarted)
        {
            StartCoroutine(BowlAnimation());
            hasAnimationStarted = true; // �ִϸ��̼� �� ���� ����ǵ��� ����
        }
    }

    private IEnumerator BowlAnimation()
    {
        Debug.Log("BowlAnimation ����");

        float rotationDuration = 1f; // Bowl ȸ�� ���� �ð� 
        float elapsedTime = 0f;
        float rotationSpeed = 1080f; // ȸ�� �ӵ� (�⺻ 360������ ����)

        // BowlBefore ������ ȸ��
        while (elapsedTime < rotationDuration)
        {
            bowlBefore.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); // 1080�� ȸ�� (�ʴ� 3����)
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("BowlAnimation ����");

        // BowlBefore ��Ȱ��ȭ �� BowlAfter Ȱ��ȭ
        bowlBefore.SetActive(false);
        bowlAfter.SetActive(true);

        // Start ��ư Ȱ��ȭ
        startButton.gameObject.SetActive(true);
    }

    private IEnumerator StartGameSequence()
    {
        // Ready? ǥ��
        readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.gameObject.SetActive(false);

        // Start! ǥ��
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        // ���� ����
        isGameRunning = true;
        remainingTime = gameDuration;

        StartCoroutine(GameTimer());
        StartCoroutine(PlayDoughAnimation());
        StartCoroutine(SpawnIcons());
    }

    // �ִϸ��̼� ���� (�̹��� ����)
    private IEnumerator PlayDoughAnimation()
    {
        isAnimating = true;
        currentFrame = 0;

        while (isGameRunning)
        {
            doughImage.sprite = doughSprites[currentFrame]; // ���� ������ ����
            currentFrame = (currentFrame + 1) % doughSprites.Length; // ���� ���������� ����

            yield return new WaitForSeconds(frameDuration); // ������ ���� �ð� ���
        }
    }

    private IEnumerator SpawnIcons()
    {
        while (isGameRunning)
        {
            int iconCount = Random.Range(1, 5); // �� ���� �ִ� 5���� ������ ����
            float startX = iconSpawnPoint.position.x;
            float startY = iconSpawnPoint.position.y;
            float spacing = Random.Range(1f, 3f); // ������ �� �Ÿ� 1~3ĭ ���� ����

            List<float> usedPositions = new List<float>(); // �̹� ���� ��ġ ����

            for (int i = 0; i < iconCount; i++)
            {
                // ���� ��ġ ���� (���� ��ġ�� �ʵ���)
                float offset = Random.Range(-1.5f, 1.5f) * spacing;
                while (usedPositions.Contains(offset))
                {
                    offset = Random.Range(-1.5f, 1.5f) * spacing;
                }
                usedPositions.Add(offset);

                Vector3 spawnPosition = new Vector3(startX, startY + offset, 0);

                GameObject newIcon = Instantiate(GetRandomIcon(), spawnPosition, Quaternion.identity);
                activeIcons.Add(newIcon);

                // �������� 1�� ���� ��ǥ �������� �̵�
                StartCoroutine(MoveIcon(newIcon, spawnPosition, targetCircle.position));
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f)); // ���� ������ �������� ���
        }
    }

    private GameObject GetRandomIcon()
    {
        int rand = Random.Range(0, 4);
        switch (rand)
        {
            case 0: return Instantiate(upIconPrefab);
            case 1: return Instantiate(downIconPrefab);
            case 2: return Instantiate(leftIconPrefab);
            case 3: return Instantiate(rightIconPrefab);
        }
        return Instantiate(upIconPrefab); // �⺻�� (���� ����� �� ����)
    }

    private IEnumerator MoveIcon(GameObject icon, Vector3 start, Vector3 target)
    {
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            icon.transform.position = Vector3.Lerp(start, target, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        icon.transform.position = target;
    }

    private void CheckInput(string direction)
    {
        if (activeIcons.Count == 0) return;

        GameObject closestIcon = null;
        float minDistance = float.MaxValue;

        // ���� ����� ������ ã��
        foreach (GameObject icon in activeIcons)
        {
            float distance = Mathf.Abs(icon.transform.position.x - targetCircle.position.x);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestIcon = icon;
            }
        }

        if (closestIcon == null) return;

        string iconType = closestIcon.tag; // �������� "Up", "Down", "Left", "Right" �±׸� ����

        if (iconType == direction)
        {
            if (minDistance <= 0.3f)
                score += 5; // Perfect
            else if (minDistance <= 0.6f)
                score += 3; // Great
            else if (minDistance <= 1.0f)
                score += 1; // Good
        }
        else
        {
            score -= 3; // Ʋ�� ��ư �Է�
        }

        Destroy(closestIcon);
        activeIcons.Remove(closestIcon);
        UpdateScoreText();
    }

    private IEnumerator GameTimer()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        EndMixingGame();
    }

    private void EndMixingGame()
    {
        isGameRunning = false;
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(true);

        // ���� ���� ���
        scoreText.text = $"Score: {score}";
    }

    private void UpdateScoreText()
    {
        scoreText.text = $"Score: {score}";
    }

    private void GoToOvenPanel()
    {
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(true);
    }
}
