using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToppingManager : MonoBehaviour
{
    public GameObject toppingPanel; // Topping ��ü �г�
    public GameObject startToppingPanel; // ���� �г�
    public GameObject addToppingPanel; // ���� ���� �г�
    public GameObject resultPanel; // ��� �г�

    public Button startToppingButton; // ���� ���� ��ư
    public Button finishToppingButton; // �Ϸ� ��ư

    public InventoryManager inventoryManager; // �κ��丮 ����

    // ���� ��ư �̸� �Ҵ� (Inspector���� ����)
    public List<GameObject> refrigeratorButtons;

    private string selectedTopping = null; // ���õ� ���� (�� ���� ����)

    void Start()
    {
        // �ʱ� UI ����
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        resultPanel.SetActive(false);

        // ��ư �̺�Ʈ ���
        startToppingButton.onClick.AddListener(OpenToppingSelection);
        finishToppingButton.onClick.AddListener(FinishToppingSelection);

        UpdateToppingButtons(); // ������ ��Ḹ Ȱ��ȭ
    }

    // StartToppingButton Ŭ�� �� ����
    private void OpenToppingSelection()
    {
        startToppingPanel.SetActive(false);
        addToppingPanel.SetActive(true);
        UpdateToppingButtons(); // �ٽ� ������Ʈ
    }

    // **������ ���θ� ��ư Ȱ��ȭ**
    private void UpdateToppingButtons()
    {
        Debug.Log("���� ��ư ������Ʈ ����");

        foreach (GameObject buttonObj in refrigeratorButtons)
        {
            string toppingName = buttonObj.name.Replace("Button", ""); // "BananaButton" -> "Banana"
            bool hasTopping = inventoryManager.HasIngredient(toppingName);

            buttonObj.SetActive(hasTopping);
            Debug.Log($"����: {toppingName}, ���� ����: {hasTopping}");

            if (hasTopping)
            {
                Button button = buttonObj.GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => SelectSingleTopping(buttonObj, toppingName));
            }
        }
    }

    // ���� ��ư Ŭ�� ��, �ϳ��� ���θ� ���� �����ϰ� ����
    private void SelectSingleTopping(GameObject buttonObj, string toppingName)
    {
        // ���� ���õ� ���� ����
        if (selectedTopping != null)
        {
            foreach (GameObject btn in refrigeratorButtons)
            {
                if (btn.name.Replace("Button", "") == selectedTopping)
                {
                    Transform prevImageBefore = btn.transform.Find("Imagebf");
                    Transform prevImageAfter = btn.transform.Find("Imageaft");
                    prevImageBefore.gameObject.SetActive(true);
                    prevImageAfter.gameObject.SetActive(false);
                    break;
                }
            }
        }

        // ���Ӱ� ������ ���� ����
        if (selectedTopping == toppingName)
        {
            // ���� ��ư�� �ٽ� �����ٸ� ���� ����
            selectedTopping = null;
        }
        else
        {
            selectedTopping = toppingName;
            Transform imageBefore = buttonObj.transform.Find("Imagebf");
            Transform imageAfter = buttonObj.transform.Find("Imageaft");

            imageBefore.gameObject.SetActive(false);
            imageAfter.gameObject.SetActive(true);
        }
    }

    // �Ϸ� ��ư Ŭ�� �� ����
    private void FinishToppingSelection()
    {
        Debug.Log($"���õ� ����: {selectedTopping ?? "����"}");
        toppingPanel.SetActive(false);
        resultPanel.SetActive(true);
    }
}
