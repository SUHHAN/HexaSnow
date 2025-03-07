using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BKStartSceneManager : MonoBehaviour
{
    public Button startButton; // 일반 버튼
    public Button specialButton; // 특별손님 버튼
    public GameObject StartPanel;

    void Start()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadBakingScene);
        }
        else if (specialButton != null)
        {
            specialButton.onClick.AddListener(LoadSpecialBakingScene);
        }
        UiLogicManager.Instance.LoadMoneyData();
    }

    public void LoadBakingScene()
    {
        SceneManager.LoadScene("Baking 1");
    }

    public void LoadSpecialBakingScene()
    {
        SceneManager.LoadScene("BakingSp");
    }
}
