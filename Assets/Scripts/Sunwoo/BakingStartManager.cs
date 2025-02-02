using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BakingStartManager : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject recipeSelectionPopup;
    public GameObject messagePopup;
    public GameObject BlackPanel;
    public GameObject ingredientSelectionPanel;

    public TextMeshProUGUI messageText;

    public Button nextButton;
    public RecipeBook recipeBook;
    private Recipe selectedRecipe = null;

    public ToppingManager toppingManager;

    public List<Button> recipeButtons;
    private Dictionary<Button, Color> originalButtonColors = new Dictionary<Button, Color>();

    private int selectedDessertIndex = 10; // 기본값 10
    private string selectedDessert = "";
    private Button lastSelectedButton = null; // 마지막으로 선택한 버튼을 저장

    private Dictionary<int, int> buttonIndexToDessertIndex = new Dictionary<int, int>()
    {
        { 0, 1 },  // 버튼 0 → 마들렌 (1)
        { 2, 4 },  // 버튼 2 → 머핀 (7)
        { 3, 7 },  // 버튼 3 → 쿠키 (4)
        { 4, 10 }, // 버튼 4 → 파운드케이크 (10)
        { 6, 14 }  // 버튼 6 → 바스크 치즈케이크 (14)
        // 나머지는 기본값 10 (파운드케이크)
    };

    public UiLogicManager uiLogicManager;

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking);
        }

        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

        startPanel.SetActive(true);
        recipeSelectionPopup.SetActive(true);
        messagePopup.SetActive(false);
        nextButton.gameObject.SetActive(false);
        ingredientSelectionPanel.SetActive(false);
        BlackPanel.SetActive(false);

        nextButton.onClick.AddListener(GoToIngredientSelection);

        for (int i = 0; i < recipeButtons.Count; i++)
        {
            int buttonIndex = i; // 로컬 변수로 캡처
            Button button = recipeButtons[i];
            originalButtonColors[button] = button.image.color;
            button.onClick.AddListener(() => SelectRecipe(button, buttonIndex));
        }

        UpdateRecipeButtons();
        // UiLogicManager.Instance.LoadMoneyData();
        uiLogicManager.LoadMoneyData();
    }

    private void UpdateRecipeButtons()
    {
        foreach (Button button in recipeButtons)
        {
            button.interactable = true;
        }
    }

    private void SelectRecipe(Button clickedButton, int buttonIndex)
    {
        string recipeName = clickedButton.name; // 버튼 이름을 기반으로 레시피 찾기
        Recipe recipe = recipeBook.GetRecipeByName(recipeName); // RecipeBook에서 레시피 찾기

        if (recipe != null && recipe.canBake)
        {
            if (selectedRecipe != null && selectedRecipe.recipeName == recipe.recipeName)
            {
                // 이미 선택된 버튼을 다시 클릭하면 취소
                ResetButtonColor(clickedButton);
                selectedRecipe = null;
                selectedDessertIndex = 10;  // 기본값으로 리셋
                selectedDessert = "";
                nextButton.gameObject.SetActive(false);
                lastSelectedButton = null;
            }
            else
            {
                // 이전에 선택한 버튼이 있으면 색상 복원
                if (lastSelectedButton != null)
                {
                    ResetButtonColor(lastSelectedButton);
                }

                // 새로운 레시피 선택
                clickedButton.image.color = new Color(clickedButton.image.color.r, clickedButton.image.color.g, clickedButton.image.color.b, 0.5f); // 투명도 50%
                selectedRecipe = recipe;
                selectedDessert = recipe.recipeName;
                selectedDessertIndex = buttonIndexToDessertIndex.ContainsKey(buttonIndex) ? buttonIndexToDessertIndex[buttonIndex] : 10;
                lastSelectedButton = clickedButton;

                Debug.Log($"선택된 제과: {selectedDessert}, 인덱스: {selectedDessertIndex}");

                nextButton.gameObject.SetActive(true);
            }
        }
        else
        {
            // 이전에 선택한 버튼이 있으면 색상 복원
            if (lastSelectedButton != null)
            {
                ResetButtonColor(lastSelectedButton);
                selectedRecipe = null;
                selectedDessertIndex = 10;  // 기본값으로 리셋
                selectedDessert = "";
                nextButton.gameObject.SetActive(false);
            }
            StartCoroutine(ShowMessage("해금되지 않은\n레시피입니다."));
        }
    }

    // 버튼 색상 복원
    private void ResetButtonColor(Button button)
    {
        if (button != null && originalButtonColors.ContainsKey(button))
        {
            button.image.color = originalButtonColors[button]; // 원래 색상 복원
        }
    }

    private IEnumerator ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        BlackPanel.SetActive(true);
        yield return new WaitForSeconds(1f);
        messagePopup.SetActive(false);
        BlackPanel.SetActive(false);
    }

    private void GoToIngredientSelection()
    {
        if (selectedRecipe != null)
        {
            startPanel.SetActive(false);
            ingredientSelectionPanel.SetActive(true);

            if (toppingManager != null)
            {
                toppingManager.SetSelectedDessert(selectedDessert, selectedDessertIndex);
            }
        }
    }

    public string GetSelectedDessert()
    {
        return selectedDessert;
    }

    public int GetSelectedDessertIndex()
    {
        return selectedDessertIndex;
    }

    public Recipe GetSelectedRecipe()
    {
        return selectedRecipe;
    }
}
