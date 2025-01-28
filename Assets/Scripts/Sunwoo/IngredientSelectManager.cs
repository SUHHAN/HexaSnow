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

    public GameObject refrigeratorPanel; // 냉장고 패널
    public GameObject shelfPanel; // 선반 패널

    public InventoryManager inventoryManager; // 재료 정보 관리
    private BakingStartManager bakingStartManager; // 선택한 레시피 가져오기

    private List<string> selectedIngredients = new List<string>(); // 사용자가 선택한 재료 목록

    void Start()
    {
        // BakingStartManager 참조 가져오기
        bakingStartManager = FindObjectOfType<BakingStartManager>();

        // 버튼 이벤트 등록
        addIngredientButton.onClick.AddListener(OpenAddIngredientPanel);
        finishButton.onClick.AddListener(FinishIngredientSelection);

        finishButton.gameObject.SetActive(false); // 초기에는 비활성화
        addingIngredientPanel.SetActive(false); // 초기에는 비활성화
    }

    // '재료 담기' 버튼 클릭 시 실행
    private void OpenAddIngredientPanel()
    {
        startIngredientPanel.SetActive(false); // 기존 패널 비활성화
        addingIngredientPanel.SetActive(true); // 새로운 패널 활성화
        GenerateIngredientButtons(); // 재료 버튼 생성 실행
    }

    // 재료 버튼 동적 생성 & 소지한 재료만 활성화
    private void GenerateIngredientButtons()
    {
        Debug.Log("재료 버튼 생성 시작");

        // 냉장고 재료 버튼 설정
        foreach (Transform child in refrigeratorPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button == null) continue; // Button이 없으면 무시

            string ingredientName = button.name;
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            button.interactable = hasIngredient;
            button.gameObject.SetActive(hasIngredient); // 소지한 재료만 활성화

            // ImageBefore & ImageAfter 설정
            Transform imageBeforeTransform = child.Find("Imagebf");
            Transform imageAfterTransform = child.Find("Imageaft");

            if (imageBeforeTransform == null || imageAfterTransform == null)
            {
                Debug.LogError($"{ingredientName}의 ImageBefore 또는 ImageAfter가 없음!");
                continue;
            }

            GameObject imageBefore = imageBeforeTransform.gameObject;
            GameObject imageAfter = imageAfterTransform.gameObject;

            imageBefore.SetActive(true);
            imageAfter.SetActive(false);

            // 버튼 클릭 시 이미지 변경 이벤트 추가
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnIngredientButtonClick(button, ingredientName, imageBefore, imageAfter));
        }

        // 선반 재료 버튼 설정
        foreach (Transform child in shelfPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button == null) continue; // Button이 없으면 무시

            string ingredientName = button.name;
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            button.interactable = hasIngredient;
            button.gameObject.SetActive(hasIngredient); // 소지한 재료만 활성화

            // ImageBefore & ImageAfter 설정
            Transform imageBeforeTransform = child.Find("Imagebf");
            Transform imageAfterTransform = child.Find("Imageaft");

            if (imageBeforeTransform == null || imageAfterTransform == null)
            {
                Debug.LogError($"{ingredientName}의 ImageBefore 또는 ImageAfter가 없음!");
                continue;
            }

            GameObject imageBefore = imageBeforeTransform.gameObject;
            GameObject imageAfter = imageAfterTransform.gameObject;

            imageBefore.SetActive(true);
            imageAfter.SetActive(false);

            // 버튼 클릭 시 이미지 변경 이벤트 추가
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnIngredientButtonClick(button, ingredientName, imageBefore, imageAfter));
        }
    }

    // 재료 클릭 시 이미지 변경 및 선택/해제 로직 수정
    public void OnIngredientButtonClick(Button button, string ingredientName, GameObject imageBefore, GameObject imageAfter)
    {
        if (selectedIngredients.Contains(ingredientName))
        {
            // 선택 해제
            selectedIngredients.Remove(ingredientName);
            imageBefore.SetActive(true);
            imageAfter.SetActive(false);
        }
        else
        {
            // 재료 선택
            selectedIngredients.Add(ingredientName);
            imageBefore.SetActive(false);
            imageAfter.SetActive(true);
        }

        // 선택된 재료가 하나라도 있으면 완료 버튼 활성화
        finishButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }

    // 선택한 재료가 레시피와 일치하는지 검증 후 MixingPanel로 이동
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
        mixingPanel.SetActive(true); // MixingPanel 활성화
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
