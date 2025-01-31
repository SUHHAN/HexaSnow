using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager : MonoBehaviour
{
    // 패널 및 UI 요소
    public GameObject mixingPanel;
    public GameObject startMixingPanel;
    public GameObject mixingGamePanel;
    public GameObject finishMixingPanel;
    public GameObject ovenPanel;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI startText;
    public Button startButton;
    public Button nextButton;
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI finalText; // 최종 점수 표시

    // 터치 포인트와 손 아이콘
    public GameObject handIcon;
    private Vector3 initialHandPosition = new Vector3(0, -100, 0);
    public Transform[] touchPointPositions;
    public GameObject[] touchPointImages;

    // Bowl 이미지
    public GameObject bowlBefore;
    public GameObject bowlAfter;
    private bool hasAnimationStarted = false;

    // 게임 설정
    public float pointLifetime = 1f;
    public float gameDuration = 30f;
    private float remainingTime;

    private bool isGameRunning = false;
    private int score = 0;
    private GameObject activeTouchPoint = null;
    private int previousIndex = -1;

    // Mixing 애니메이션 관련
    public Image doughImage;
    public Image doughImage2;
    private Sprite[] doughSprites;
    private int currentFrame = 0;
    private float frameDuration = 30f / 85f;
    private bool isAnimating = false;

    public int finalScore = 0;

    private void Start()
    {
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false);

        bowlBefore.SetActive(true);
        bowlAfter.SetActive(false);
        startButton.gameObject.SetActive(false);

        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel);

        LoadDoughSprites();
    }

    private void LoadDoughSprites()
    {
        doughSprites = new Sprite[17];
        for (int i = 0; i < 17; i++)
        {
            string path = $"Sunwoo/Mixing/ani_paste_{i + 1}";
            doughSprites[i] = Resources.Load<Sprite>(path);

            if (doughSprites[i] == null)
            {
                Debug.LogError($"애니메이션 이미지 로드 실패: {path}");
            }
        }
    }

    public void ActivateMixingPanel()
    {
        mixingPanel.SetActive(true);
        startMixingPanel.SetActive(true);

        if (!hasAnimationStarted)
        {
            StartCoroutine(BowlAnimation());
            hasAnimationStarted = true;
        }
    }

    private IEnumerator BowlAnimation()
    {
        float rotationDuration = 1f;
        float elapsedTime = 0f;
        float rotationSpeed = 1080f;

        while (elapsedTime < rotationDuration)
        {
            bowlBefore.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bowlBefore.SetActive(false);
        bowlAfter.SetActive(true);
        startButton.gameObject.SetActive(true);
    }

    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false);
        mixingGamePanel.SetActive(true);
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator StartGameSequence()
    {
        readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.gameObject.SetActive(false);

        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        doughImage2.gameObject.SetActive(false);
        doughImage.gameObject.SetActive(true);

        foreach (GameObject touchPoint in touchPointImages)
        {
            touchPoint.SetActive(false);
        }

        isGameRunning = true;
        remainingTime = gameDuration;

        StartCoroutine(GameTimer());
        StartCoroutine(PlayDoughAnimation());
        StartCoroutine(SpawnTouchPoints());
    }

    private IEnumerator PlayDoughAnimation()
    {
        isAnimating = true;
        currentFrame = 0;

        while (isGameRunning)
        {
            doughImage.sprite = doughSprites[currentFrame];
            currentFrame = (currentFrame + 1) % doughSprites.Length;

            yield return new WaitForSeconds(frameDuration);
        }
    }

    private IEnumerator SpawnTouchPoints()
    {
        while (isGameRunning)
        {
            int randomIndex;

            do
            {
                randomIndex = Random.Range(0, touchPointPositions.Length);
            } while (randomIndex == previousIndex);

            previousIndex = randomIndex;

            if (activeTouchPoint != null)
            {
                activeTouchPoint.SetActive(false);
            }

            activeTouchPoint = touchPointImages[randomIndex];
            activeTouchPoint.SetActive(true);

            yield return new WaitForSeconds(pointLifetime);
            activeTouchPoint.SetActive(false);
        }
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

    public void EndMixingGame()
    {
        isGameRunning = false;
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(true);

        finalScore = Mathf.RoundToInt(score / 10f);
        Debug.Log($"반죽 게임 최종 점수: {finalScore}/15");

        // finalText에 최종 점수 표시
        finalText.gameObject.SetActive(true);
        finalText.text = $"최종 점수: {finalScore} / 15";
    }

    private void Update()
    {
        if (isGameRunning && Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10f;
            handIcon.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EvaluateTouchPoint();
            handIcon.transform.localPosition = initialHandPosition;
        }
    }

    private void EvaluateTouchPoint()
    {
        if (!isGameRunning || activeTouchPoint == null) return;

        float distance = Vector2.Distance(handIcon.transform.position, activeTouchPoint.transform.position);
        int point = 0;

        if (distance <= 0.5)
        {
            point = 10;
        }
        else if (distance <= 1)
        {
            point = 6;
        }
        else if (distance <= 1.5)
        {
            point = 2;
        }
        else
        {
            point = -6;
        }

        score += point;
        ShowFloatingScore(point);
    }

    private void ShowFloatingScore(int point)
    {
        gameText.gameObject.SetActive(true);
        gameText.text = point > 0 ? $"+{point}" : $"{point}";

        Vector3 randomPosition = new Vector3(Random.Range(-200f, 200f), Random.Range(-50f, 50f), 0);
        gameText.rectTransform.anchoredPosition = randomPosition;

        StartCoroutine(FadeOutText());
    }

    private IEnumerator FadeOutText()
    {
        Color textColor = gameText.color;
        textColor.a = 1f;
        gameText.color = textColor;

        while (textColor.a > 0)
        {
            textColor.a -= Time.deltaTime * 1.5f;
            gameText.color = textColor;
            yield return null;
        }

        gameText.gameObject.SetActive(false);
    }

    private void GoToOvenPanel()
    {
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(true);
    }
}
