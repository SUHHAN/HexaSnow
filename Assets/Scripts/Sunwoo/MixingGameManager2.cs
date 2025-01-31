using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager2 : MonoBehaviour
{
    // UI 패널 및 요소
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

    // Bowl 이미지
    public GameObject bowlBefore; // BowlBefore 이미지
    public GameObject bowlAfter; // BowlAfter 이미지
    private bool hasAnimationStarted = false; // 애니메이션 실행 여부 확인

    // 상하좌우 버튼
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;

    // 아이콘과 동그라미 위치
    public Transform iconSpawnPoint; // 아이콘이 생성되는 위치 (오른쪽)
    public Transform targetCircle;   // 동그라미 위치 (왼쪽, 버튼이 눌리는 곳)

    public GameObject upIconPrefab;
    public GameObject downIconPrefab;
    public GameObject leftIconPrefab;
    public GameObject rightIconPrefab;

    private List<GameObject> activeIcons = new List<GameObject>();
    private float moveDuration = 1f; // 아이콘이 1초 만에 도착하도록 설정
    private float gameDuration = 30f; // 게임 지속 시간
    private float remainingTime;
    private bool isGameRunning = false;
    private int score = 0;

    // Mixing 애니메이션 관련
    public Image doughImage; // UI Image (애니메이션을 표시할 대상)
    public Image doughImage2; // UI Image
    private Sprite[] doughSprites; // 스프라이트 배열
    private int currentFrame = 0;
    private float frameDuration = 30f / 85f; // 30초 동안 17장 애니메이션
    private bool isAnimating = false;

    private void Start()
    {
        // 초기 UI 설정
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false);

        bowlBefore.SetActive(true);
        bowlAfter.SetActive(false);
        startButton.gameObject.SetActive(false); // Start 버튼 초기 비활성화
        startButton.gameObject.SetActive(false);

        // 버튼 이벤트 등록
        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel);

        upButton.onClick.AddListener(() => CheckInput("Up"));
        downButton.onClick.AddListener(() => CheckInput("Down"));
        leftButton.onClick.AddListener(() => CheckInput("Left"));
        rightButton.onClick.AddListener(() => CheckInput("Right"));

        // 애니메이션 스프라이트 로드
        LoadDoughSprites();
    }

    // Mixing 게임 시작
    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false);
        mixingGamePanel.SetActive(true);
        StartCoroutine(StartGameSequence());
    }

    // `Resources` 폴더에서 스프라이트 로드
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
        mixingPanel.SetActive(true); // Mixing 전체 패널 활성화
        startMixingPanel.SetActive(true); // StartMixingPanel 활성화

        if (!hasAnimationStarted)
        {
            StartCoroutine(BowlAnimation());
            hasAnimationStarted = true; // 애니메이션 한 번만 실행되도록 설정
        }
    }

    private IEnumerator BowlAnimation()
    {
        Debug.Log("BowlAnimation 시작");

        float rotationDuration = 1f; // Bowl 회전 지속 시간 
        float elapsedTime = 0f;
        float rotationSpeed = 1080f; // 회전 속도 (기본 360도에서 증가)

        // BowlBefore 빠르게 회전
        while (elapsedTime < rotationDuration)
        {
            bowlBefore.transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime); // 1080도 회전 (초당 3바퀴)
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("BowlAnimation 종료");

        // BowlBefore 비활성화 및 BowlAfter 활성화
        bowlBefore.SetActive(false);
        bowlAfter.SetActive(true);

        // Start 버튼 활성화
        startButton.gameObject.SetActive(true);
    }

    private IEnumerator StartGameSequence()
    {
        // Ready? 표시
        readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.gameObject.SetActive(false);

        // Start! 표시
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        // 게임 시작
        isGameRunning = true;
        remainingTime = gameDuration;

        StartCoroutine(GameTimer());
        StartCoroutine(PlayDoughAnimation());
        StartCoroutine(SpawnIcons());
    }

    // 애니메이션 실행 (이미지 변경)
    private IEnumerator PlayDoughAnimation()
    {
        isAnimating = true;
        currentFrame = 0;

        while (isGameRunning)
        {
            doughImage.sprite = doughSprites[currentFrame]; // 현재 프레임 변경
            currentFrame = (currentFrame + 1) % doughSprites.Length; // 다음 프레임으로 변경

            yield return new WaitForSeconds(frameDuration); // 프레임 지속 시간 대기
        }
    }

    private IEnumerator SpawnIcons()
    {
        while (isGameRunning)
        {
            int iconCount = Random.Range(1, 5); // 한 번에 최대 5개의 아이콘 생성
            float startX = iconSpawnPoint.position.x;
            float startY = iconSpawnPoint.position.y;
            float spacing = Random.Range(1f, 3f); // 아이콘 간 거리 1~3칸 랜덤 설정

            List<float> usedPositions = new List<float>(); // 이미 사용된 위치 저장

            for (int i = 0; i < iconCount; i++)
            {
                // 랜덤 위치 설정 (서로 겹치지 않도록)
                float offset = Random.Range(-1.5f, 1.5f) * spacing;
                while (usedPositions.Contains(offset))
                {
                    offset = Random.Range(-1.5f, 1.5f) * spacing;
                }
                usedPositions.Add(offset);

                Vector3 spawnPosition = new Vector3(startX, startY + offset, 0);

                GameObject newIcon = Instantiate(GetRandomIcon(), spawnPosition, Quaternion.identity);
                activeIcons.Add(newIcon);

                // 아이콘이 1초 동안 목표 지점으로 이동
                StartCoroutine(MoveIcon(newIcon, spawnPosition, targetCircle.position));
            }

            yield return new WaitForSeconds(Random.Range(1f, 2f)); // 다음 아이콘 생성까지 대기
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
        return Instantiate(upIconPrefab); // 기본값 (절대 실행될 일 없음)
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

        // 가장 가까운 아이콘 찾기
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

        string iconType = closestIcon.tag; // 아이콘은 "Up", "Down", "Left", "Right" 태그를 가짐

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
            score -= 3; // 틀린 버튼 입력
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

        // 최종 점수 출력
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
