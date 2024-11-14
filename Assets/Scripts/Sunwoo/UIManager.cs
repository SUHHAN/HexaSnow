using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // �޽��� �˾�
    public TextMeshProUGUI messageText; // �޽��� �ؽ�Ʈ
    public GameObject recipeSelectionPopup; // ���� ���� ���� �˾� �г�
    public GameObject addingIngredientPanel; // ��� ��� �г�

    public InventoryManager inventoryManager; // �κ��丮 �Ŵ��� ����
    public GameObject refrigeratorPanel; // ����� �г�
    public GameObject shelfPanel; // ���� �г�
    public GameObject finishIngredientButton; // ��� ���� �Ϸ� ��ư

    public Dictionary<string, Button> recipeButtons = new Dictionary<string, Button>(); // ������ ��ư ���
    private Button currentlySelectedButton = null; // ���� ���õ� ��ư ����
    private List<Button> selectedIngredients = new List<Button>(); // ���õ� ��� ��ư ���

    private float buttonSpacing = 110f; // ��ư ����
    private Vector2 buttonSize = new Vector2(100, 30); // ��ư ũ��

    void Start()
    {
        // RecipeSelectionPopup�� �� ������ ��ư�� Dictionary�� ���
        foreach (Button btn in recipeSelectionPopup.GetComponentsInChildren<Button>())
        {
            recipeButtons.Add(btn.name, btn);
        }

        finishIngredientButton.SetActive(false); // �ʱ⿡�� finishIngredient ��ư ��Ȱ��ȭ
        GenerateIngredientButtons(); // ������ ���ݿ� ��� ��ư ����
    }

    // ���� ���õ� ������ ��ư ��ȯ
    public Button GetCurrentlySelectedButton()
    {
        return currentlySelectedButton;
    }

    // �޽����� ���� �ð� ���� ǥ���ϰ� �ڵ����� ����� �޼���
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(1f)); // 1�� �� �޽��� ����
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messagePopup.SetActive(false);
    }

    // ���õ� ������ ��ư ���� ǥ�� (��� ���)
    public void HighlightRecipeButton(string recipeName)
    {
        // ���� ���õ� ��ư�� Ŭ���� ��� ���� ����
        if (currentlySelectedButton != null && currentlySelectedButton.name == recipeName)
        {
            currentlySelectedButton.GetComponent<Image>().color = Color.white; // �⺻ �������� �ǵ���
            currentlySelectedButton = null; // ���� ����
        }
        else
        {
            // ���� ���õ� ��ư�� ���� ��� ���� �ʱ�ȭ
            if (currentlySelectedButton != null)
            {
                currentlySelectedButton.GetComponent<Image>().color = Color.white;
            }

            // ���� ���õ� ��ư�� �ʷϻ����� ���� ǥ��
            if (recipeButtons.TryGetValue(recipeName, out Button selectedButton))
            {
                selectedButton.GetComponent<Image>().color = Color.green;
                currentlySelectedButton = selectedButton; // ���� ���õ� ��ư ������Ʈ
            }
        }
    }

    // ��� ��ư �ʱ�ȭ
    public void InitializeIngredientButtons()
    {
        // AddingIngredient �г� ���� ��� ��ư �ʱ�ȭ
        foreach (Button button in addingIngredientPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = Color.white; // ��ư ������ �⺻������ ����
            button.onClick.AddListener(() => OnIngredientButtonClick(button)); // Ŭ�� �̺�Ʈ ����
        }
        selectedIngredients.Clear(); // ���õ� ��� ��� �ʱ�ȭ
        finishIngredientButton.SetActive(false); // 'finishIngredient' ��ư ��Ȱ��ȭ
    }

    // ��� ��ư Ŭ�� �� ȣ��
    public void OnIngredientButtonClick(Button button)
    {
        if (selectedIngredients.Contains(button))
        {
            // ���õ� ��Ḧ �ٽ� Ŭ���ϸ� ���� ����
            button.GetComponent<Image>().color = Color.white;
            selectedIngredients.Remove(button);
        }
        else
        {
            // ��� ����
            button.GetComponent<Image>().color = Color.green;
            selectedIngredients.Add(button);
        }

        // ���õ� ��ᰡ �ϳ� �̻� ���� ���� finishIngredient ��ư Ȱ��ȭ
        finishIngredientButton.SetActive(selectedIngredients.Count > 0);
    }

    // ���õ� ��� �̸� ��� ��ȯ
    public List<string> GetSelectedIngredientNames()
    {
        List<string> ingredientNames = new List<string>();
        foreach (Button button in selectedIngredients)
        {
            ingredientNames.Add(button.name);
        }
        return ingredientNames;
    }

    // Ư�� �г��� Ȱ��ȭ ���¸� �����ϴ� �޼���
    public void TogglePanel(GameObject panel, bool isActive)
    {
        panel.SetActive(isActive);
    }

    // ������ ��Ḹ ������ ���ݿ� ��ư���� ����
    public void GenerateIngredientButtons()
    {
        // ����� �г� �� ��ư ��ġ ����
        Vector2 startPosition = new Vector2(0, 0);
        int index = 0;

        // ����� ��� ���
        string[] fridgeIngredients = { "Butter", "Egg", "Milk", "Egg Whites", "Browned Butter", "Cream Cheese", "Heavy Cream", "Condensed Milk" };
        foreach (string ingredient in fridgeIngredients)
        {
            if (inventoryManager.HasIngredient(ingredient))
            {
                CreateIngredientButton(ingredient, refrigeratorPanel, startPosition + new Vector2((index % 4) * buttonSpacing, -(index / 4) * buttonSpacing));
                index++;
            }
        }

        // ���� �г� �� ��ư ��ġ ����
        startPosition = new Vector2(0, 0);
        index = 0;

        // ���� ��� ���
        string[] shelfIngredients = { "Flour", "Sugar", "Baking Powder", "Cocoa Powder", "Almond Powder", "Sugar Powder", "Honey" };
        foreach (string ingredient in shelfIngredients)
        {
            if (inventoryManager.HasIngredient(ingredient))
            {
                CreateIngredientButton(ingredient, shelfPanel, startPosition + new Vector2((index % 4) * buttonSpacing, -(index / 4) * buttonSpacing));
                index++;
            }
        }
    }

    // ��� ��ư ���� �� Ŭ�� �̺�Ʈ ����
    void CreateIngredientButton(string ingredient, GameObject parentPanel, Vector2 position)
    {
        // UI ��ư�� ����
        GameObject buttonObj = new GameObject(ingredient, typeof(RectTransform), typeof(CanvasRenderer), typeof(Button), typeof(Image));
        buttonObj.transform.SetParent(parentPanel.transform, false);

        // RectTransform �� ��ġ ����
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = buttonSize;

        // Image ������Ʈ ����
        Image buttonImage = buttonObj.GetComponent<Image>();
        buttonImage.color = Color.white; // ��ư �⺻ ���� ���� (���)

        // �ؽ�Ʈ �߰� (��� �̸�)
        GameObject textObj = new GameObject("Text");
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = ingredient;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontSize = 18;
        buttonText.color = Color.black; // �ؽ�Ʈ�� ���������� ����
        textObj.transform.SetParent(buttonObj.transform, false);

        // �ؽ�Ʈ ��ġ�� ũ�� ����
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = buttonSize;
        textRect.anchoredPosition = Vector2.zero;

        // ��ư Ŭ�� �̺�Ʈ �߰�
        buttonObj.GetComponent<Button>().onClick.AddListener(() => OnIngredientButtonClick(buttonObj.GetComponent<Button>()));
    }
}
