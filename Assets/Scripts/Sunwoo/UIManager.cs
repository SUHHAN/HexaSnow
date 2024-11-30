using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // 메시지 팝업
    public TextMeshProUGUI messageText; // 메시지 텍스트
    public GameObject recipeSelectionPopup; // 제과 종류 선택 팝업 패널 추가
    public Dictionary<string, Button> recipeButtons = new Dictionary<string, Button>(); // 레시피 버튼 목록

    private bool isMessageShown = false; // 메시지가 표시 중인지 여부

    void Start()
    {
        // RecipeSelectionPopup의 각 레시피 버튼을 Dictionary에 등록
        foreach (Button btn in recipeSelectionPopup.GetComponentsInChildren<Button>())
        {
            recipeButtons.Add(btn.name, btn);
        }
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

    // 선택된 레시피 버튼 강조 표시
    public void HighlightRecipeButton(string recipeName)
    {
        foreach (var button in recipeButtons.Values)
        {
            // 다른 버튼의 색상을 기본으로 되돌림
            button.GetComponent<Image>().color = Color.white;
        }

        // 선택된 버튼의 색상을 변경하여 강조 표시
        if (recipeButtons.TryGetValue(recipeName, out Button selectedButton))
        {
            selectedButton.GetComponent<Image>().color = Color.green; // 예시로 녹색 강조
        }
    }
}
