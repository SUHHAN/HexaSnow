using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ingreGameManager_h : MonoBehaviour
{
    // 싱글톤 생성
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
    [SerializeField] private GameObject BadItem; // 나쁜 아이템 프리팹

    [SerializeField] private GameObject goodItem; // 좋은 아이템 프리팹

    [SerializeField]
    private Sprite[] badItemSprites; // 나쁜 아이템 스프라이트 배열

    [SerializeField]
    private Sprite[] goodItemSprites; // 좋은 아이템 스프라이트 배열

    private List<Vector3> badItemPositions = new List<Vector3>(); // 적 생성 위치 리스트
    private List<Vector3> goodItemPositions = new List<Vector3>(); // 좋은 아이템 생성 위치 리스트


    // 상태 표시 관련 선언
    [SerializeField]
    private GameObject gameOverPanel; // 게임 오버 패널
    [SerializeField] private TextMeshProUGUI FinishScoreText;

    private int voidScore; // 현재 점수
    private int savedScore; // 저장된 점수
    [SerializeField] private TextMeshProUGUI voidScoreText;

    private int heartScore = 3; // 현재 생명
    [SerializeField] private TextMeshProUGUI heartScoreText;
    [SerializeField] private TextMeshProUGUI TimeText;
    private float elapsedTime = 30f; // 초기 시간은 30초
    private float gameDuration = 30f;


    private float minDistance = 1.0f; // 생성된 아이템 간 최소 거리
    private bool isGameOver = false;
    private bool isFinalizingGame = false; // 2초 동안 최종 상태를 처리하기 위한 플래그

    // 시작 관련 선언
    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private TextMeshProUGUI GoText;
    private bool isGameStarting = false; // Ready/Go 표시 중인지 여부



    void Start()
    {
        StartCoroutine(StartGameRoutine()); // Ready/Go 처리 포함한 게임 시작
    }

    void Update() 
    {
        if (!isGameOver && !isFinalizingGame)
        {
            UpdateTimer(); // 타이머 업데이트
        }
    }

    private void UpdateTimer()
    {
        elapsedTime -= Time.deltaTime; // 시간 감소

        // 타이머 텍스트 업데이트
        float remainingTime = Mathf.Max(0, elapsedTime);
        TimeText.text = $"시간: {remainingTime:F1}초";

        // 시간이 0초가 되면 게임 오버 처리
        if (elapsedTime <= 0)
        {
            StartCoroutine(HandleGameOver());
        }
    }

    public bool IsGameStarting()
    {
        return isGameStarting; // Ready/Go 표시 상태 반환
    }

    private IEnumerator StartGameRoutine()
    {
        isGameStarting = true;

        // Ready 표시
        ReadyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        ReadyText.gameObject.SetActive(false);

        // 게임 시작
        isGameStarting = false;
        StartGame();

        // Go 표시
        GoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        GoText.gameObject.SetActive(false);
    }

    private void StartGame()
    {
        // 적과 좋은 아이템 생성 시작
        StartCoroutine(CreateBadItemRoutine());
        StartCoroutine(CreateGoodItemRoutine());
        StartCoroutine(CleanupPositionsRoutine());
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
            AssignRandomSprite(item, itemSprites); // 랜덤 스프라이트 할당
        }
    }

    private void AssignRandomSprite(GameObject item, Sprite[] itemSprites)
    {
        if (itemSprites.Length == 0) return; // 스프라이트가 없으면 아무 작업도 하지 않음

        SpriteRenderer renderer = item.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            int randomIndex = Random.Range(0, itemSprites.Length);
            renderer.sprite = itemSprites[randomIndex]; // 랜덤 스프라이트 할당
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
        if (isFinalizingGame) return; // 최종 상태에서는 점수 증가 금지
        voidScore = voidScore + 3;
        voidScoreText.text = "점수 : " + voidScore;
    }
    public void BackVoidScore()
    {
        if (isFinalizingGame) return; // 최종 상태에서는 점수 증가 금지
        
        voidScore = voidScore - 1;
        if (voidScore <= 0) {
            voidScore = 0;
        }   // 점수가 음수가 되지는 않도록 조건 설정해두기

        voidScoreText.text = "점수 : " + voidScore;
    }

    public void BackHeartScore()
    {
        if (isFinalizingGame) return; // 최종 상태에서는 생명 감소 금지

        heartScore--;
        heartScoreText.text = "생명 : " + heartScore;

        if (heartScore <= 0)
        {
            heartScore = 0;
            StartCoroutine(HandleGameOver()); // 2초 지연 후 게임 오버 처리
        }
    }

    private IEnumerator HandleGameOver()
    {
        isFinalizingGame = true; // 최종 상태로 전환
        yield return new WaitForSeconds(1.5f); // 1.5초 지연
        GameOver(); // 게임 오버 처리
    }

    public void GameOver()
    {
        isGameOver = true;
        savedScore = voidScore + heartScore * 3; // 점수 저장
        FinishScoreText.text = "최종 점수 : " + savedScore;
        StopAllCoroutines(); // 모든 코루틴 중지
        gameOverPanel.SetActive(true); // 게임 오버 패널 활성화
    }

    public void RestartGame()
    {
        // 초기화
        isGameOver = false;
        isFinalizingGame = false;
        voidScore = 0;
        heartScore = 3;
        elapsedTime = gameDuration;

        voidScoreText.text = "점수 : " + voidScore;
        heartScoreText.text = "생명 : " + heartScore;
        TimeText.text = $"남은 시간: {gameDuration:F1}초";

        gameOverPanel.SetActive(false); // 게임 오버 패널 숨김

        // 플레이어 초기화
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
        return savedScore; // 저장된 점수 반환
    }

    public bool IsGameOverFinalizing() // 최종 상태 확인 메서드
    {
        return isFinalizingGame; // 생명이 0이고 2초 지연 중일 때 true 반환
    }
}
