using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject messagePopup; // �޽��� �˾�
    public TextMeshProUGUI messageText; // �޽��� �ؽ�Ʈ
    public GameObject recipeSelectionPopup; // ���� ���� ���� �˾� �г� �߰�
    public Dictionary<string, Button> recipeButtons = new Dictionary<string, Button>(); // ������ ��ư ���

    private bool isMessageShown = false; // �޽����� ǥ�� ������ ����

    void Start()
    {
        // RecipeSelectionPopup�� �� ������ ��ư�� Dictionary�� ���
        foreach (Button btn in recipeSelectionPopup.GetComponentsInChildren<Button>())
        {
            recipeButtons.Add(btn.name, btn);
        }
    }

    // �޽����� ���� �ð� ���� ǥ���ϰ� �ڵ����� ����� �޼���
    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePopup.SetActive(true);
        StartCoroutine(HideMessageAfterDelay(1f)); // 1�� �� �޽��� ����
    }

    private IEnumerator HideMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messagePopup.SetActive(false);
    }

    // ���õ� ������ ��ư ���� ǥ��
    public void HighlightRecipeButton(string recipeName)
    {
        foreach (var button in recipeButtons.Values)
        {
            // �ٸ� ��ư�� ������ �⺻���� �ǵ���
            button.GetComponent<Image>().color = Color.white;
        }

        // ���õ� ��ư�� ������ �����Ͽ� ���� ǥ��
        if (recipeButtons.TryGetValue(recipeName, out Button selectedButton))
        {
            selectedButton.GetComponent<Image>().color = Color.green; // ���÷� ��� ����
        }
    }
}
