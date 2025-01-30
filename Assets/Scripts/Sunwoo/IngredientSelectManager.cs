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

    // 냉장고 & 선반 버튼 미리 할당 (Inspector에서 설정)
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


    // **소지한 재료만 활성화**
    private void UpdateIngredientButtons()
    {
        // 냉장고 버튼 처리
        foreach (GameObject buttonObj in refrigeratorButtons)
        {
            string ingredientName = buttonObj.name.Replace("Button", ""); // ex. "ButterButton" -> "Butter"
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            buttonObj.SetActive(hasIngredient);
            Button button = buttonObj.GetComponent<Button>();

            if (hasIngredient && button != null)
            {
                button.onClick.RemoveAllListeners(); // 기존 이벤트 제거
                button.onClick.AddListener(() => OnIngredientButtonClick(buttonObj, ingredientName));

                Debug.Log($"냉장고 재료: {ingredientName}, 소지 여부: {hasIngredient}");
            }
        }

        // 선반 버튼 처리
        foreach (GameObject buttonObj in shelfButtons)
        {
            string ingredientName = buttonObj.name.Replace("Button", ""); // "FlourButton" -> "Flour"
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

    // 재료 버튼 클릭 시 이미지 변경 및 선택된 재료 리스트 업데이트
    public void OnIngredientButtonClick(GameObject buttonObj, string ingredientName)
    {
        Transform imageBefore = buttonObj.transform.Find("Imagebf");
        Transform imageAfter = buttonObj.transform.Find("Imageaft");

        if (imageBefore == null || imageAfter == null)
        {
            Debug.LogError($"오류: {ingredientName} 버튼에 'Imagebf' 또는 'Imageaft'가 없습니다!");
            return;
        }

        if (selectedIngredients.Contains(ingredientName))
        {
            selectedIngredients.Remove(ingredientName);
            imageBefore.gameObject.SetActive(true);
            imageAfter.gameObject.SetActive(false);
            Debug.Log($"재료 선택 해제: {ingredientName}");
        }
        else
        {
            selectedIngredients.Add(ingredientName);
            imageBefore.gameObject.SetActive(false);
            imageAfter.gameObject.SetActive(true);
            Debug.Log($"재료 선택: {ingredientName}");
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
            Debug.Log("재료가 레시피와 일치합니다!");
        }
        else
        {
            Debug.Log("선택한 재료가 레시피와 일치하지 않습니다.");
        }

        ingredientSelectionPanel.SetActive(false);
        mixingGameManager.ActivateMixingPanel(); // MixingGameManager의 ActivateMixingPanel 호출
    }

    // 선택한 재료가 레시피와 일치하는지 확인
    private bool VerifyIngredients(List<string> requiredIngredients)
    {
        foreach (string ingredient in requiredIngredients)
        {
            if (!selectedIngredients.Contains(ingredient))
            {
                return false; // 필요한 재료가 없으면 false 반환
            }
        }
        return true;
    }
}
