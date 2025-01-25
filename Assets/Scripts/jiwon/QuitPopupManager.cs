using UnityEngine;

public class QuitPopupManager : MonoBehaviour
{
    public GameObject quitPopupPanel;  // Quit Game 팝업 패널
    private bool isPopupActive = false; // 팝업 활성화 상태

    void Start()
    {
        quitPopupPanel.SetActive(false);  // 시작 시 Quit 팝업 비활성화
    }

    void Update()
    {
        // ESC 키를 눌렀을 때 팝업을 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPopupActive)
            {
                ShowQuitPopup();  // 팝업 활성화
            }
            else
            {
                HideQuitPopup();  // 팝업 비활성화
            }
        }
    }

    // Quit 팝업 표시
    public void ShowQuitPopup()
    {
        quitPopupPanel.SetActive(true);
        isPopupActive = true;  // 팝업이 활성화 되었음을 표시
    }

    // Quit 팝업 숨기기
    public void HideQuitPopup()
    {
        quitPopupPanel.SetActive(false);
        isPopupActive = false;  // 팝업이 비활성화 되었음을 표시
    }

    // 게임 종료 확인
    public void ConfirmQuit()
    {
        Debug.Log("게임 종료");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Unity Editor에서 실행 중이라면 종료
        #endif
    }
}