using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientSelectManager : MonoBehaviour
{
    public GameObject ingredientSelectionPanel; // ��� ���� �г�
    public GameObject startIngredientPanel; // �ʱ� �г�
    public GameObject addingIngredientPanel; // ��� �߰� �г�
    public GameObject mixingPanel; // Mixing �г�

    public Button addIngredientButton; // '��� ���' ��ư
    public Button finishButton; // '�Ϸ�' ��ư

    public GameObject refrigeratorPanel; // ����� �г�
    public GameObject shelfPanel; // ���� �г�

    public InventoryManager inventoryManager; // ��� ���� ����
    private BakingStartManager bakingStartManager; // ������ ������ ��������

    private List<string> selectedIngredients = new List<string>(); // ����ڰ� ������ ��� ���

    void Start()
    {
        // BakingStartManager ���� ��������
        bakingStartManager = FindObjectOfType<BakingStartManager>();

        // ��ư �̺�Ʈ ���
        addIngredientButton.onClick.AddListener(OpenAddIngredientPanel);
        finishButton.onClick.AddListener(FinishIngredientSelection);

        finishButton.gameObject.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
        addingIngredientPanel.SetActive(false); // �ʱ⿡�� ��Ȱ��ȭ
    }

    // '��� ���' ��ư Ŭ�� �� ����
    private void OpenAddIngredientPanel()
    {
        startIngredientPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
        addingIngredientPanel.SetActive(true); // ���ο� �г� Ȱ��ȭ
        GenerateIngredientButtons(); // ��� ��ư ���� ����
    }

    // ��� ��ư ���� ���� & ������ ��Ḹ Ȱ��ȭ
    private void GenerateIngredientButtons()
    {
        Debug.Log("��� ��ư ���� ����");

        // ����� ��� ��ư ����
        foreach (Transform child in refrigeratorPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button == null) continue; // Button�� ������ ����

            string ingredientName = button.name;
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            button.interactable = hasIngredient;
            button.gameObject.SetActive(hasIngredient); // ������ ��Ḹ Ȱ��ȭ

            // ImageBefore & ImageAfter ����
            Transform imageBeforeTransform = child.Find("Imagebf");
            Transform imageAfterTransform = child.Find("Imageaft");

            if (imageBeforeTransform == null || imageAfterTransform == null)
            {
                Debug.LogError($"{ingredientName}�� ImageBefore �Ǵ� ImageAfter�� ����!");
                continue;
            }

            GameObject imageBefore = imageBeforeTransform.gameObject;
            GameObject imageAfter = imageAfterTransform.gameObject;

            imageBefore.SetActive(true);
            imageAfter.SetActive(false);

            // ��ư Ŭ�� �� �̹��� ���� �̺�Ʈ �߰�
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnIngredientButtonClick(button, ingredientName, imageBefore, imageAfter));
        }

        // ���� ��� ��ư ����
        foreach (Transform child in shelfPanel.transform)
        {
            Button button = child.GetComponent<Button>();
            if (button == null) continue; // Button�� ������ ����

            string ingredientName = button.name;
            bool hasIngredient = inventoryManager.HasIngredient(ingredientName);

            button.interactable = hasIngredient;
            button.gameObject.SetActive(hasIngredient); // ������ ��Ḹ Ȱ��ȭ

            // ImageBefore & ImageAfter ����
            Transform imageBeforeTransform = child.Find("Imagebf");
            Transform imageAfterTransform = child.Find("Imageaft");

            if (imageBeforeTransform == null || imageAfterTransform == null)
            {
                Debug.LogError($"{ingredientName}�� ImageBefore �Ǵ� ImageAfter�� ����!");
                continue;
            }

            GameObject imageBefore = imageBeforeTransform.gameObject;
            GameObject imageAfter = imageAfterTransform.gameObject;

            imageBefore.SetActive(true);
            imageAfter.SetActive(false);

            // ��ư Ŭ�� �� �̹��� ���� �̺�Ʈ �߰�
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnIngredientButtonClick(button, ingredientName, imageBefore, imageAfter));
        }
    }

    // ��� Ŭ�� �� �̹��� ���� �� ����/���� ���� ����
    public void OnIngredientButtonClick(Button button, string ingredientName, GameObject imageBefore, GameObject imageAfter)
    {
        if (selectedIngredients.Contains(ingredientName))
        {
            // ���� ����
            selectedIngredients.Remove(ingredientName);
            imageBefore.SetActive(true);
            imageAfter.SetActive(false);
        }
        else
        {
            // ��� ����
            selectedIngredients.Add(ingredientName);
            imageBefore.SetActive(false);
            imageAfter.SetActive(true);
        }

        // ���õ� ��ᰡ �ϳ��� ������ �Ϸ� ��ư Ȱ��ȭ
        finishButton.gameObject.SetActive(selectedIngredients.Count > 0);
    }

    // ������ ��ᰡ �����ǿ� ��ġ�ϴ��� ���� �� MixingPanel�� �̵�
    private void FinishIngredientSelection()
    {
        Recipe selectedRecipe = bakingStartManager.GetSelectedRecipe();
        if (selectedRecipe == null)
        {
            Debug.LogError("���õ� �����ǰ� �����ϴ�!");
            return;
        }

        if (VerifyIngredients(selectedRecipe.ingredients))
        {
            Debug.Log("��ᰡ �����ǿ� ��ġ�մϴ�!");
        }
        else
        {
            Debug.Log("������ ��ᰡ �����ǿ� ��ġ���� �ʽ��ϴ�.");
        }

        ingredientSelectionPanel.SetActive(false);
        mixingPanel.SetActive(true); // MixingPanel Ȱ��ȭ
    }

    // ������ ��ᰡ �����ǿ� ��ġ�ϴ��� Ȯ��
    private bool VerifyIngredients(List<string> requiredIngredients)
    {
        foreach (string ingredient in requiredIngredients)
        {
            if (!selectedIngredients.Contains(ingredient))
            {
                return false; // �ʿ��� ��ᰡ ������ false ��ȯ
            }
        }
        return true;
    }
}
