using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 레시피 해금 여부 확인
public class RecipeManager : MonoBehaviour
{
    public RecipeData[] recipes; // ScriptableObject로 관리
    public GameObject nextButton;
    public GameObject lockedMessage;

    private RecipeData selectedRecipe;

    public void SelectRecipe(int index)
    {
        selectedRecipe = recipes[index];
        if (selectedRecipe.isUnlocked)
        {
            nextButton.SetActive(true); // '다음' 버튼 활성화
        }
        else
        {
            ShowLockedMessage(); // 해금되지 않은 경우 메시지 표시
        }
    }

    private void ShowLockedMessage()
    {
        lockedMessage.SetActive(true);
        Invoke("HideLockedMessage", 2f); // 2초 후 숨기기
    }

    private void HideLockedMessage()
    {
        lockedMessage.SetActive(false);
    }
}
