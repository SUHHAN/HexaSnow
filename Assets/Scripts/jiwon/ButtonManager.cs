using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public GameObject RunStory;  // 실행 버튼
    public GameObject QuitButton;  // 종료 버튼
    public GameObject Free;  // 자유 모드 버튼
    public GameObject RunPopupPanel;  // 실행 팝업 패널
    public GameObject SettingPanel;  // 설정 패널
    public GameObject QuitPopupPanel;  // 종료 팝업 패널

    private bool isLoggedIn = false;  // 로그인 상태

    void Start()
    {
        // 처음에는 버튼을 숨김
        HideButtons();
    }

    // 로그인 성공 시 호출되는 함수
    public void OnLoginSuccess()
    {
        Debug.Log("로그인 성공");
        isLoggedIn = true;
        ShowButtons();  // 로그인 성공 후 버튼을 보이게 합니다.
    }

    // 3개의 버튼을 보이게 하는 함수
    public void ShowButtons()
    {
        if (!IsAnyPanelActive())  // 활성화된 다른 패널이 없을 때만 버튼을 표시
        {
            RunStory.SetActive(true);
            QuitButton.SetActive(true);
            Free.SetActive(true);
        }
    }

    // 3개의 버튼을 숨기는 함수
    private void HideButtons()
    {
        RunStory.SetActive(false);
        QuitButton.SetActive(false);
        Free.SetActive(false);
    }

    // 활성화된 패널이 있는지 확인하는 함수
    private bool IsAnyPanelActive()
    {
        return RunPopupPanel.activeSelf || SettingPanel.activeSelf || QuitPopupPanel.activeSelf;
    }
}

