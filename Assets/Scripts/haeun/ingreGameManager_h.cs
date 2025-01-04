using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject BadItem;
    [SerializeField] private GameObject goodItem;
    [SerializeField] private Sprite[] badItemSprites;
    [SerializeField] private Sprite[] goodItemSprites;

    private List<Vector3> badItemPositions = new List<Vector3>();
    private List<Vector3> goodItemPositions = new List<Vector3>();

    // 안내 관련 선언
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI FinishScoreText;

    private int voidScore;
    private int savedScore;
    [SerializeField] private TextMeshProUGUI voidScoreText;

    private int heartScore = 3;
    [SerializeField] private TextMeshProUGUI heartScoreText;
    [SerializeField] private TextMeshProUGUI TimeText;

    private float elapsedTime = 30f;
    private float gameDuration = 30f;

    private float minDistance = 1.0f;
    private bool isGameOver = false;
    private bool isFinalizingGame = false;

    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private TextMeshProUGUI GoText;
    private bool isGameStarting = false;

    void Start()
    {
        StartCoroutine(StartGameRoutine());
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
        TimeText.text = $"시간: {remainingTime:F1}초";
    }

    IEnumerator CreateBadItemRoutine()
    {
        while (!isGameOver)
        {
            CreateItem(BadItem, badItemPositions, badItemSprites);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator CreateGoodItemRoutine()
    {
        while (!isGameOver)
        {
            CreateItem(goodItem, goodItemPositions, goodItemSprites);
            yield return new WaitForSeconds(0.25f);
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

        heartScore--;
        heartScoreText.text = "생명 : " + heartScore;

        if (heartScore <= 0)
        {
            heartScore = 0;
            StartCoroutine(HandleGameOver());
        }
    }

    private IEnumerator HandleGameOver()
    {
        isFinalizingGame = true;
        yield return new WaitForSeconds(1.5f);
        GameOver();
    }

    public void GameOver()
    {
        isGameOver = true;
        savedScore = voidScore + heartScore * 3;
        FinishScoreText.text = "최종 점수 : " + savedScore;
        StopAllCoroutines();
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        isGameOver = false;
        isFinalizingGame = false;
        voidScore = 0;
        heartScore = 3;
        elapsedTime = gameDuration;

        voidScoreText.text = "점수 : " + voidScore;
        heartScoreText.text = "생명 : " + heartScore;
        TimeText.text = $"시간: {gameDuration:F1}초";

        gameOverPanel.SetActive(false);

        var player = FindObjectOfType<player_h>();
        if (player != null)
        {
            player.ResetPlayerState();
        }

        StartCoroutine(StartGameRoutine());
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
