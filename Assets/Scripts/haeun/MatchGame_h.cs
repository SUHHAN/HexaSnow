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

    [SerializeField]
    private float timeLimit = 15f;
    private float currentTime;


    // Score : 베이킹 점수에 관여하는 공간
    [Header("베이킹 점수 관리")]
    private int totalMatches = 10;

    private int matchesFound = 0;

    [Header("베이킹 점수 관리")]
    [SerializeField] private int BakingScore; // float일수도

    private int FinalScore;
    private string FinalLevel;

    [SerializeField] private TextMeshProUGUI FinalScoreText;
    [SerializeField] private TextMeshProUGUI BonusScoreText;


    // GameOver : 게임 오버에 관여하는 공간
    [Header("게임 오버 관리")]
    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI gameOverText;

    private bool isGameOver;


    void Awake() {
        if (instance == null){
            instance = this;
        }
    }
    void Start()
    {
        Board_h board = FindObjectOfType<Board_h>();
        allCards = board.GetCards();

        currentTime = timeLimit;
        SetCurrentTimeText();

        gameOverPanel.SetActive(false);

        StartCoroutine("FilpAllCardsRoutine");
    }

    IEnumerator FilpAllCardsRoutine() {
        isFlipping = true;
        yield return new WaitForSeconds(0.5f);
        FilpAllCards();
        yield return new WaitForSeconds(2f);
        FilpAllCards();
        yield return new WaitForSeconds(0.5f);
        isFlipping = false;

        // 카드를 보여준 후에 시간이 넘어갈 수 있도록
        yield return StartCoroutine("CountDownTimeRoutine");
    }

    IEnumerator CountDownTimeRoutine() {
        while(currentTime > 0) {
            currentTime -= Time.deltaTime;
            SetCurrentTimeText();
        
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

        if (isFlipping || isGameOver) {
            return;
        }
        card.FilpCard();

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
            yield return new WaitForSeconds(0.7f);

            card1.FilpCard();
            card2.FilpCard();

            yield return new WaitForSeconds(0.4f);
        }

        isFlipping = false;
        filppedCard = null; // 다음번 카드를 뒤집혔을 때, 초기화가 필요함
    }

    void GameOver(bool success) {

        if (!isGameOver) {
            isGameOver = false;

            StopCoroutine("CountDownTimeRoutine");
            SetScoreText();

            if (success) {
                gameOverText.SetText("게임 종료! - 성공");
            } else {
                gameOverText.SetText("게임 종료! - 실패");
            }

            Invoke("ShowGameOverPanel", 2f);
        }
    }

    void SetScoreText() {
        FinalScore = BakingScore + matchesFound;

        FinalScoreText.text = $"총 베이킹 점수 : {FinalScore}";
        BonusScoreText.text = $"얻은 보너스 점수 : {matchesFound}";

    }

    void ShowGameOverPanel() {
        gameOverPanel.SetActive(true);
    }

    void MatchRestartGame(){
        SceneManager.LoadScene("Match");
    }
}
