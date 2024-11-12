using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup;
    public GameObject recipeSelectionPopup; // ���� ���� ���� �˾�
    public TextMeshProUGUI messageText;

    // �޽��� ǥ��
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
    }

    // �޽��� �˾� �ݱ�
    public void HideMessage()
    {
        messagePopup.SetActive(false);
    }
}
