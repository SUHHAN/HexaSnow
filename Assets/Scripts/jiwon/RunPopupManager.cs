using UnityEngine;
using UnityEngine.SceneManagement;

public class RunPopupManager : MonoBehaviour
{
    public GameObject runPopupPanel; // Run Story 팝업 패널

    void Start()
    {
        runPopupPanel.SetActive(false); // 시작 시 Run 팝업 비활성화
    }

    // Run Story 팝업 표시
    public void ShowRunPopup()
    {
        runPopupPanel.SetActive(true);
    }

    // Run Story 팝업 숨기기
    public void HideRunPopup()
    {
        runPopupPanel.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Ingredient");
    }
}
