using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum으로 단계 관리
public enum GameState
{
    Start, // 시작 단계
    IngredientSelection, // 재료 선택 단계
    Mixing, // 반죽 미니게임 단계
    Oven, // 오븐 미니게임 단계
    Topping, // 토핑 단계
    Result // 결과 단계
}

public class BakingGameManager : MonoBehaviour
{
    public GameState currentState; // 현재 게임 상태
    public GameObject startPanel; // 처음 시작 패널
    public GameObject ingredientSelectionPanel; // 재료 선택 패널
    public GameObject mixingPanel; // 반죽 미니게임 패널
    public GameObject ovenPanel; // 오븐 미니게임 패널
    public GameObject toppingPanel; // 토핑 재료 선택 패널
    public GameObject resultPanel; // 결과 패널

    private bool isCooking = false; // 조리 중 여부
    public RecipeBook recipeBook; // 레시피 관리 스크립트
    public UIManager uiManager; // UI 관리 스크립트

    public GameObject recipeSelectionPopup; // 제과 종류 선택 팝업(StartPanel 내부)


    void Start()
    {
        SetGameState(GameState.Start); // 초기 상태를 시작 단계로 설정
        isCooking = false; // 처음에는 조리 중이 아님
        recipeSelectionPopup.SetActive(false); // 처음에는 제과 선택 팝업 비활성화
        ingredientSelectionPanel.SetActive(false); // 게임 시작 시 재료 선택 패널 비활성화
    }

    // 게임 상태를 설정하는 메서드. 매개변수로 받은 상태를 통해 currentState 변경하고 ui도 변경함
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        UpdateUI();
    }

    // currentState에 따라 각 상태에 해당하는 패널만 활성화하고 나머지는 꺼둠
    void UpdateUI()
    {
        startPanel.SetActive(currentState == GameState.Start);
        ingredientSelectionPanel.SetActive(currentState == GameState.IngredientSelection);
        mixingPanel.SetActive(currentState == GameState.Mixing);
        ovenPanel.SetActive(currentState == GameState.Oven);
        toppingPanel.SetActive(currentState == GameState.Topping);
        resultPanel.SetActive(currentState == GameState.Result);

        recipeSelectionPopup.SetActive(false);
    }

    // '시작' 버튼 클릭 시 호출
    public void OnStartButtonClick()
    {
        if (!isCooking)
        {
            isCooking = true;
            // '시작' 버튼이 있는 StartPanel의 버튼을 숨기고 제과 종류 선택 팝업을 활성화
            startPanel.transform.Find("StartButton").gameObject.SetActive(false); // 시작 버튼 비활성화
            recipeSelectionPopup.SetActive(true); // 제과 종류 선택 팝업 활성화
        }
    }

    // 레시피 선택 시 호출
    public void OnRecipeSelected(string recipeName)
    {
        Recipe selectedRecipe = recipeBook.GetRecipeByName(recipeName);

        if (selectedRecipe != null)
        {
            if (selectedRecipe.canBake)
            {
                // 해금된 레시피인 경우
                SetGameState(GameState.IngredientSelection); // IngredientSelectionPanel로 전환
                recipeSelectionPopup.SetActive(false); // 레시피 선택 팝업 비활성화
            }
            else
            {
                // 해금되지 않은 레시피인 경우
                uiManager.ShowMessage("not yet");
            }
        }
    }

    // 메뉴 선택 후 재료 선택 단계로 전환하는 메서드
    // 메뉴 선택이 끝났을 때 호출할거임
    public void OnStartComplete()
    {
        SetGameState(GameState.IngredientSelection);
    }

    // 재료 고르기 끝났을 때 호출
    public void OnIngredientSelectionComplete()
    {
        SetGameState(GameState.Mixing);
    }

    // 반죽 게임 끝나면 호출
    public void OnMixingComplete()
    {
        SetGameState(GameState.Oven);
    }

    // 오븐 게임 끝나면 호출
    public void OnOvenComplete()
    {
        SetGameState(GameState.Topping);
    }

    // 토핑 끝나면 호출
    public void OnToppingComplete()
    {
        SetGameState(GameState.Result);
    }
}
