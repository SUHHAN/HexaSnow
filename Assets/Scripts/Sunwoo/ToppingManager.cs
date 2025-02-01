using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

public class ToppingManager : MonoBehaviour
{
    // ���� �г�
    public GameObject toppingPanel;
    public GameObject startToppingPanel;
    public GameObject addToppingPanel;
    public GameObject finishBakingPanel;

    // ��ư
    public Button startToppingButton;
    public Button finishToppingButton;
    public Button finishButton;

    // ��Ÿ ����
    public InventoryManager inventoryManager;
    public OvenGameManager ovenGameManager;
    public Image bakingImage;
    public Image oriImage1;
    public Image oriImage2;

    private int selectedDessertIndex = 1; // �⺻�� 1
    private int selectedToppingIndex = -1; // ������ ���� �ε��� (-1�� ���� �� ��)
    private int finalImageIndex = 1; // ���� ��� �̹��� �ε���
    private string selectedDessert;

    public BakingStartManager bakingStartManager;

    // ���� ��ư ����Ʈ (Inspector���� �Ҵ�)
    public List<GameObject> toppingButtons;

    // ����Ʈ �ε����� �´� �̹��� ����
    public List<Sprite> dessertSprites;

    // ����Ʈ �� Ȱ��ȭ�� ���� ��ư
    private Dictionary<int, List<int>> dessertToppingMap = new Dictionary<int, List<int>>()
    {
        { 1, new List<int> { 4, 5 } }, // Madeleine: ���� 4, 5�� Ȱ��ȭ
        { 4, new List<int> { 4, 6 } }, // Cookie: ���� 4, 6�� Ȱ��ȭ
        { 7, new List<int> { 2, 4 } }, // Muffin: ���� 2, 4�� Ȱ��ȭ
        { 10, new List<int> { 1, 4, 5 } }, // �Ŀ������ũ (���� 1, 4, 5 Ȱ��ȭ)
        { 14, new List<int>() }        // �ٽ�ũ ġ������ũ
    };

    // ���� ���ÿ� ���� BakingImage ���� (����Ʈ �ε���, ���� �ε��� �� ��� �̹��� �ε���)
    private Dictionary<(int, int), int> toppingImageMap = new Dictionary<(int, int), int>()
    {
        { (1, 4), 2 }, { (1, 5), 3 }, // Madeleine �� 2: ����4, 3: ����5
        { (4, 4), 5 }, { (4, 6), 6 }, // Cookie �� 5: ����4, 6: ����6
        { (7, 2), 8 }, { (7, 4), 9 }, // Muffin �� 8: ����2, 9: ����4
        { (10, 4), 11 }, { (10, 5), 12 }, { (10, 1), 13 } // �Ŀ������ũ
    };

    void Start()
    {
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);
        finishButton.onClick.AddListener(FinishBaking);

