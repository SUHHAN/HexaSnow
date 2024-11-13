using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // �޽��� �˾�
    public TextMeshProUGUI messageText; // �޽��� �ؽ�Ʈ

    private bool isMessageShown = false; // �޽����� ǥ�� ������ ����

    // �޽��� ǥ���ϰų� ����
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(1f)); // 1�� �� �޽��� ����
    }

    // ������ �ð� �Ŀ� �޽����� ����� �ڷ�ƾ
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 1�� ���
        messagePopup.SetActive(false); // �޽��� �����
    }
}
