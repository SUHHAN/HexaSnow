using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BakingStartManager : MonoBehaviour
{
    public GameObject startPanel; // ���� �г�
    public GameObject recipeSelectionPopup; // ������ ���� �˾�
    public GameObject messagePopup; // �޽��� �˾�
    public GameObject ingredientSelectionPanel; // ��� ���� �г�

    public TextMeshProUGUI messageText; // �޽��� �ؽ�Ʈ

    public Button startButton; // ���� ��ư
    public Button nextButton; // '����' ��ư
    public RecipeBook recipeBook; // ������ ������
    private Recipe selectedRecipe = null; // ���õ� ������

    public ToppingManager toppingManager; // ToppingManager ����

    public List<Button> recipeButtons; // ������ ��ư ����Ʈ (Inspector���� �Ҵ�)
    private Dictionary<Button, Color> originalButtonColors = new Dictionary<Button, Color>(); // ��ư ���� ���� ����

    private Dictionary<string, int> dessertIndexMap = new Dictionary<string, int>()
    {
        { "Madeleine", 1 },
        { "Muffin", 7 },
        { "Cookie", 4 },
        { "Pound Cake", 10 },
        { "Financier", 10 },
        { "Basque Cheesecake", 14 },
        { "Scone", 10 },
        { "Tart", 10 },
        { "Slice Cake", 10 },
        { "Doughnut", 10 }
    };

    private int selectedDessertIndex = 1; // �⺻�� 1

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking); // Baking 1 �� BGM ����
        }

        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

        // �ʱ� UI ����
        startPanel.SetActive(true);
        recipeSelectionPopup.SetActive(true);
        messagePopup.SetActive(false);
        nextButton.gameObject.SetActive(false);
        ingredientSelectionPanel.SetActive(false);

        // ��ư �̺�Ʈ ���
        nextButton.onClick.AddListener(GoToIngredientSelection);

        // ������ ��ư �̺�Ʈ ���
        foreach (Button button in recipeButtons)
        {
            string recipeName = button.name;
            originalButtonColors[button] = button.image.color; // ���� ���� ����
            button.onClick.AddListener(() => SelectRecipe(button, recipeName));
        }

        UpdateRecipeButtons();
    }

    // ������ ��ư Ȱ��ȭ/��Ȱ��ȭ ������Ʈ
    private void UpdateRecipeButtons()
    {
        foreach (Button button in recipeButtons)
        {
            string recipeName = button.name;
            Recipe recipe = recipeBook.GetRecipeByName(recipeName);

            if (recipe != null && recipe.canBake)
            {
                button.interactable = true; // �رݵ� �����Ǹ� Ȱ��ȭ
            }
            else
            {
                button.interactable = true; // ��ư Ŭ�� �����ϰ� ����
            }
        }
    }

    public void OnDessertSelected(string dessertName)
    {
        if (dessertIndexMap.ContainsKey(dessertName))
        {
            selectedDessertIndex = dessertIndexMap[dessertName];
        }
        else
        {
            selectedDessertIndex = 1; // �⺻��
        }

        // ������ ����Ʈ ���� ToppingManager�� ����
        if (toppingManager != null)
        {
            toppingManager.SetSelectedDessert(dessertName, selectedDessertIndex);
        }
    }

    public int GetSelectedDessertIndex()
    {
        return selectedDessertIndex;
    }

    // ������ ���� �� ����
    private void SelectRecipe(Button clickedButton, string recipeName)
    {
        Recipe recipe = recipeBook.GetRecipeByName(recipeName);

        if (recipe != null && recipe.canBake)
        {
            if (selectedRecipe != null && selectedRecipe.recipeName == recipeName)
            {
                // �̹� ������ �����Ǹ� �ٽ� Ŭ���ϸ� ���
                clickedButton.image.color = originalButtonColors[clickedButton]; // ���� ���� ����
                selectedRecipe = null;
                nextButton.gameObject.SetActive(false);
            }
            else
            {
                // ���ο� ������ ����
                DeselectAllRecipeButtons(); // ���� ���� ����
                Color newColor = clickedButton.image.color;
                newColor.a = 0.5f; // �������� 50%�� ����
                clickedButton.image.color = newColor;

                selectedRecipe = recipe;
                Debug.Log($"���õ� ����: {selectedRecipe.recipeName}");

                nextButton.gameObject.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(ShowMessage("�رݵ��� ���� �������Դϴ�!"));
        }
    }

    // ��� ������ ��ư ������ ������� �ǵ���
    private void DeselectAllRecipeButtons()
    {
        foreach (Button button in recipeButtons)
        {
            if (originalButtonColors.ContainsKey(button))
            {
                button.image.color = originalButtonColors[button]; // ���� ���� ����
            }
        }
    }

    // �޽��� �˾��� ���� �ð� ���� ǥ�� �� ����
    private IEnumerator ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        messagePopup.SetActive(false);
    }

    // '����' ��ư Ŭ�� �� ��� ���� �гη� �̵�
    private void GoToIngredientSelection()
    {
        if (selectedRecipe != null)
        {
            startPanel.SetActive(false);
            ingredientSelectionPanel.SetActive(true);
        }
    }

    // ������ ������ ��ȯ ���� �ܰ迡�� ���� ����
    public string GetSelectedDessert()
    {
        return selectedRecipe != null ? selectedRecipe.recipeName : null;
    }

    // ������ ������ ��ȯ ��� �������� ����
    public Recipe GetSelectedRecipe()
    {
        return selectedRecipe;
    }
}