        UpdateToppingButtons();
        SetOriginalImages();
    }

    public void SetSelectedDessert(string dessertName, int index)
    {
        selectedDessert = dessertName;
        selectedDessertIndex = index;
        selectedToppingIndex = -1; // ���� ���� �ʱ�ȭ
        finalImageIndex = index; // �⺻������ ���� �̹��� ����
        SetOriginalImages();
        UpdateToppingButtons();
    }

    private void SetOriginalImages()
    {
        if (selectedDessertIndex < dessertSprites.Count)
        {
            oriImage1.sprite = dessertSprites[selectedDessertIndex];
            oriImage2.sprite = dessertSprites[selectedDessertIndex];
            bakingImage.sprite = dessertSprites[selectedDessertIndex]; // �⺻ �̹��� ����
        }
        else
        {
            Debug.LogError($"SetOriginalImages: �ε��� {selectedDessertIndex}�� �ش��ϴ� �̹����� �����ϴ�.");
        }
    }

    // ������ ���� ��ư Ȱ��ȭ
    private void UpdateToppingButtons()
    {
        foreach (GameObject button in toppingButtons)
        {
            button.SetActive(false); // ��� ��ư ��Ȱ��ȭ
            SetButtonOpacity(button, 1f); // ��� ��ư ���� �ʱ�ȭ
        }

        if (dessertToppingMap.ContainsKey(selectedDessertIndex))
        {
            foreach (int toppingIndex in dessertToppingMap[selectedDessertIndex])
            {
                if (toppingIndex - 1 < toppingButtons.Count)
                {
                    toppingButtons[toppingIndex - 1].SetActive(true);
                    int index = toppingIndex; // Ŭ���� ���� ����
                    toppingButtons[toppingIndex - 1].GetComponent<Button>().onClick.RemoveAllListeners();
                    toppingButtons[toppingIndex - 1].GetComponent<Button>().onClick.AddListener(() => SelectSingleTopping(index));
                }
            }
        }
    }

    // ���� �ϳ��� ���� �����ϵ��� ����
    private void SelectSingleTopping(int toppingIndex)
    {
        if (selectedToppingIndex == toppingIndex)
        {
            selectedToppingIndex = -1; // ���� ���� �ٽ� ������ ���� ����
            finalImageIndex = selectedDessertIndex; // ���� ����Ʈ �̹����� �ǵ���
        }
        else
        {
            selectedToppingIndex = toppingIndex;
            if (toppingImageMap.ContainsKey((selectedDessertIndex, toppingIndex)))
            {
                finalImageIndex = toppingImageMap[(selectedDessertIndex, toppingIndex)];
            }
        }

        UpdateBakingImage();
        UpdateToppingButtonOpacity();
    }

    // ������ ����Ʈ�� ������ �ݿ��Ͽ� ���� �̹��� ������Ʈ
    private void UpdateBakingImage()
    {
        int newImageIndex = selectedDessertIndex;

        if (toppingImageMap.ContainsKey((selectedDessertIndex, selectedToppingIndex)))
        {
            newImageIndex = toppingImageMap[(selectedDessertIndex, selectedToppingIndex)];
        }

        if (newImageIndex < dessertSprites.Count)
        {
            bakingImage.sprite = dessertSprites[newImageIndex];
        }
        else
        {
            Debug.LogError($"UpdateBakingImage: �ε��� {newImageIndex}�� �ش��ϴ� �̹����� �����ϴ�.");
        }
    }

    // ������ ��ư�� ����������
    private void UpdateToppingButtonOpacity()
    {
        foreach (GameObject button in toppingButtons)
        {
            SetButtonOpacity(button, 1f); // �⺻�� 1 (������)
        }

        if (selectedToppingIndex != -1 && selectedToppingIndex - 1 < toppingButtons.Count)
        {
            SetButtonOpacity(toppingButtons[selectedToppingIndex - 1], 0.3f); // ������ ��ư ���� 30%
        }
    }

    // ���� ����
    private void SetButtonOpacity(GameObject buttonObj, float alpha)
    {
        Image buttonImage = buttonObj.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color color = buttonImage.color;
            color.a = alpha;
            buttonImage.color = color;
        }
    }

    // ���� ���� �Ϸ�
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    // ���� ���� �г� ����
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons();
    }

    private void FinishBaking()
    {
        SaveBakingResult();
        SceneManager.LoadScene("Bonus");
    }

    // ����ŷ ��� ����
    private void SaveBakingResult()
    {
        if (DataManager.Instance == null)
        {
            Debug.LogError("DataManager �ν��Ͻ��� ã�� �� �����ϴ�!");
            return;
        }

        string dessertName = bakingStartManager.GetSelectedDessert();
        int totalScore = ovenGameManager.GetTotalScore();
        Debug.Log($"���� ����: {totalScore}");

        MyRecipeList newRecipe = new MyRecipeList(
            DataManager.Instance.gameData.myBake.Count + 1,
            finalImageIndex, // ����Ǵ� ���� �̹��� �ε���
            dessertName,
            totalScore,
            false
        );

        DataManager.Instance.gameData.myBake.Add(newRecipe);
        DataManager.Instance.SaveGameData();

        Debug.Log($"���� �Ϸ�: {dessertName} | �̹��� �ε���: {finalImageIndex} | ����: {totalScore}");
    }
}
