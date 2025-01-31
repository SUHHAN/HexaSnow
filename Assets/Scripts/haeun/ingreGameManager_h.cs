using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class ingreGameManager_h : MonoBehaviour
{
    private static ingreGameManager_h _instance;
    public static ingreGameManager_h Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ingreGameManager_h>();
            }
            return _instance;
        }
    }

    // 아이템 관련 선언
    [Header("떨어지는 아이템 관리")]
    [SerializeField] private GameObject BadItem;
    [SerializeField] private GameObject goodItem;
    [SerializeField] private Sprite[] badItemSprites;
    [SerializeField] private Sprite[] goodItemSprites;

    private List<Vector3> badItemPositions = new List<Vector3>();
    private List<Vector3> goodItemPositions = new List<Vector3>();

    // 안내 관련 선언
    [Header("안내 패널 관리")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI FinishScoreText;

    private int voidScore;
    private int savedScore;
    [SerializeField] private TextMeshProUGUI voidScoreText;

    public int heartScore = 3;

    // 아래 heartO의 3개의 하트 이미지가 heartScore가 1씩 없어질 때마다 heartX로 스프라이트를 변경했으면 좋겠음
    [SerializeField] private GameObject[] heartO;
    [SerializeField] private Sprite heartX;
    [SerializeField] private Sprite originalHeartSprite;
    [SerializeField] private TextMeshProUGUI TimeText;

    private float elapsedTime = 30f;
    private float gameDuration = 30f;

    private float minDistance = 1.0f;
    private bool isGameOver = false;
    private bool isFinalizingGame = false;

    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private TextMeshProUGUI GoText;
    private bool isGameStarting = false;

    [Header("게임 오버 시 이미지/텍스트 관리")]
    [SerializeField] private GameObject PlayerIdle;
    [SerializeField] private Sprite PlayerDeath;
    [SerializeField] private GameObject ingamePlayScore;
    private Animator animator;



    void Start()
    {
        animator = PlayerIdle.GetComponent<Animator>();


        StartCoroutine(StartGameRoutine());

        gameOverPanel.SetActive(false);

    }

    void Update()
    {
        if (!isGameOver && !isFinalizingGame && !isGameStarting)
        {
            UpdateTimer();
        }
    }
    

    private IEnumerator StartGameRoutine()
    {
        isGameStarting = true;

        // Ready 표시
        ReadyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        ReadyText.gameObject.SetActive(false);

        // Go 표시
        GoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        GoText.gameObject.SetActive(false);

        // 게임 시작
        isGameStarting = false;
        StartGame();
    }

    private void StartGame()
    {
        // 적과 좋은 아이템 생성 시작
        StartCoroutine(CreateBadItemRoutine());
        StartCoroutine(CreateGoodItemRoutine());
        StartCoroutine(CleanupPositionsRoutine());
    }

    private void UpdateTimer()
    {
        elapsedTime -= Time.deltaTime;
        UpdateTimerText();

        if (elapsedTime <= 0)
        {
            StartCoroutine(HandleGameOver());
        }
    }


    private void UpdateTimerText()
    {
        float remainingTime = Mathf.Max(0, elapsedTime);
        TimeText.text = $"{remainingTime:F1}초";
    }

    IEnumerator CreateBadItemRoutine()
    {
        while (!isGameOver)
        {
            CreateItem(BadItem, badItemPositions, badItemSprites);
            yield return new WaitForSeconds(0.8f);
        }
    }

    IEnumerator CreateGoodItemRoutine()
    {
        while (!isGameOver)
        {
            CreateItem(goodItem, goodItemPositions, goodItemSprites);
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void CreateItem(GameObject itemPrefab, List<Vector3> positionList, Sprite[] itemSprites)
    {
        Vector3 pos;
        int maxAttempts = 10;
        int attempt = 0;

        do
        {
            pos = Camera.main.ViewportToWorldPoint(new Vector3(Random.Range(0.0f, 1.0f), 1.1f, 0));
            pos.z = 0.0f;
            attempt++;
        }
        while (IsPositionOccupied(pos, positionList) && attempt < maxAttempts);

        if (attempt < maxAttempts)
        {
            positionList.Add(pos);
            GameObject item = Instantiate(itemPrefab, pos, Quaternion.identity);
            AssignRandomSprite(item, itemSprites);
        }
    }

    private void AssignRandomSprite(GameObject item, Sprite[] itemSprites)
    {
        if (itemSprites.Length == 0) return;

        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            int randomIndex = Random.Range(0, itemSprites.Length);
            renderer.sprite = itemSprites[randomIndex];
        }
    }

    private bool IsPositionOccupied(Vector3 position, List<Vector3> positionList)
    {
        foreach (var spawnedPos in positionList)
        {
            if (Vector3.Distance(position, spawnedPos) < minDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void GetVoidScore()
    {
        if (isFinalizingGame) return;
        voidScore += 3;
        voidScoreText.text = "점수 : " + voidScore;
    }

    public void BackVoidScore()
    {
        if (isFinalizingGame) return;

        voidScore = Mathf.Max(0, voidScore - 1);
        voidScoreText.text = "점수 : " + voidScore;
    }

    public void BackHeartScore()
    {
        if (isFinalizingGame) return;

        if (heartScore > 0)
        {
            // 현재 heartScore에 해당하는 하트를 빈 하트(heartX)로 변경
            heartO[heartScore - 1].GetComponent<UnityEngine.UI.Image>().sprite = heartX;

            heartScore--; // 목숨 감소
        }

        if (heartScore <= 0)
        {
            heartScore = 0;
            StartCoroutine(HandleGameOver());
        }
    }

    private IEnumerator HandleGameOver()
    {
        isFinalizingGame = true;
        ingamePlayScore.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        GameOver();
    }

    public void GameOver()
    {
        isGameOver = true;
        savedScore = voidScore + heartScore * 3;

        Debug.Log($"남은 목숨  = {heartScore}개");

        // 인게임 플레이 점수 없애기
        ingamePlayScore.SetActive(false);

        ScrollbarManager.Instance.SetFinalScore(savedScore);

        FinishScoreText.text = "최종 점수 : " + savedScore + "점";
        StopAllCoroutines();
        gameOverPanel.SetActive(true);
    }

    

    // 상점 떠나기 버튼을 눌렀다면 아래 함수로 이동하도록
    public void OnOutStoreButton()
    {
        SceneManager.LoadScene("Deadline_Last");
    }

    IEnumerator CleanupPositionsRoutine()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(1.0f);
            CleanupPositions(badItemPositions);
            CleanupPositions(goodItemPositions);
        }
    }

    private void CleanupPositions(List<Vector3> positionList)
    {
        positionList.Clear();
    }

    public int GetSavedScore()
    {
        return savedScore;
    }

    public bool IsGameOverFinalizing()
    {
        return isFinalizingGame;
    }

    public bool IsGameStarting()
    {
        return isGameStarting; // Ready/Go 상태 반환
    }
}
