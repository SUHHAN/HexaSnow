using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RunPopupManager : MonoBehaviour

{
    public GameObject runPopupPanel; // Run Story 팝업 패널
    public Button button_order;
    public GameObject backgroundClickArea;

    void Start()
    {
        runPopupPanel.SetActive(false); // 시작 시 Run 팝업 비활성화

        // 배경 클릭 감지 영역에 클릭 이벤트 추가
      /*  if (backgroundClickArea != null)
        {
            backgroundClickArea.GetComponent<Button>().onClick.AddListener(() =>
            {
                HideRunPopup(); // 바깥 클릭 시 팝업 닫기
           });
         }
         */

        button_order.onClick.AddListener(() => {
            Debug.Log("order 씬으로 이동");
            SceneManager.LoadScene("order");
    });
    }

    // Run Story 팝업 표시
    public void ShowRunPopup()
    {
        runPopupPanel.SetActive(true);
        backgroundClickArea.SetActive(true);
    }

    // Run Story 팝업 숨기기
    public void HideRunPopup()
    {
        runPopupPanel.SetActive(false);
        backgroundClickArea.SetActive(false);
    }

    // 새로운 게임 시작 (기존 데이터 삭제 후 새로 시작)
    public void StartNewGame()
    {
        DataManager.Instance.gameData = new GameData(); // 새로운 데이터로 초기화
        DataManager.Instance.SaveInitialGameData(); // 저장
        SceneManager.LoadScene("order"); // 새 게임 씬으로 이동
        Debug.Log("새로하기 초기화-튜토리얼 씬으로 이동");
    }

    // 이어하기 (저장된 데이터 불러와 게임 시작)
    public void ContinueGame()
    {
        GameData loadedData = DataManager.Instance.LoadGameData();

        if (loadedData != null)
        {
            DataManager.Instance.gameData = loadedData; // 불러온 데이터를 적용
            Debug.Log("이어하기 진행: 저장된 데이터 로드 완료");
            SceneManager.LoadScene("Main"); // 기존 진행 상태에서 게임 시작
        }
        else
        {
            Debug.LogWarning("저장된 데이터가 없어 새 게임을 시작합니다.");
            StartNewGame();
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Ingredient");
    }

    public void deadline()
    {
        SceneManager.LoadScene("Deadline");
    }

    public void bonus()
    {
        SceneManager.LoadScene("Bonus");
    }
}

