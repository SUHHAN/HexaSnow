using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; 
public class RunPopupManager : MonoBehaviour

{
    public GameObject runPopupPanel; // Run Story 팝업 패널
    public Button button_order;

    void Start()
    {
        runPopupPanel.SetActive(false); // 시작 시 Run 팝업 비활성화

        button_order.onClick.AddListener(() => {
            SceneManager.LoadScene("order");
    });
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

    public void deadline()
    {
        SceneManager.LoadScene("Deadline");
    }

    public void bonus()
    {
        SceneManager.LoadScene("Bonus");
    }
}

