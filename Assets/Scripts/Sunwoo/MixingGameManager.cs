using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager : MonoBehaviour
{
    // 패널 및 UI 요소
    public GameObject startMixingPanel;
    public GameObject mixingGamePanel;
    public GameObject finishMixingPanel;
    public GameObject ovenPanel;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI startText;
    public Button startButton;
    public Button nextButton;
    public TextMeshProUGUI scoreText;

    // 터치 포인트와 손 아이콘
    public GameObject handIcon;
    public Transform[] touchPointPositions; // 터치 포인트 위치 배열
    public GameObject[] touchPointImages; // 터치 포인트 이미지 배열

    // 게임 설정
    public float pointLifetime = 1.5f; // 터치 포인트 유지 시간
    public float gameDuration = 30f; // 게임 지속 시간
    private float remainingTime;

    private bool isGameRunning = false; // 게임 진행 상태
    private int score = 0; // 현재 점수

    private GameObject activeTouchPoint = null; // 현재 활성화된 터치 포인트
    private int previousIndex = -1; // 이전에 생성된 터치 포인트 인덱스 (-1은 초기값)

    private void Start()
    {
        // 초기 UI 상태 설정
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false); // Oven 패널 초기 비활성화

        // Start 버튼 클릭 이벤트 등록
        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel); // Next 버튼 클릭 이벤트 등록
    }

    // Mixing 게임 시작
    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false); // StartMixing 패널 비활성화
        StartCoroutine(StartGameSequence());
    }

    // Ready?와 Start! 텍스트 표시 및 Mixing 게임 시작
    private IEnumerator StartGameSequence()
    {
        mixingGamePanel.SetActive(true); // MixingGame 패널 활성화

        // Ready? 표시
        readyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        readyText.gameObject.SetActive(false);

        // Start! 표시
        startText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        startText.gameObject.SetActive(false);

        // 터치 포인트 모두 비활성화
        foreach (GameObject touchPoint in touchPointImages)
        {
            touchPoint.SetActive(false);
        }

        // 게임 시작
        isGameRunning = true;
        remainingTime = gameDuration;
        StartCoroutine(GameTimer());
        StartCoroutine(SpawnTouchPoints());
    }

    // 터치 포인트 스폰 (한 번에 하나만 활성화, 이전 위치 제외)
    private IEnumerator SpawnTouchPoints()
    {
        while (isGameRunning)
        {
            int randomIndex;

            // 이전 인덱스와 다른 위치 선택
            do
            {
                randomIndex = Random.Range(0, touchPointPositions.Length);
            } while (randomIndex == previousIndex);

            previousIndex = randomIndex; // 현재 인덱스를 이전 인덱스로 저장

            if (activeTouchPoint != null)
            {
                activeTouchPoint.SetActive(false); // 이전 활성화된 포인트 비활성화
            }

            activeTouchPoint = touchPointImages[randomIndex];
            activeTouchPoint.SetActive(true); // 새 포인트 활성화

            yield return new WaitForSeconds(pointLifetime); // 유지 시간 대기
            activeTouchPoint.SetActive(false); // 포인트 비활성화
        }
    }

    // 게임 타이머
    private IEnumerator GameTimer()
    {
        while (remainingTime > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        EndMixingGame(); // 게임 종료
    }

    // Mixing 게임 종료
    public void EndMixingGame()
    {
        isGameRunning = false;
        mixingGamePanel.SetActive(false); // MixingGame 패널 비활성화
        finishMixingPanel.SetActive(true); // FinishMixing 패널 활성화

        // 점수 표시
        scoreText.text = "점수: " + score;
    }

    // 손 아이콘 마우스 드래그로 이동
    private void Update()
    {
        if (isGameRunning && Input.GetMouseButton(0)) // 마우스 왼쪽 버튼 클릭
        {
            Vector3 mousePosition = Input.mousePosition; // 마우스 위치 가져오기
            mousePosition.z = 10f; // 카메라와의 거리 설정
            handIcon.transform.position = Camera.main.ScreenToWorldPoint(mousePosition); // 손 아이콘 위치 업데이트
        }

        if (Input.GetMouseButtonUp(0)) // 마우스 왼쪽 버튼을 놓을 때
        {
            EvaluateTouchPoint();
            handIcon.transform.localPosition = Vector3.zero; // 원래 위치로 복귀
        }
    }

    // 손 아이콘과 터치 포인트의 거리 계산 및 점수 판정
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

    // Oven 패널로 이동
    private void GoToOvenPanel()
    {
        finishMixingPanel.SetActive(false); // FinishMixing 패널 비활성화
        ovenPanel.SetActive(true); // Oven 패널 활성화
    }
}
