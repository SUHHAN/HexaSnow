using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BakingStartGameManager : MonoBehaviour
{
    public GameObject startButton; // '게임 시작' 버튼
    public GameObject recipePopup; // 제과 종류 선택 UI
    public GameObject nextButton; // '다음' 버튼. IngredientScene으로 넘어가는 버튼
    public TextMeshProUGUI unlockMessage; // '해금되지 않았습니다!' 안내 UI
    public Transform recipeButtonContainer; // 제과 버튼 배치할 컨테이너
    public GameObject recipeButtonPrefab; // 제과 버튼 프리팹

    public string selectedRecipe = null; // 선택된 제과의 이름

    // 레시피와 해금 상태를 저장하는 딕셔너리
    private Dictionary<string, (List<string> ingredients, bool isUnlocked)> recipes = new Dictionary<string, (List<string>, bool)>
    {
        { "Madeleine", (new List<string>{ "Butter", "Egg", "Flour", "Sugar" }, true) }, // 마들렌만 기본적으로 해금됨
        { "Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder" }, false) },
        { "Choco Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Cocoa Powder", "Chocolate Chips" }, false) },
        { "Blueberry Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Blueberry" }, false) },
        { "Cookie", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Almond Powder", "Sugar Powder" }, false) },
        { "Pound Cake", (new List<string>{ "Butter", "Egg", "Flour", "Sugar" }, false) },
        { "Financier", (new List<string>{ "Browned Butter", "Egg Whites", "Flour", "Sugar", "Almond Powder", "Honey" }, false) },
        { "Basque Cheesecake", (new List<string>{ "Egg", "Sugar", "Cream Cheese", "Heavy Cream" }, false) },
        { "Scone", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Heavy Cream" }, false) },
        { "Tart", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Cream Cheese", "Condensed Milk", "Heavy Cream", "Almond Powder", "Sugar Powder" }, false) }
    };

    // Start is called before the first frame update
    void Start()
    {
        recipePopup.SetActive(false); // 시작할 때 제과 팝업 비활성화
        nextButton.SetActive(false); // 시작할 때 "Next" 버튼 비활성화
        unlockMessage.gameObject.SetActive(false); // "해금되지 않았습니다!" 메시지 비활성화

        InitializeRecipeButtons(); // 제과 버튼 초기화
    }

    // '시작하기' 버튼 누를 경우
    public void OnStartButtonClick()
    {
        startButton.SetActive(false); // "Start" 버튼 숨기기
        recipePopup.SetActive(true); // 제과 선택 팝업 활성화
    }

    private void InitializeRecipeButtons()
    {
        foreach (var recipe in recipes) // 모든 제과를 반복
        {
            GameObject button = Instantiate(recipeButtonPrefab, recipeButtonContainer); // 버튼 프리팹 생성
            button.GetComponentInChildren<Text>().text = recipe.Key; // 버튼 텍스트에 제과 이름 설정

            bool isUnlocked = recipe.Value.isUnlocked; // 제과의 해금 상태 확인
            button.GetComponent<Button>().interactable = isUnlocked; // 해금 상태에 따라 버튼 활성화/비활성화

            button.GetComponent<Button>().onClick.AddListener(() => OnRecipeSelect(recipe.Key)); // 버튼 클릭 이벤트 연결
        }
    }

    // 제과 버튼 눌렀을 경우
    public void OnRecipeSelect(string recipeName)
    {
        if (recipes[recipeName].isUnlocked) // 선택한 제과가 해금되었는지 확인
        {
            selectedRecipe = recipeName; // 선택된 제과 이름 저장
            nextButton.SetActive(true); // "Next" 버튼 활성화
        }
        else
        {
            StartCoroutine(ShowUnlockMessage()); // "해금되지 않았습니다!" 메시지 표시
        }
    }

    // '다음' 버튼 눌렀을 경우
    public void OnNextButtonClick()
    {
        BakingGameManager1.Instance.SetSelectedRecipe(selectedRecipe); // GameManager에 선택된 제과 저장
        SceneManager.LoadScene("IngredientScene"); // IngredientScene으로 전환
    }

    private IEnumerator ShowUnlockMessage()
    {
        unlockMessage.gameObject.SetActive(true); // 메시지 활성화
        yield return new WaitForSeconds(1f); // 1초 동안 대기
        unlockMessage.gameObject.SetActive(false); // 메시지 비활성화
    }
}
