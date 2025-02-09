using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchGame_h : MonoBehaviour
{
    public static MatchGame_h instance;
    private List<Card_h> allCards;
    private Card_h filppedCard;
    private bool isFlipping = false;

    [SerializeField]
    private TextMeshProUGUI TimeText;

    [SerializeField] private float timeLimit = 15f;
    private float currentTime;


    [Header("준비, 시작 텍스트 관리")]
    [SerializeField] private TextMeshProUGUI ReadyText;
    [SerializeField] private TextMeshProUGUI GoText;



    [Header("일시 정지 관리")]
    private bool isPaused = false; // 일시정지 상태 여부
    [SerializeField] private GameObject pausePanel; // 일시 정지 UI 패널


    private int totalMatches = 10;

    private int matchesFound = 0;


    [Header("베이킹 점수 관리")]
    private int BakingScore; // 이거 선우언니 점수와 연결하기

    private int FinalScore;
    private char FinalLevel;

    [SerializeField] private TextMeshProUGUI FinalScoreText;
    [SerializeField] private TextMeshProUGUI BonusScoreText;


    // GameOver : 게임 오버에 관여하는 공간
    [Header("게임 오버 관리")]
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    [SerializeField] private GameData GD = new GameData();
    [SerializeField] private MyRecipeList myRecipe;


    private bool isGameOver;

    void Awake() {
        if (instance == null){
            instance = this;
        }
    }

    void Start()
    {

        // 이전 씬에서 넘어온 Menu_Index 값을 가져오기
        if (PlayerPrefs.HasKey("SelectedMenuIndex"))
        {
            int menuIndex = PlayerPrefs.GetInt("SelectedMenuIndex") - 1;
            Debug.Log("받은 Menu_Index 값: " + menuIndex);

            // 필요한 로직 추가 (예: 특정 메뉴 정보 불러오기)
            LoadBakeData(menuIndex);
        }

        // AudioManager.Instance.PlayBgm(AudioManager.Bgm.main_bonus_ingre);


        Board_h board = FindObjectOfType<Board_h>();
        allCards = board.GetCards();

        currentTime = timeLimit;
        SetCurrentTimeText();

        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false); // 초기에는 패널 비활성화

        StartCoroutine("FilpAllCardsRoutine");
    }

    IEnumerator FilpAllCardsRoutine() {
        isFlipping = true;
        ReadyText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        FilpAllCards();
        yield return new WaitForSeconds(3f);
        FilpAllCards();
        
        ReadyText.gameObject.SetActive(false);
        GoText.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.24f);
        GoText.gameObject.SetActive(false);

        isFlipping = false;

        // 카드를 보여준 후에 시간이 넘어갈 수 있도록
        yield return StartCoroutine("CountDownTimeRoutine");
    }

    IEnumerator CountDownTimeRoutine() {
        while(currentTime > 0) {
            if (!isPaused) { // 일시 정지 중이 아닐 때만 시간 감소
                currentTime -= Time.deltaTime;
                SetCurrentTimeText();
            }
            yield return null; // 프레임 단위로 계속 실행
        }
        GameOver(false);
    }

    void SetCurrentTimeText() {
        float timeSet = Mathf.Max(0, currentTime);
        TimeText.text = $"{timeSet:F1}초";
    }

    void FilpAllCards() {
        foreach (Card_h card in allCards) {
            card.FilpCard();
        }
    }

    // Card를 게임매니저가 관리
    public void CardClicked(Card_h card) {
        if (isFlipping || isGameOver || isPaused) { // 일시 정지 중일 때 카드 클릭 불가
            return;
        }
        card.FilpCard();
        // AudioManager.Instance.PlaySfx(AudioManager.Sfx.bonus_card);

        if (filppedCard == null) { // filppedCard 안에 값이 없으면, 현 카드를 입력
            filppedCard = card;
        } else {
            // check match
            StartCoroutine(CheckMatchRoutine(filppedCard, card));
        }
    }

    IEnumerator CheckMatchRoutine(Card_h card1, Card_h card2) {
        // 2. 현재 확인하고 있는 상황일 때는 다른 카드가 선택되지 않도록 하기
        isFlipping = true;

        if (card1.cardID == card2.cardID) {
            // 두 개 카드의 id가 같으면, 매치된 상태로 두기
            card1.SetMatched();
            card2.SetMatched();

            matchesFound++;
            if (matchesFound == totalMatches) {
                GameOver(true);
            }

        } else {
            // 두개의 카드의 id가 다르면, 다시 뒤집기
            yield return new WaitForSeconds(0.5f);

            card1.FilpCard();
            card2.FilpCard();

            yield return new WaitForSeconds(0.2f);
        }

        isFlipping = false;
        filppedCard = null; // 다음번 카드를 뒤집혔을 때, 초기화가 필요함
    }

    void GameOver(bool success) {
        if (!isGameOver) {
            isGameOver = true;
            
            int menuIndex = PlayerPrefs.GetInt("SelectedMenuIndex");

            // 저장되어 있던 키를 삭제하기
            PlayerPrefs.DeleteKey("SelectedMenuIndex");
            PlayerPrefs.DeleteKey("SelectedMenuScore");
            PlayerPrefs.Save(); // 변경 사항 저장

            StopCoroutine("CountDownTimeRoutine");
            SetScoreText();

            Debug.Log($"FinalScore: {FinalScore} 확인");

            if (success) {
                gameOverText.SetText("보너스 게임 종료!");
            } else {
                gameOverText.SetText("보너스 게임 종료!");
            }

            SaveBakeData(menuIndex);

            Invoke("ShowGameOverPanel", 2f);
        }
    }

    void SetScoreText() {
        FinalScore = BakingScore + matchesFound;
        if(FinalScore >= 60) {
            FinalScore = 60;
        }

        LevelCalculate();
        TimeText.text = $"{0:F1}초";

        FinalScoreText.text = $"총 베이킹 점수 : {FinalScore}, {FinalLevel}";
        BonusScoreText.text = $"얻은 보너스 점수 : {matchesFound}";

        Debug.Log($"FinalScore: {FinalScore} 확인");

    }

    void LevelCalculate() {
        if (FinalScore >= 60) {
            FinalLevel = 'S';
        }else if(FinalScore > 40) {
            FinalLevel = 'A';
        }else if(FinalScore > 30) {
            FinalLevel = 'B';            
        }else if(FinalScore > 20) {
            FinalLevel = 'C';            
        }else if(FinalScore > 10) {
            FinalLevel = 'D';            
        }else if(FinalScore <= 10) {
            FinalLevel = 'F';            
        }
    }

    void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);

        Debug.Log($"FinalScore: {FinalScore} 확인");
    }

    // 이 코드는 다시 시작 코드가 아니라, 변한 점수, 등급 과 보너스 게임 bool 변수를 true로 바꾼 값을 저장한 뒤에 Bonus 씬으로 넘어가기
    public void MatchRestartGame(){
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        Debug.Log($"FinalScore: {FinalScore} 확인");
        
        // 보너스 게임을 인벤토리를 통해 시작한 씬으로 다시 돌아갈 수 있도록 만들기
        string SceneName = PlayerPrefs.GetString("PreviousScene", "order");
        SceneManager.LoadScene(SceneName);
    }

    public void PauseGame() {
        if (!isPaused && !isFlipping && !isGameOver) {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
            pausePanel.SetActive(true);
            isPaused = true;
        }
    }

    public void ResumeGame() {
        if (isPaused) {
            AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
            pausePanel.SetActive(false);
            StartCoroutine(ResumeReadyGoRoutine());
        }
    }

    private IEnumerator ResumeReadyGoRoutine()
    {
        // Ready 표시
        ReadyText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f); // 실시간 기준으로 대기
        ReadyText.gameObject.SetActive(false);

        // Go 표시
        GoText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f); // 실시간 기준으로 대기
        GoText.gameObject.SetActive(false);

        isPaused = false;

    }

    public void LoadBakeData(int num)
    {
        if (DataManager.Instance != null)
        {   
            GD = DataManager.Instance.LoadGameData();

            // 리스트 크기 확인 후 접근
            if (GD.myBake == null || num >= GD.myBake.Count)
            {
                Debug.LogError($"myBake 리스트의 인덱스 {num}가 범위를 벗어났습니다! 리스트 크기: {GD.myBake?.Count}");
                return;
            }

            // 정상적으로 접근 가능할 경우 데이터 로드
            myRecipe = GD.myBake[num];
            BakingScore = myRecipe.score;
        }
        else
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }


    private void SaveBakeData(int num) {
        if (DataManager.Instance != null)
        {   
            num = num - 1;
            Debug.Log($"GameData에 {num} 저장 완료!");
            
            Debug.Log($"FinalScore: {FinalScore} SaveBakeData 확인");
            // 돈을 저장한 뒤에 넘기기
            DataManager.Instance.gameData.myBake[num].score = FinalScore;
            DataManager.Instance.gameData.myBake[num].bonus = true;
            DataManager.Instance.SaveGameData(); // 저장 함수 호출
            Debug.Log($"GameData에 {DataManager.Instance.gameData.myBake[num].score},{DataManager.Instance.gameData.myBake[num].bonus} 저장 완료!");
        }
        else
        {
            Debug.LogError("DataManager 인스턴스를 찾을 수 없습니다.");
        }
    }
}
