using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // 메시지 팝업
    public TextMeshProUGUI messageText; // 메시지 텍스트

    private bool isMessageShown = false; // 메시지가 표시 중인지 여부

    // 메시지 표시하거나 숨김
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(1f)); // 1초 후 메시지 숨김
    }

    // 지정된 시간 후에 메시지를 숨기는 코루틴
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 1초 대기
        messagePopup.SetActive(false); // 메시지 숨기기
    }
}
