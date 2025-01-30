using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToppingManager : MonoBehaviour
{
    public GameObject toppingPanel; // Topping ��ü �г�
    public GameObject startToppingPanel; // ���� �г�
    public GameObject addToppingPanel; // ���� ���� �г�
    public GameObject finishBakingPanel; // ��� �г�

    public Button startToppingButton; // ���� ���� ��ư
    public Button finishToppingButton; // �Ϸ� ��ư

    public InventoryManager inventoryManager; // �κ��丮 ����
    public Image bakingImage;

    // ���� ��ư �̸� �Ҵ�
    public List<GameObject> refrigeratorButtons;

    private string selectedTopping = null; // ���õ� ����

    public BakingStartManager bakingStartManager; // BakingStartManager ����

    void Start()
    {
        // �ʱ� UI ����
        startToppingPanel.SetActive(true);
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(false);

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

    // ������ ���θ� ��ư Ȱ��ȭ
    private void UpdateToppingButtons()
    {
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
                    Image prevImageAfter = btn.transform.Find("Imageaft").GetComponent<Image>();
                    prevImageAfter.color = new Color(1f, 1f, 1f, 1f);
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
            Image imageAfter = buttonObj.transform.Find("Imageaft").GetComponent<Image>();
            imageAfter.color = new Color(1f, 1f, 1f, 0.3f);
        }
    }

    // �Ϸ� ��ư Ŭ�� �� ����
    private void FinishToppingSelection()
    {
        Debug.Log($"���õ� ����: {selectedTopping ?? "����"}");
        addToppingPanel.SetActive(false);
        finishBakingPanel.SetActive(true);
        UpdateBakingImage();
    }

    private void UpdateBakingImage()
    {
        string selectedDessert = bakingStartManager.GetSelectedDessert();
        string imagePath = $"Sunwoo/Images/menu_{selectedDessert.ToLower()}_{(selectedTopping ?? "original").ToLower()}";
        bakingImage.sprite = Resources.Load<Sprite>(imagePath);
    }
}
