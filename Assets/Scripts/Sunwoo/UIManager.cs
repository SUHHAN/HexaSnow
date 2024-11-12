using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup;
    public GameObject recipeSelectionPopup; // 제과 종류 선택 팝업
    public TextMeshProUGUI messageText;

    // 메시지 표시
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
    }

    // 메시지 팝업 닫기
    public void HideMessage()
    {
        messagePopup.SetActive(false);
    }
}
