using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BakingTutorialManager : MonoBehaviour
{
    private int currentPage = 0;
    private int maxPage = 0;
    private bool hasDoneTutorial = false;

    [Header("패널")]
    [SerializeField] private GameObject tutorialPanel; // 튜토리얼 전체 팝업
    [SerializeField] private GameObject startPanel;    // 베이킹 시작 UI

    [Header("튜토리얼 UI 구성")]
    [SerializeField] private Image tutorialImage;      // 페이지 이미지

    [Header("버튼")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("튜토리얼 이미지들")]
    [SerializeField] private Sprite[] tutorialImages;

    void Start()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null)
        {
            Debug.LogError("DataManager 또는 gameData가 null입니다!");
            return;
        }

        hasDoneTutorial = DataManager.Instance.gameData.hasCompletedBakingTutorial;

        if (hasDoneTutorial)
        {
            tutorialPanel.SetActive(false);
            startPanel.SetActive(true);
            return;
        }

        tutorialPanel.SetActive(true);
        startPanel.SetActive(false);
        InitTutorial();
    }

    private void InitTutorial()
    {
        currentPage = 0;
        maxPage = tutorialImages.Length;

        if (maxPage == 0)
        {
            Debug.LogError("튜토리얼 이미지가 비어있습니다!");
            return;
        }

        UpdateTutorialPage();
    }

    public void NextPage()
    {
        if (currentPage < maxPage - 1)
        {
            currentPage++;
            UpdateTutorialPage();
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdateTutorialPage();
        }
    }

    public void EndTutorial()
    {
        DataManager.Instance.gameData.hasCompletedBakingTutorial = true;
        DataManager.Instance.SaveGameData();

        tutorialPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    private void UpdateTutorialPage()
    {
        tutorialImage.sprite = tutorialImages[currentPage];

        previousButton.interactable = (currentPage > 0);
        nextButton.interactable = (currentPage < maxPage - 1);
        startButton.interactable = (currentPage == maxPage - 1);
    }
}
