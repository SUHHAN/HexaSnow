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
    public GameObject startToppingPanel; // ���� ���� �г�
    public GameObject addToppingPanel; // ���� ���� �г�
    public GameObject finishBakingPanel; // ����ŷ �Ϸ� �г�

    // ��ư
    public Button startToppingButton; // ���� ���� ��ư
    public Button finishToppingButton; // ���� �Ϸ� ��ư

    // ��Ÿ ����
    public InventoryManager inventoryManager; // �κ��丮 �Ŵ���
    public OvenGameManager ovenGameManager; // ���� ���� �Ŵ���
    public Image bakingImage; // ���� ����ŷ �̹���
    public Image oriImage1; // ���� �̹��� 1
    public Image oriImage2; // ���� �̹��� 2

    public BakingStartManager bakingStartManager; // ����ŷ ���� �Ŵ���

    private string selectedTopping = null; // ���õ� ����
    private Dictionary<int, string> menuDictionary = new Dictionary<int, string>(); // �޴� �����͸� �����ϴ� ��ųʸ�
    private Dictionary<string, int> priceTable = new Dictionary<string, int>() // ����Ʈ ���� ����
    {
        { "���鷻", 2000 }, { "����", 2500 }, { "��Ű", 1700 }, { "�Ŀ������ũ", 3000 },
        { "����ũ", 5000 }, { "����", 4000 }, { "�ٽ�ũ ġ������ũ", 5000 }, { "�ֳ��ÿ�", 2200 },
        { "����", 2500 }, { "Ÿ��Ʈ", 4000 }, { "��ī��", 2300 }
    };

    void Start()
    {
        LoadRecipeCSV(); // ������ �����͸� CSV���� �ҷ�����

        startToppingPanel.SetActive(true); // ���� ���� �г� Ȱ��ȭ
        addToppingPanel.SetActive(false); // ���� ���� �г� ��Ȱ��ȭ
        finishBakingPanel.SetActive(false); // ����ŷ �Ϸ� �г� ��Ȱ��ȭ

        startToppingButton.onClick.AddListener(OpenToppingSelection); // ���� ���� ��ư �̺�Ʈ ���
        finishToppingButton.onClick.AddListener(FinishToppingSelection); // ���� �Ϸ� ��ư �̺�Ʈ ���

        UpdateToppingButtons(); // ���� ��ư ������Ʈ
        SetOriginalImages(); // ������ ����Ʈ�� ���� �̹��� ����
    }

    // CSV���� ������ �����͸� �ҷ����� �Լ�
    private void LoadRecipeCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("recipe"); // Resources �������� "recipe.csv" �ҷ�����
        if (csvFile == null)
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: recipe.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n'); // CSV ������ �� ������ ������
        for (int i = 1; i < lines.Length; i++) // ù ��° ��(���) �����ϰ� �б�
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue; // ������ �����ϸ� ����

            int id;
            if (int.TryParse(fields[0].Trim(), out id)) // ù ��° ���� �޴� ID
            {
                string menuName = fields[1].Trim(); // �� ��° ���� �޴� �̸�
                menuDictionary[id] = menuName; // ��ųʸ��� ����
            }
        }
        Debug.Log($"CSV���� {menuDictionary.Count}���� �޴� �����͸� �ҷ��Խ��ϴ�.");
    }

    // ���� ���� �г� ����
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        addToppingPanel.SetActive(true); // ���� ���� �г� Ȱ��ȭ
        UpdateToppingButtons(); // ���� ��ư ���� ������Ʈ
    }

    // ������ ���� ��ư Ȱ��ȭ
    private void UpdateToppingButtons()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        inventoryManager.UpdateRefrigeratorButtons(); // �κ��丮���� ��� ������Ʈ

        foreach (GameObject buttonObj in inventoryManager.refrigeratorButtons)
        {
            string toppingName = buttonObj.name.Replace("Button", ""); // ��ư �̸����� "Button" �����Ͽ� ���� �̸� ����
            bool hasTopping = inventoryManager.HasIngredient(toppingName); // �ش� ������ �ִ��� Ȯ��

            buttonObj.SetActive(hasTopping); // ������ ���θ� ��ư Ȱ��ȭ
            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingName)); // Ŭ�� �̺�Ʈ ���
            }
        }
    }

    // ���� �ϳ��� ���� �����ϵ��� ����
    private void SelectSingleTopping(GameObject buttonObj, string toppingName)
    {
        if (selectedTopping != null) // ������ ������ ������ �ִٸ� ���� ����
        {
            foreach (GameObject btn in inventoryManager.refrigeratorButtons)
            {
                if (btn.name.Replace("Button", "") == selectedTopping)
                {
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f); // ���� ������ ���·� ����
                    break;
                }
            }
        }

        if (selectedTopping == toppingName) // ���� ��ư�� �ٽ� ������ ���� ����
        {
            selectedTopping = null;
        }
        else // ���ο� ���� ����
        {
            selectedTopping = toppingName;
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f); // ���� ����
        }
    }

    // ���� ���� �Ϸ�
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false); // ���� ���� �г� ��Ȱ��ȭ
        finishBakingPanel.SetActive(true); // ����ŷ �Ϸ� �г� Ȱ��ȭ
        UpdateBakingImage(); // ���� �̹��� ������Ʈ
        SaveBakingResult(); // ��� ����

        SceneManager.LoadScene("Bonus"); // ���ʽ� ������ �̵�
    }

    // ������ ����Ʈ�� ���� �̹��� ����
    private void SetOriginalImages()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string originalImagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_original";

        Sprite originalSprite = Resources.Load<Sprite>(originalImagePath);
        if (originalSprite != null)
        {
            oriImage1.sprite = originalSprite;
            oriImage2.sprite = originalSprite;
        }
    }

    // ������ ����Ʈ�� ������ �ݿ��Ͽ� ���� �̹��� ������Ʈ
    private void UpdateBakingImage()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(selectedTopping ?? "original").ToLower()}";

        Sprite newSprite = Resources.Load<Sprite>(imagePath);
        if (newSprite != null)
        {
            bakingImage.sprite = newSprite;
        }
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
        int totalScore = ovenGameManager.GetTotalScore(); // ���� ���� ��������
        Debug.Log($"���� ����: {totalScore}");

        bool isBonusGame = false; // ���ʽ� ���δ� ���ʽ� ������ ������

        int menuID = GetMenuID(dessertName);
        if (menuID == -1)
        {
            Debug.LogError($"�޴� ID�� ã�� �� �����ϴ�: {dessertName}");
            return;
        }

        string finalName = selectedTopping != null ? $"{selectedTopping} {dessertName}" : $"�������� {dessertName}";

        MyRecipeList newRecipe = new MyRecipeList(
            DataManager.Instance.gameData.myBake.Count + 1,
            menuID,
            finalName,
            totalScore,
            isBonusGame
        );

        DataManager.Instance.gameData.myBake.Add(newRecipe);
        DataManager.Instance.SaveGameData();

        Debug.Log($"���� �Ϸ�: {finalName} | ����: {totalScore} | ���ʽ� ����: {isBonusGame}");
    }

    // ����Ʈ �̸����� �޴� ID ��������
    private int GetMenuID(string dessertName)
    {
        foreach (var entry in menuDictionary)
        {
            if (entry.Value == dessertName)
            {
                return entry.Key;
            }
        }
        return -1;
    }
}
