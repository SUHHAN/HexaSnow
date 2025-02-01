using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BKStartSceneManager : MonoBehaviour
{
    public Button startButton; // Start 버튼

    void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadBakingScene); // 버튼 클릭 이벤트 등록
        }
        else
        {
            Debug.LogError("Start 버튼이 할당되지 않았습니다!");
        }
    }

    private void LoadBakingScene()
    {
        SceneManager.LoadScene("Baking 1"); 
    }
}
