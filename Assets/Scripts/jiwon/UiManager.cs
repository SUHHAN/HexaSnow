using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Button KitchenButton;

    public Button SettingButton;
    public GameObject SettingParentPanel;

    public GameObject Calender;
    public GameObject Clock;

    public Button order_button;
    public GameObject OrderBook;
    public Button RecipeButton;
    public GameObject RecipeBook;



    // Additive로 UI 씬을 로드하는 함수
    public void LoadUIScene()
    {
        SceneManager.LoadSceneAsync("Main", LoadSceneMode.Additive);
    }

    // Additive로 로드된 UI 씬을 언로드하는 함수
    public void UnloadUIScene()
    {
        SceneManager.UnloadSceneAsync("Main");
    }

    void Start()
    {
        // UI 씬을 Additive 방식으로 로드
        LoadUIScene();

        // 현재 씬에 따라 버튼을 다르게 활성화/비활성화
        if (SceneManager.GetActiveScene().name == "tutorial")
        {
            // settingsButton.gameObject.SetActive(true);  // A 씬에서는 설정 버튼만 활성화
            // button1.gameObject.SetActive(false);
            // button2.gameObject.SetActive(false);
            // button3.gameObject.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "Lobby")
        {
            SettingButton.gameObject.SetActive(true);
            Calender.gameObject.SetActive(false);  // B 씬에서는 3개 버튼만 활성화
            Clock.gameObject.SetActive(false);
            order_button.gameObject.SetActive(false);
            RecipeButton.gameObject.SetActive(false);  // B 씬에서는 3개 버튼만 활성화
        }
    }
}
