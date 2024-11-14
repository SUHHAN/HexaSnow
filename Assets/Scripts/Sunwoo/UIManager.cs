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

    public InventoryManager inventoryManager;
    public GameObject refrigeratorPanel; // ����� �г�
    public GameObject shelfPanel; // ���� �г�
    public GameObject finishIngredientButton; // ��� ���� �Ϸ� ��ư

    public Dictionary<string, Button> recipeButtons = new Dictionary<string, Button>(); // ������ ��ư ���
    private Button currentlySelectedButton = null; // ���� ���õ� ��ư ����
    private List<Button> selectedIngredients = new List<Button>(); // ���õ� ��� ��ư ���

    void Start()
    {
        // RecipeSelectionPopup�� �� ������ ��ư�� Dictionary�� ���
        foreach (Button btn in recipeSelectionPopup.GetComponentsInChildren<Button>())
        {
            recipeButtons.Add(btn.name, btn);
        }

        finishIngredientButton.SetActive(false); // �ʱ⿡�� finishIngredient ��ư ��Ȱ��ȭ
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

    // AddIngredientButton�� Ŭ������ �� ȣ��Ǵ� �޼����, ��� ��ư���� ����
    // ������ ���ݿ� ��� ��ư ����
    public void GenerateIngredientButtons()
    {
        // ����� ��� ���
        string[] fridgeIngredients = { "Butter", "Egg", "Milk", "Egg Whites", "Browned Butter", "Cream Cheese", "Heavy Cream", "Condensed Milk" };
        foreach (string ingredient in fridgeIngredients)
        {
            bool hasIngredient = inventoryManager.HasIngredient(ingredient);
            CreateIngredientButton(ingredient, refrigeratorPanel, hasIngredient);
        }

        // ���� ��� ���
        string[] shelfIngredients = { "Flour", "Sugar", "Baking Powder", "Cocoa Powder", "Almond Powder", "Sugar Powder", "Honey" };
        foreach (string ingredient in shelfIngredients)
        {
            bool hasIngredient = inventoryManager.HasIngredient(ingredient);
            CreateIngredientButton(ingredient, shelfPanel, hasIngredient);
        }
    }

    // ��� ��ư ���� �� Ŭ�� �̺�Ʈ ����
    void CreateIngredientButton(string ingredient, GameObject parentPanel, bool hasIngredient)
    {
        GameObject buttonObj = new GameObject(ingredient);
        Button button = buttonObj.AddComponent<Button>();
        button.transform.SetParent(parentPanel.transform);

        // �ؽ�Ʈ �߰� (��� �̸�)
        Text buttonText = buttonObj.AddComponent<Text>();
        buttonText.text = ingredient;
        buttonText.alignment = TextAnchor.MiddleCenter;

        // ��ư ��Ÿ�� ����
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 30); // ��ư ũ�� ����

        // ���� �� ���� ����
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = hasIngredient ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f);

        // ��ư Ŭ�� �̺�Ʈ �߰�
        button.onClick.AddListener(() => OnIngredientButtonClick(button, hasIngredient));
    }

    // ��� ��ư Ŭ�� �� ȣ��
    public void OnIngredientButtonClick(Button button, bool hasIngredient)
    {
        if (hasIngredient)
        {
            // ���õ� ��� ó��
            if (selectedIngredients.Contains(button))
            {
                // ���õ� ��Ḧ �ٽ� Ŭ���ϸ� ���� ����
                button.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f); // ������� �ʱ�ȭ
                selectedIngredients.Remove(button);
            }
            else
            {
                // ��� ����
                button.GetComponent<Image>().color = Color.green; // ���õ� ����
                selectedIngredients.Add(button);
            }

            // ���õ� ��ᰡ �ϳ� �̻��� ���� finishIngredient ��ư Ȱ��ȭ
            finishIngredientButton.SetActive(selectedIngredients.Count > 0);
        }
        else
        {
            // �������� ���� ��� Ŭ�� �� �޽��� ǥ��
            ShowMessage("���� ����Դϴ�");
        }
    }
}
