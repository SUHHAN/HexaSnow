using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BakingStartManager : MonoBehaviour
{
    public GameObject startPanel; // 시작 패널
    public GameObject recipeSelectionPopup; // 레시피 선택 팝업
    public GameObject messagePopup; // 메시지 팝업
    public GameObject ingredientSelectionPanel; // 재료 선택 패널

    public TextMeshProUGUI messageText; // 메시지 텍스트

    public Button startButton; // 시작 버튼
    public Button nextButton; // '다음' 버튼
    public RecipeBook recipeBook; // 레시피 데이터
    private Recipe selectedRecipe = null; // 선택된 레시피

    public ToppingManager toppingManager; // ToppingManager 참조

    public List<Button> recipeButtons; // 레시피 버튼 리스트 (Inspector에서 할당)
    private Dictionary<Button, Color> originalButtonColors = new Dictionary<Button, Color>(); // 버튼 원래 색상 저장

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

    private int selectedDessertIndex = 1; // 기본값 1

    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBgm(AudioManager.Bgm.inside_kitchen_baking); // Baking 1 씬 BGM 실행
        }

        // 초기 UI 설정
        startPanel.SetActive(true);
        recipeSelectionPopup.SetActive(false);
        messagePopup.SetActive(false);
        nextButton.gameObject.SetActive(false);
        ingredientSelectionPanel.SetActive(false);

        // 버튼 이벤트 등록
        nextButton.onClick.AddListener(GoToIngredientSelection);

        // 레시피 버튼 이벤트 등록
        foreach (Button button in recipeButtons)
        {
            string recipeName = button.name;
            originalButtonColors[button] = button.image.color; // 원래 색상 저장
            button.onClick.AddListener(() => SelectRecipe(button, recipeName));
        }

        UpdateRecipeButtons();
    }

    // 레시피 버튼 활성화/비활성화 업데이트
    private void UpdateRecipeButtons()
    {
        foreach (Button button in recipeButtons)
        {
            string recipeName = button.name;
            Recipe recipe = recipeBook.GetRecipeByName(recipeName);

            if (recipe != null && recipe.canBake)
            {
                button.interactable = true; // 해금된 레시피만 활성화
            }
            else
            {
                button.interactable = true; // 버튼 클릭 가능하게 유지
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
            selectedDessertIndex = 1; // 기본값
        }

        // 선택한 디저트 정보 ToppingManager로 전달
        if (toppingManager != null)
        {
            toppingManager.SetSelectedDessert(dessertName, selectedDessertIndex);
        }
    }

    public int GetSelectedDessertIndex()
    {
        return selectedDessertIndex;
    }

    // 레시피 선택 시 실행
    private void SelectRecipe(Button clickedButton, string recipeName)
    {
        Recipe recipe = recipeBook.GetRecipeByName(recipeName);

        if (recipe != null && recipe.canBake)
        {
            if (selectedRecipe != null && selectedRecipe.recipeName == recipeName)
            {
                // 이미 선택한 레시피를 다시 클릭하면 취소
                clickedButton.image.color = originalButtonColors[clickedButton]; // 원래 색상 복원
                selectedRecipe = null;
                nextButton.gameObject.SetActive(false);
            }
            else
            {
                // 새로운 레시피 선택
                DeselectAllRecipeButtons(); // 이전 선택 해제
                Color newColor = clickedButton.image.color;
                newColor.a = 0.5f; // 투명도를 50%로 낮춤
                clickedButton.image.color = newColor;

                selectedRecipe = recipe;
                Debug.Log($"선택된 제과: {selectedRecipe.recipeName}");

                nextButton.gameObject.SetActive(true);
            }
        }
        else
        {
            StartCoroutine(ShowMessage("해금되지 않은 레시피입니다!"));
        }
    }

    // 모든 레시피 버튼 색상을 원래대로 되돌림
    private void DeselectAllRecipeButtons()
    {
        foreach (Button button in recipeButtons)
        {
            if (originalButtonColors.ContainsKey(button))
            {
                button.image.color = originalButtonColors[button]; // 원래 색상 복원
            }
        }
    }

    // 메시지 팝업을 일정 시간 동안 표시 후 숨김
    private IEnumerator ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        yield return new WaitForSeconds(1f);
        messagePopup.SetActive(false);
    }

    // '다음' 버튼 클릭 시 재료 선택 패널로 이동
    private void GoToIngredientSelection()
    {
        if (selectedRecipe != null)
        {
            startPanel.SetActive(false);
            ingredientSelectionPanel.SetActive(true);
        }
    }

    // 선택한 레시피 반환 토핑 단계에서 쓰기 위해
    public string GetSelectedDessert()
    {
        return selectedRecipe != null ? selectedRecipe.recipeName : null;
    }

    // 선택한 레시피 반환 재료 검증에서 쓰기
    public Recipe GetSelectedRecipe()
    {
        return selectedRecipe;
    }
}
