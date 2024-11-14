using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // 메시지 팝업
    public TextMeshProUGUI messageText; // 메시지 텍스트
    public GameObject recipeSelectionPopup; // 제과 종류 선택 팝업 패널
    public GameObject addingIngredientPanel; // 재료 담기 패널

    public InventoryManager inventoryManager; // 인벤토리 매니저 참조
    public GameObject refrigeratorPanel; // 냉장고 패널
    public GameObject shelfPanel; // 선반 패널
    public GameObject finishIngredientButton; // 재료 선택 완료 버튼

    public Dictionary<string, Button> recipeButtons = new Dictionary<string, Button>(); // 레시피 버튼 목록
    private Button currentlySelectedButton = null; // 현재 선택된 버튼 저장
    private List<Button> selectedIngredients = new List<Button>(); // 선택된 재료 버튼 목록

    private float buttonSpacing = 110f; // 버튼 간격
    private Vector2 buttonSize = new Vector2(100, 30); // 버튼 크기

    void Start()
    {
        // RecipeSelectionPopup의 각 레시피 버튼을 Dictionary에 등록
        foreach (Button btn in recipeSelectionPopup.GetComponentsInChildren<Button>())
        {
            recipeButtons.Add(btn.name, btn);
        }

        finishIngredientButton.SetActive(false); // 초기에는 finishIngredient 버튼 비활성화
        GenerateIngredientButtons(); // 냉장고와 선반에 재료 버튼 생성
    }

    // 현재 선택된 레시피 버튼 반환
    public Button GetCurrentlySelectedButton()
    {
        return currentlySelectedButton;
    }

    // 메시지를 일정 시간 동안 표시하고 자동으로 숨기는 메서드
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(1f)); // 1초 후 메시지 숨김
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messagePopup.SetActive(false);
    }

    // 선택된 레시피 버튼 강조 표시 (토글 방식)
    public void HighlightRecipeButton(string recipeName)
    {
        // 현재 선택된 버튼을 클릭한 경우 선택 해제
        if (currentlySelectedButton != null && currentlySelectedButton.name == recipeName)
        {
            currentlySelectedButton.GetComponent<Image>().color = Color.white; // 기본 색상으로 되돌림
            currentlySelectedButton = null; // 선택 해제
        }
        else
        {
            // 기존 선택된 버튼이 있을 경우 색상 초기화
            if (currentlySelectedButton != null)
            {
                currentlySelectedButton.GetComponent<Image>().color = Color.white;
            }

            // 새로 선택된 버튼을 초록색으로 강조 표시
            if (recipeButtons.TryGetValue(recipeName, out Button selectedButton))
            {
                selectedButton.GetComponent<Image>().color = Color.green;
                currentlySelectedButton = selectedButton; // 현재 선택된 버튼 업데이트
            }
        }
    }

    // 재료 버튼 초기화
    public void InitializeIngredientButtons()
    {
        // AddingIngredient 패널 내의 모든 버튼 초기화
        foreach (Button button in addingIngredientPanel.GetComponentsInChildren<Button>())
        {
            button.GetComponent<Image>().color = Color.white; // 버튼 색상을 기본값으로 설정
            button.onClick.AddListener(() => OnIngredientButtonClick(button)); // 클릭 이벤트 설정
        }
        selectedIngredients.Clear(); // 선택된 재료 목록 초기화
        finishIngredientButton.SetActive(false); // 'finishIngredient' 버튼 비활성화
    }

    // 재료 버튼 클릭 시 호출
    public void OnIngredientButtonClick(Button button)
    {
        if (selectedIngredients.Contains(button))
        {
            // 선택된 재료를 다시 클릭하면 선택 해제
            button.GetComponent<Image>().color = Color.white;
            selectedIngredients.Remove(button);
        }
        else
        {
            // 재료 선택
            button.GetComponent<Image>().color = Color.green;
            selectedIngredients.Add(button);
        }

        // 선택된 재료가 하나 이상 있을 때만 finishIngredient 버튼 활성화
        finishIngredientButton.SetActive(selectedIngredients.Count > 0);
    }

    // 선택된 재료 이름 목록 반환
    public List<string> GetSelectedIngredientNames()
    {
        List<string> ingredientNames = new List<string>();
        foreach (Button button in selectedIngredients)
        {
            ingredientNames.Add(button.name);
        }
        return ingredientNames;
    }

    // 특정 패널의 활성화 상태를 설정하는 메서드
    public void TogglePanel(GameObject panel, bool isActive)
    {
        panel.SetActive(isActive);
    }

    // 소지한 재료만 냉장고와 선반에 버튼으로 생성
    public void GenerateIngredientButtons()
    {
        // 냉장고 패널 내 버튼 위치 설정
        Vector2 startPosition = new Vector2(0, 0);
        int index = 0;

        // 냉장고 재료 목록
        string[] fridgeIngredients = { "Butter", "Egg", "Milk", "Egg Whites", "Browned Butter", "Cream Cheese", "Heavy Cream", "Condensed Milk" };
        foreach (string ingredient in fridgeIngredients)
        {
            if (inventoryManager.HasIngredient(ingredient))
            {
                CreateIngredientButton(ingredient, refrigeratorPanel, startPosition + new Vector2((index % 4) * buttonSpacing, -(index / 4) * buttonSpacing));
                index++;
            }
        }

        // 선반 패널 내 버튼 위치 설정
        startPosition = new Vector2(0, 0);
        index = 0;

        // 선반 재료 목록
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

    // 재료 버튼 생성 및 클릭 이벤트 설정
    void CreateIngredientButton(string ingredient, GameObject parentPanel, Vector2 position)
    {
        // UI 버튼을 생성
        GameObject buttonObj = new GameObject(ingredient, typeof(RectTransform), typeof(CanvasRenderer), typeof(Button), typeof(Image));
        buttonObj.transform.SetParent(parentPanel.transform, false);

        // RectTransform 및 위치 설정
        RectTransform rectTransform = buttonObj.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = position;
        rectTransform.sizeDelta = buttonSize;

        // Image 컴포넌트 설정
        Image buttonImage = buttonObj.GetComponent<Image>();
        buttonImage.color = Color.white; // 버튼 기본 색상 설정 (흰색)

        // 텍스트 추가 (재료 이름)
        GameObject textObj = new GameObject("Text");
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = ingredient;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontSize = 18;
        buttonText.color = Color.black; // 텍스트를 검은색으로 설정
        textObj.transform.SetParent(buttonObj.transform, false);

        // 텍스트 위치와 크기 설정
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = buttonSize;
        textRect.anchoredPosition = Vector2.zero;

        // 버튼 클릭 이벤트 추가
        buttonObj.GetComponent<Button>().onClick.AddListener(() => OnIngredientButtonClick(buttonObj.GetComponent<Button>()));
    }
}
