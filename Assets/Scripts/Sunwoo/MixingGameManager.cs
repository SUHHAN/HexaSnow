using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager : MonoBehaviour
{
    // �г� �� UI ���
    public GameObject mixingPanel; // Mixing ��ü �г�
    public GameObject startMixingPanel;
    public GameObject mixingGamePanel;
    public GameObject finishMixingPanel;
    public GameObject ovenPanel;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI startText;
    public Button startButton;
    public Button nextButton;
    public TextMeshProUGUI scoreText;

    // ��ġ ����Ʈ�� �� ������
    public GameObject handIcon;
    private Vector3 initialHandPosition = new Vector3(0, -100, 0);
    public Transform[] touchPointPositions; // ��ġ ����Ʈ ��ġ �迭
    public GameObject[] touchPointImages; // ��ġ ����Ʈ �̹��� �迭

    // Bowl �̹���
    public GameObject bowlBefore; // BowlBefore �̹���
    public GameObject bowlAfter; // BowlAfter �̹���
    private bool hasAnimationStarted = false; // �ִϸ��̼� ���� ���� Ȯ��

    // ���� ����
    public float pointLifetime = 1f; // ��ġ ����Ʈ ���� �ð�
    public float gameDuration = 30f; // ���� ���� �ð�
    private float remainingTime;

    private bool isGameRunning = false; // ���� ���� ����
    private int score = 0; // ���� ����

    private GameObject activeTouchPoint = null; // ���� Ȱ��ȭ�� ��ġ ����Ʈ
    private int previousIndex = -1; // ������ ������ ��ġ ����Ʈ �ε��� (-1�� �ʱⰪ)

    private void Start()
    {
        // �ʱ� UI ���� ����
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false); // Oven �г� �ʱ� ��Ȱ��ȭ

        bowlBefore.SetActive(true);
        bowlAfter.SetActive(false);
        startButton.gameObject.SetActive(false); // Start ��ư �ʱ� ��Ȱ��ȭ

        // Start ��ư Ŭ�� �̺�Ʈ ���
        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel); // Next ��ư Ŭ�� �̺�Ʈ ���
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

    // Mixing ���� ����
    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false); // StartMixing �г� ��Ȱ��ȭ
        StartCoroutine(StartGameSequence());
    }

    // Ready?�� Start! �ؽ�Ʈ ǥ�� �� Mixing ���� ����
    private IEnumerator StartGameSequence()
    {
        mixingGamePanel.SetActive(true); // MixingGame �г� Ȱ��ȭ

        // Ready? ǥ��
        readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.gameObject.SetActive(false);

        // Start! ǥ��
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        // ��ġ ����Ʈ ��� ��Ȱ��ȭ
        foreach (GameObject touchPoint in touchPointImages)
        {
            touchPoint.SetActive(false);
        }

        // ���� ����
        isGameRunning = true;
        remainingTime = gameDuration;
        StartCoroutine(GameTimer());
        StartCoroutine(SpawnTouchPoints());
    }

    // ��ġ ����Ʈ ���� (�� ���� �ϳ��� Ȱ��ȭ, ���� ��ġ ����)
    private IEnumerator SpawnTouchPoints()
    {
        while (isGameRunning)
        {
            int randomIndex;

            // ���� �ε����� �ٸ� ��ġ ����
            do
            {
                randomIndex = Random.Range(0, touchPointPositions.Length);
            } while (randomIndex == previousIndex);

            previousIndex = randomIndex; // ���� �ε����� ���� �ε����� ����

            if (activeTouchPoint != null)
            {
                activeTouchPoint.SetActive(false); // ���� Ȱ��ȭ�� ����Ʈ ��Ȱ��ȭ
            }

            activeTouchPoint = touchPointImages[randomIndex];
            activeTouchPoint.SetActive(true); // �� ����Ʈ Ȱ��ȭ

            yield return new WaitForSeconds(pointLifetime); // ���� �ð� ���
            activeTouchPoint.SetActive(false); // ����Ʈ ��Ȱ��ȭ
        }
    }

    // ���� Ÿ�̸�
    private IEnumerator GameTimer()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        EndMixingGame(); // ���� ����
    }

    // Mixing ���� ����
    public void EndMixingGame()
    {
        isGameRunning = false;
        mixingGamePanel.SetActive(false); // MixingGame �г� ��Ȱ��ȭ
        finishMixingPanel.SetActive(true); // FinishMixing �г� Ȱ��ȭ

        // ������ ���� ��� ǥ��
        if (score >= 65 && score <= 75)
        {
            scoreText.text = "Perfect!";
            Debug.Log("Perfect!");
        }
        else if (score >= 40 && score < 65)
        {
            scoreText.text = "Great!";
            Debug.Log("Great!");
        }
        else if (score >= 10 && score < 40)
        {
            scoreText.text = "Good!";
            Debug.Log("Good!");
        }
        else
        {
            scoreText.text = "Bad!";
            Debug.Log("Bad!");
        }
    }

    // �� ������ ���콺 �巡�׷� �̵�
    private void Update()
    {
        if (isGameRunning && Input.GetMouseButton(0)) // ���콺 ���� ��ư Ŭ��
        {
            Vector3 mousePosition = Input.mousePosition; // ���콺 ��ġ ��������
            mousePosition.z = 10f; // ī�޶���� �Ÿ� ����
            handIcon.transform.position = Camera.main.ScreenToWorldPoint(mousePosition); // �� ������ ��ġ ������Ʈ
        }

        if (Input.GetMouseButtonUp(0)) // ���콺 ���� ��ư�� ���� ��
        {
            EvaluateTouchPoint();
            handIcon.transform.localPosition = initialHandPosition; // ���� ��ġ�� ����
        }
    }

    // �� �����ܰ� ��ġ ����Ʈ�� �Ÿ� ��� �� ���� ����
    private void EvaluateTouchPoint()
    {
        if (!isGameRunning || activeTouchPoint == null) return;

        float distance = Vector2.Distance(handIcon.transform.position, activeTouchPoint.transform.position);
        Debug.Log($"Distance: {distance}");

        if (distance <= 0.5)
        {
            score += 5; // Perfect
            Debug.Log("Perfect! +5");
        }
        else if (distance <= 1)
        {
            score += 3; // Great
            Debug.Log("Great! +3");
        }
        else if (distance <= 1.5)
        {
            score += 1; // Good
            Debug.Log("Good! +1");
        }
        else if (distance <= 2)
        {
            Debug.Log("Bad! +0");
        }
        else
        {
            score -= 5; // Miss
            Debug.Log("Miss! -5");
        }
    }

    // Oven �гη� �̵�
    private void GoToOvenPanel()
    {
        finishMixingPanel.SetActive(false); // FinishMixing �г� ��Ȱ��ȭ
        ovenPanel.SetActive(true); // Oven �г� Ȱ��ȭ
    }
}