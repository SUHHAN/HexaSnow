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

    public BakingStartManager bakingStartManager;

    private int selectedToppingIndex = -1; // int Ÿ������ �����Ͽ� �ε����� ����
    private Dictionary<int, string> menuDictionary = new Dictionary<int, string>();

    void Start()
    {
        LoadRecipeCSV();

        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);
        finishButton.onClick.AddListener(FinishBaking);

        UpdateToppingButtons();
        SetOriginalImages();
    }

    // CSV���� ������ �����͸� �ҷ����� �Լ�
    private void LoadRecipeCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("recipe");
        if (csvFile == null)
        {
            Debug.LogError("CSV ������ ã�� �� �����ϴ�: recipe.csv");
            return;
        }

        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] fields = lines[i].Split(',');
            if (fields.Length < 2) continue;

            int id;
            if (int.TryParse(fields[0].Trim(), out id))
            {
                string menuName = fields[1].Trim();
                menuDictionary[id] = menuName;
            }
        }
        Debug.Log($"CSV���� {menuDictionary.Count}���� �޴� �����͸� �ҷ��Խ��ϴ�.");
    }

    // ���� ���� �г� ����
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons();
    }

    // ������ ���� ��ư Ȱ��ȭ
    private void UpdateToppingButtons()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager�� �Ҵ���� �ʾҽ��ϴ�!");
            return;
        }

        inventoryManager.UpdateRefrigeratorButtons();

        foreach (GameObject buttonObj in inventoryManager.refrigeratorButtons)
        {
            string toppingEname = buttonObj.name.Replace("Button", ""); // ��ư���� "Button" �����Ͽ� eName ����
            int toppingIndex = inventoryManager.GetIngredientIndexFromEname(toppingEname); // eName �� index ��ȯ

            if (toppingIndex == -1) continue; // ���� ���� ����

            bool hasTopping = inventoryManager.HasIngredient(toppingIndex);

            buttonObj.SetActive(hasTopping);
            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingIndex)); // int Ÿ������ ����
            }
        }
    }

    // ���� �ϳ��� ���� �����ϵ��� ����
    private void SelectSingleTopping(GameObject buttonObj, int toppingIndex)
    {
        if (selectedToppingIndex != -1) // ���� ���õ� ������ ������ ����
        {
            foreach (GameObject btn in inventoryManager.refrigeratorButtons)
            {
                string btnEname = btn.name.Replace("Button", "");
                int btnIndex = inventoryManager.GetIngredientIndexFromEname(btnEname);
                if (btnIndex == selectedToppingIndex)
                {
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f);
                    break;
                }
            }
        }

        if (selectedToppingIndex == toppingIndex) // ���� ��ư �ٽ� ������ ���� ����
        {
            selectedToppingIndex = -1;
        }
        else // ���ο� ���� ����
        {
            selectedToppingIndex = toppingIndex;
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    // ���� ���� �Ϸ�
    private void FinishToppingSelection()
    {
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    private void FinishBaking()
    {
        SaveBakingResult();

        SceneManager.LoadScene("Bonus");
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
        string toppingEname = inventoryManager.GetIngredientEname(selectedToppingIndex); // �ε����� eName���� ��ȯ
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(toppingEname ?? "original").ToLower()}";

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
        int totalScore = ovenGameManager.GetTotalScore();
        Debug.Log($"���� ����: {totalScore}");

        bool isBonusGame = false;

        int menuID = GetMenuID(dessertName);
        if (menuID == -1)
        {
            Debug.LogError($"�޴� ID�� ã�� �� �����ϴ�: {dessertName}");
            return;
        }

        string finalName = selectedToppingIndex != -1
            ? $"{inventoryManager.GetIngredientEname(selectedToppingIndex)} {dessertName}"
            : $"�������� {dessertName}";

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

    // �޴� ID�� �������� �Լ� �߰�
    private int GetMenuID(string dessertName)
    {
        foreach (var entry in menuDictionary)
        {
            if (entry.Value == dessertName)
            {
                return entry.Key; // �ش��ϴ� �޴� ID ��ȯ
            }
        }
        Debug.LogError($"GetMenuID: '{dessertName}'�� �ش��ϴ� �޴� ID�� ã�� �� �����ϴ�.");
        return -1; // ã�� ���� ��� -1 ��ȯ
    }

}
