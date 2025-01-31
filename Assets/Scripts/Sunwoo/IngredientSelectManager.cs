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
    private int ingredientScore = 30; // 초기 총점 30점

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
                Debug.Log($"냉장고 재료: {ingredientName}, 소지 여부: {hasIngredient}");
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
                Debug.Log($"선반 재료: {ingredientName}, 소지 여부: {hasIngredient}");
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
            Debug.Log($"재료 선택 해제: {ingredientName}");
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
                Debug.Log($"재료 선택: {ingredientName}");
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

        if (VerifyIngredients(selectedRecipe.ingredients))
        {
            ingredientScore -= 5;
            Debug.Log("재료가 레시피와 정확히 일치합니다!");
        }
        else
        {
            Debug.Log("선택한 재료가 레시피와 일치하지 않습니다.");
        }

        // 최종 점수 출력 (Debug 로그)
        Debug.Log($"최종 재료 점수: {ingredientScore}/30");

        ingredientSelectionPanel.SetActive(false);
        mixingGameManager.ActivateMixingPanel();
    }

    // 선택한 재료가 레시피와 정확히 일치하는지 확인 (순서 고려 X)
    private bool VerifyIngredients(List<string> requiredIngredients)
    {
        if (selectedIngredients.Count != requiredIngredients.Count)
        {
            return false; // 개수가 다르면 실패
        }

        foreach (string ingredient in selectedIngredients)
        {
            if (!requiredIngredients.Contains(ingredient))
            {
                return false; // 불필요한 재료가 포함되면 실패
            }
        }
        return true;
    }
}
