using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject popupPanel; // PopupPanel 연결 필드
    private bool isPopupActive = false;

    void Update()
    {
        // 뒤로 가기 버튼 입력 감지
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPopupActive)
            {
                ShowPopup();
            }
            else
            {
                HidePopup();
            }
        }
    }

    public void ShowPopup()
    {
        popupPanel.SetActive(true); // PopupPanel 활성화
        isPopupActive = true;
    }

    public void HidePopup()
    {
        popupPanel.SetActive(false); // PopupPanel 비활성화
        isPopupActive = false;
    }

    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}