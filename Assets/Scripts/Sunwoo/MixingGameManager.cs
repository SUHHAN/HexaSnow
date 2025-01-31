using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MixingGameManager : MonoBehaviour
{
    // 패널 및 UI 요소
    public GameObject mixingPanel; // Mixing 전체 패널
    public GameObject startMixingPanel;
    public GameObject mixingGamePanel;
    public GameObject finishMixingPanel;
    public GameObject ovenPanel;

    public TextMeshProUGUI readyText;
    public TextMeshProUGUI startText;
    public Button startButton;
    public Button nextButton;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI gameText;
    public TextMeshProUGUI finalText;

    // 터치 포인트와 손 아이콘
    public GameObject handIcon;
    private Vector3 initialHandPosition = new Vector3(0, -100, 0);
    public Transform[] touchPointPositions; // 터치 포인트 위치 배열
    public GameObject[] touchPointImages; // 터치 포인트 이미지 배열

    // Bowl 이미지
    public GameObject bowlBefore; // BowlBefore 이미지
    public GameObject bowlAfter; // BowlAfter 이미지
    private bool hasAnimationStarted = false; // 애니메이션 실행 여부 확인

    // 게임 설정
    public float pointLifetime = 1f; // 터치 포인트 유지 시간
    public float gameDuration = 30f; // 게임 지속 시간
    private float remainingTime;

    private bool isGameRunning = false; // 게임 진행 상태
    private int score = 0; // 현재 점수
    private GameObject activeTouchPoint = null; // 현재 활성화된 터치 포인트
    private int previousIndex = -1; // 이전에 생성된 터치 포인트 인덱스 (-1은 초기값)

    // Mixing 애니메이션 관련
    public Image doughImage; // UI Image (애니메이션을 표시할 대상)
    public Image doughImage2; // UI Image
    private Sprite[] doughSprites; // 스프라이트 배열
    private int currentFrame = 0;
    private float frameDuration = 30f / 85f; // 30초 동안 17장 애니메이션
    private bool isAnimating = false;

    public int finalScore = 0;

    private void Start()
    {
        // 초기 UI 상태 설정
        readyText.gameObject.SetActive(false);
        startText.gameObject.SetActive(false);
        mixingGamePanel.SetActive(false);
        finishMixingPanel.SetActive(false);
        ovenPanel.SetActive(false); // Oven 패널 초기 비활성화

        bowlBefore.SetActive(true);
        bowlAfter.SetActive(false);
        startButton.gameObject.SetActive(false); // Start 버튼 초기 비활성화

        // Start 버튼 클릭 이벤트 등록
        startButton.onClick.AddListener(StartMixingGame);
        nextButton.onClick.AddListener(GoToOvenPanel); // Next 버튼 클릭 이벤트 등록

        // 애니메이션 스프라이트 로드
        LoadDoughSprites();
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

    // Mixing 게임 시작
    public void StartMixingGame()
    {
        startMixingPanel.SetActive(false); // StartMixing 패널 비활성화
        mixingGamePanel.SetActive(true); // MixingGame 패널 활성화
        StartCoroutine(StartGameSequence());
    }

    // Ready?와 Start! 텍스트 표시 및 Mixing 게임 시작
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

        doughImage2.gameObject.SetActive(false);
        doughImage.gameObject.SetActive(true);

        // 터치 포인트 모두 비활성화
        foreach (GameObject touchPoint in touchPointImages)
        {
            touchPoint.SetActive(false);
        }

        // 게임 시작
        isGameRunning = true;
        remainingTime = gameDuration;
        Debug.Log("반죽 애니메이션 시작: IsMixing = true");
        
        StartCoroutine(GameTimer());
        StartCoroutine(PlayDoughAnimation());
        StartCoroutine(SpawnTouchPoints());
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
        Debug.Log("반죽 애니메이션 종료: IsMixing = false");
        isAnimating = false;

        finalScore = Mathf.RoundToInt(score / 10f);
        Debug.Log($"반죽 게임 최종 점수: {finalScore}/15");
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
            handIcon.transform.localPosition = initialHandPosition; // 원래 위치로 복귀
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
            score += 10; // Perfect
            Debug.Log("Perfect! +5");
        }
        else if (distance <= 1)
        {
            score += 6; // Great
            Debug.Log("Great! +3");
        }
        else if (distance <= 1.5)
        {
            score += 2; // Good
            Debug.Log("Good! +1");
        }
        else if (distance <= 2)
        {
            Debug.Log("Bad! +0");
        }
        else
        {
            score -= 6; // Miss
            Debug.Log("Miss! -3");
        }

        ShowFloatingScore(score);
    }

    private void ShowFloatingScore(int point)
    {
        gameText.gameObject.SetActive(true);
        gameText.text = point > 0 ? $"+{point}" : $"{point}";

        // 점수 위치를 랜덤으로 조정 (UI 기준)
        Vector3 randomPosition = new Vector3(Random.Range(-200f, 200f), Random.Range(-50f, 50f), 0);
        gameText.rectTransform.anchoredPosition = randomPosition;

        // 점점 사라지는 애니메이션
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

    // Oven 패널로 이동
    private void GoToOvenPanel()
    {
        finishMixingPanel.SetActive(false); // FinishMixing 패널 비활성화
        ovenPanel.SetActive(true); // Oven 패널 활성화
    }
}