using TMPro;
using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스 추가
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class UiManagerTest : MonoBehaviour
{
    public GameObject OrderBook;  // OrderBook 패널
    public Button order_button;    // Order 버튼
    public GameObject order_buttonGO;    // Order 버튼

    public GameObject RecipeBook;
    public Button RecipeButton;
    public GameObject RecipeBookGO;


    public GameObject CookButton;
    public GameObject datePanel;
    public GameObject timePanel;

    public TMP_Text calendarText;
    public TMP_FontAsset customFont;

    void Start()
    {

        // 현재 씬 이름 가져오기
        string currentSceneName = SceneManager.GetActiveScene().name;

        // OrderButton 클릭 이벤트 등록
        order_button.onClick.AddListener(OnOrderBook);
        RecipeButton.onClick.AddListener(OnRecipeBook);
        LoadCalendarDate__();

        // 씬 이름에 따라 버튼 활성화 설정
        if (currentSceneName == "tutorial")
        {
            CookButton.SetActive(false);
            datePanel.SetActive(false);
            timePanel.SetActive(false);
            order_buttonGO.SetActive(false);
            RecipeBookGO.SetActive(false);
        }

        // else
        // {
        //     LoadButton.SetActive(true);
        //     IngameButton.SetActive(false);
            
        // }

        // // 씬 이름에 따라 버튼 활성화 설정
        // if (currentSceneName == "Tutorial" 
        //     || currentSceneName == "StoreMain" 
        //     || currentSceneName == "PartyMain")
        // {
        //     LoadButton.SetActive(false);
        //     IngameButton.SetActive(true);
        // }
        // else
        // {
        //     LoadButton.SetActive(true);
        //     IngameButton.SetActive(false);
            
        // }
    }
    

    void OnOrderBook()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        OrderBook.SetActive(!OrderBook.activeSelf);
    }

    void OnRecipeBook()
    {
        AudioManager.Instance.PlaySfx(AudioManager.Sfx.button);
        RecipeBook.SetActive(!RecipeBook.activeSelf);
    }

    public void LoadCalendarDate__()
    {
        // DataManager.Instance가 null인지 확인
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager.Instance가 null입니다! DataManager가 씬에 존재하는지 확인하세요.");
            return;
        }

        // LoadGameData()를 통해 GameData 가져오기
        GameData dateGD = DataManager.Instance.LoadGameData();


        // 정상적으로 값이 있으면 텍스트 설정
        calendarText.text = $"{dateGD.date}일차";
        Debug.Log($"Calendar updated with date: {dateGD.date}일차");

        // 폰트 설정
        if (customFont != null)
        {
            calendarText.font = customFont; // 커스텀 폰트를 설정
        }
        else
        {
            Debug.LogError("폰트가 설정되지 않았습니다! Unity Inspector에서 customFont를 설정하세요.");
        }

        // 텍스트 속성 설정
        calendarText.color = UnityEngine.Color.black;  // 텍스트 색상 설정
        calendarText.fontSize = 30;  // 폰트 크기 설정
    }
}
