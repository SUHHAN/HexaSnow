using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UitutorialManager : MonoBehaviour
{
    private int currentPage = 0;
    private int maxPage = 0;
    private bool hasDoneTutorial = false;

    [Header("�г�")]
    [SerializeField] private GameObject tutorialPanel; // Ʃ�丮�� ��ü �˾�


    [Header("Ʃ�丮�� UI ����")]
    [SerializeField] private Image tutorialImage;      // ������ �̹���

    [Header("��ư")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    [Header("Ʃ�丮�� �̹�����")]
    [SerializeField] private Sprite[] tutorialImages;

    void Start()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null)
        {
            Debug.LogError("DataManager �Ǵ� gameData�� null�Դϴ�!");
            return;
        }

        hasDoneTutorial = DataManager.Instance.gameData.hasCompletedBakingTutorial;

        if (hasDoneTutorial)
        {
            SceneManager.LoadScene("order1");
            return;
        }

        tutorialPanel.SetActive(true);
        InitTutorial();
    }

    private void InitTutorial()
    {
        currentPage = 0;
        maxPage = tutorialImages.Length;

        if (maxPage == 0)
        {
            Debug.LogError("Ʃ�丮�� �̹����� ����ֽ��ϴ�!");
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

        SceneManager.LoadScene("order1");
    }

    private void UpdateTutorialPage()
    {
        tutorialImage.sprite = tutorialImages[currentPage];

        previousButton.interactable = (currentPage > 0);
        nextButton.interactable = (currentPage < maxPage - 1);
        startButton.interactable = (currentPage == maxPage - 1);
    }
}
