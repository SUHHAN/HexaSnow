using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.VisualScripting;

public class PopupManager : MonoBehaviour
{
    private static PopupManager _instance;
    public static PopupManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PopupManager>();
            }
            return _instance;
        }
    }
    
    private int CurrentPage = 0; // 기본 페이지는 0으로 두기
    private int MaxPage = 0;
    private string currentSceneName;
    private bool tutoBool = false;
    public bool isDone = false;
    [SerializeField] private Sprite[] PageImage;
    [SerializeField] private GameObject ImagePanel;

    [SerializeField] private GameObject StartButton;
    [SerializeField] private GameObject NextButton;
    [SerializeField] private GameObject PreviousButton;
    [SerializeField] private GameObject StartSet;
    [SerializeField] private TextMeshProUGUI PageText;

    

    public void ShowTutorial()
    {
        currentSceneName = SceneManager.GetActiveScene().name;
        StartSet.SetActive(false);

        if(currentSceneName == "Ingredient") {
            tutoBool = DataManager.Instance.gameData.hasCompletedIngredientTutorial;
        }else if(currentSceneName == "Baking") {
            tutoBool = DataManager.Instance.gameData.hasCompletedBakingTutorial;
        }else if(currentSceneName == "Match") {
            tutoBool = DataManager.Instance.gameData.hasCompletedBonusTutorial;
        }

        if(tutoBool == false) {
            StartSet.SetActive(true);
            InitFunction();
        }else{
            isDone = true;
        }
    }

    private void InitFunction() {
        MaxPage = PageImage.Count();
        ImagePanel.GetComponent<Image>().sprite = PageImage[0];
        PageText.text = $"{CurrentPage + 1} / {MaxPage}";

        Button StartButtonB = StartButton.GetComponent<Button>();
        StartButtonB.interactable = false;
    }

    public void OnNextPageButton() {
        if (CurrentPage <= MaxPage) {
            CurrentPage = CurrentPage + 1;
            if(CurrentPage < MaxPage) {
                ChangePageNum();
                ImagePanel.GetComponent<Image>().sprite = PageImage[CurrentPage];
            }
        }

        if (CurrentPage == MaxPage) {
            Button StartButtonB = StartButton.GetComponent<Button>();
            ChangePageNum();
            ActivateStartButton();
        }

        if(CurrentPage == MaxPage) {
            StartSet.SetActive(false);
            SaveData();

            if(currentSceneName == "Ingredient") {
                ingreGameManager_h.Instance.isDonetuto = true;
                ingreGameManager_h.Instance.StartFunc();
            }else if(currentSceneName == "Baking") {
                //
            }else if(currentSceneName == "Match") {
                MatchGame_h.instance.isDonetuto = true;
                MatchGame_h.instance.StartFunc();
            }else{}
            }
    }

    private void ChangePageNum() {
        PageText.text = $"{CurrentPage + 1} / {MaxPage}";
    }


    public void OnPreviousPageButton() {
        if (CurrentPage > 0) {
            CurrentPage = CurrentPage - 1;
            ChangePageNum();
            ImagePanel.GetComponent<Image>().sprite = PageImage[CurrentPage];
        }
    }

    public void ActivateStartButton() {
        // StartButton.SetActive(true);

        Button StartButtonB = StartButton.GetComponent<Button>();
        StartButtonB.interactable = true;
    }

    private void SaveData() {
        if(currentSceneName == "Ingredient") {
            DataManager.Instance.gameData.hasCompletedIngredientTutorial = true;
        }else if(currentSceneName == "Baking") {
            DataManager.Instance.gameData.hasCompletedBakingTutorial = true;
        }else if(currentSceneName == "Match") {
            DataManager.Instance.gameData.hasCompletedBonusTutorial = true;
        }else{}
        DataManager.Instance.SaveGameData();
    }
}
