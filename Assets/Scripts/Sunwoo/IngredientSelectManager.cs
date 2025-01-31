using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSelectManager : MonoBehaviour
{
    public GameObject ingredientSelectionPanel; // 재료 선택 패널
    public GameObject startIngredientPanel; // 초기 패널
    public GameObject addingIngredientPanel; // 재료 추가 패널
    public GameObject mixingPanel; // Mixing 패널

    public Button addIngredientButton; // '재료 담기' 버튼
    public Button finishButton; // '완료' 버튼

    public InventoryManager inventoryManager; // 재료 정보 관리
    private BakingStartManager bakingStartManager; // 선택한 레시피 가져오기

    private List<string> selectedIngredients = new List<string>(); // 사용자가 선택한 재료 목록
    public int ingredientScore = 30; // 초기 총점 30점

    // 냉장고 & 선반 버튼 리스트
    public List<GameObject> refrigeratorButtons;
    public List<GameObject> shelfButtons;

    public MixingGameManager mixingGameManager; // MixingGameManager 참조

    void Start()
    {
        // BakingStartManager 참조 가져오기
        bakingStartManager = FindObjectOfType<BakingStartManager>();

        // 버튼 이벤트 등록
        addIngredientButton.onClick.AddListener(OpenAddIngredientPanel);
        finishButton.onClick.AddListener(FinishIngredientSelection);

        finishButton.gameObject.SetActive(false); // 초기에는 비활성화
        addingIngredientPanel.SetActive(false); // 초기에는 비활성화

        UpdateIngredientButtons(); // 재료 버튼 업데이트
    }

    // '재료 담기' 버튼 클릭 시 실행
    private void OpenAddIngredientPanel()
    {
        startIngredientPanel.SetActive(false); // 기존 패널 비활성화
        addingIngredientPanel.SetActive(true); // 새로운 패널 활성화
        UpdateIngredientButtons(); // 재료 버튼 상태 업데이트
    }

    // 소지한 재료만 버튼 활성화
    private void UpdateIngredientButtons()
    {
        foreach (GameObject buttonObj in refrigeratorButtons)
        {
            string ingredientName = buttonObj.name.Replace("Button", "");
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            buttonObj.SetActive(hasIngredient);
            Button button = buttonObj.GetComponent<Button>();

            if (hasIngredient && button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnIngredientButtonClick(buttonObj, ingredientName));
            }
        }

        foreach (GameObject buttonObj in shelfButtons)
        {
            string ingredientName = buttonObj.name.Replace("Button", "");
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            buttonObj.SetActive(hasIngredient);
            Button button = buttonObj.GetComponent<Button>();

            if (hasIngredient && button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnIngredientButtonClick(buttonObj, ingredientName));
            }
        }
    }

    // 재료 버튼 클릭 시 투명도 조절 및 선택된 재료 리스트 업데이트
    public void OnIngredientButtonClick(GameObject buttonObj, string ingredientName)
    {
        Image imageAfter = buttonObj.transform.Find("Imageaft")?.GetComponent<Image>();

        if (imageAfter == null)
        {
            Debug.LogError($"오류: {ingredientName} 버튼에 'Imageaft'가 없습니다!");
            return;
        }

        if (selectedIngredients.Contains(ingredientName))
        {
            selectedIngredients.Remove(ingredientName);
            Color color = imageAfter.color;
            color.a = 1f; // 원래 불투명하게 설정
            imageAfter.color = color;
            inventoryManager.AddIngredient(ingredientName); // 개수 +1
        }
        else
        {
            // 선택 시: 개수 감소
            if (inventoryManager.UseIngredient(ingredientName))
            {
                selectedIngredients.Add(ingredientName);
                Color color = imageAfter.color;
                color.a = 0.3f; // 투명도를 30%로 설정
                imageAfter.color = color;
            }
            else
            {
                Debug.LogError($"재료 {ingredientName} 개수가 부족합니다!");
            }
        }

        // 선택된 재료가 하나라도 있으면 완료 버튼 활성화
        finishButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }

    // 선택한 재료가 레시피와 일치하는지 검증
    private void FinishIngredientSelection()
    {
        Recipe selectedRecipe = bakingStartManager.GetSelectedRecipe();
        if (selectedRecipe == null)
        {
            Debug.LogError("선택된 레시피가 없습니다!");
            return;
        }

        // 정확한 개수 및 재료 체크
        ingredientScore = CalculateScore(selectedRecipe.ingredients, selectedIngredients);

        // 최종 점수 출력
        Debug.Log($"최종 재료 점수: {ingredientScore}/30");

        ingredientSelectionPanel.SetActive(false);
        mixingGameManager.ActivateMixingPanel();
    }

    private int CalculateScore(List<string> requiredIngredients, List<string> selectedIngredients)
    {
        int score = 30;

        // 올바른 재료 개수와 비교
        int extraIngredients = selectedIngredients.Count - requiredIngredients.Count;

        // 오버하거나 부족하면 -5점씩 감점
        score -= Mathf.Abs(extraIngredients) * 5;

        // 선택한 재료가 레시피에 없는 재료일 경우 -5점씩 감점
        foreach (string ingredient in selectedIngredients)
        {
            if (!requiredIngredients.Contains(ingredient))
            {
                score -= 5;
            }
        }

        // 최소 0점 보장
        return Mathf.Max(0, score);
    }
}
