using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BakingStartGameManager : MonoBehaviour
{
    public GameObject startButton; // '���� ����' ��ư
    public GameObject recipePopup; // ���� ���� ���� UI
    public List<Button> recipeButtons; // ���� ��ư ����Ʈ
    public GameObject nextButton; // '����' ��ư. IngredientScene���� �Ѿ�� ��ư
    public TextMeshProUGUI unlockMessage; // '�رݵ��� �ʾҽ��ϴ�!' �ȳ� UI

    public string selectedRecipe = null; // ���õ� ������ �̸�

    // �����ǿ� �ر� ���¸� �����ϴ� ��ųʸ�
    private Dictionary<string, (List<string> ingredients, bool isUnlocked)> recipes = new Dictionary<string, (List<string>, bool)>
    {
        { "Madeleine", (new List<string>{ "Butter", "Egg", "Flour", "Sugar" }, true) }, // ���鷻�� �⺻������ �رݵ�
        { "Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder" }, false) },
        { "Choco Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Cocoa Powder", "Chocolate Chips" }, false) },
        { "Blueberry Muffin", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Blueberry" }, false) },
        { "Cookie", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Almond Powder", "Sugar Powder" }, false) },
        { "Pound Cake", (new List<string>{ "Butter", "Egg", "Flour", "Sugar" }, false) },
        { "Financier", (new List<string>{ "Browned Butter", "Egg Whites", "Flour", "Sugar", "Almond Powder", "Honey" }, false) },
        { "Basque Cheesecake", (new List<string>{ "Egg", "Sugar", "Cream Cheese", "Heavy Cream" }, false) },
        { "Scone", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Baking Powder", "Milk", "Heavy Cream" }, false) },
        { "Tart", (new List<string>{ "Butter", "Egg", "Flour", "Sugar", "Cream Cheese", "Condensed Milk", "Heavy Cream", "Almond Powder", "Sugar Powder" }, false) }
    };

    // Start is called before the first frame update
    void Start()
    {
        recipePopup.SetActive(false); // ������ �� ���� �˾� ��Ȱ��ȭ
        nextButton.SetActive(false); // ������ �� "Next" ��ư ��Ȱ��ȭ
        unlockMessage.gameObject.SetActive(false); // "�رݵ��� �ʾҽ��ϴ�!" �޽��� ��Ȱ��ȭ

        InitializeRecipeButtons(); // ���� ��ư �ʱ�ȭ
    }

    // '�����ϱ�' ��ư ���� ���
    public void OnStartButtonClick()
    {
        startButton.SetActive(false); // "Start" ��ư �����
        recipePopup.SetActive(true); // ���� ���� �˾� Ȱ��ȭ
    }

    private void InitializeRecipeButtons()
    {
        foreach (Button button in recipeButtons) // ��ư ����Ʈ ��ȸ
        {
            // ��ư �ؽ�Ʈ���� ������ �̸� ��������
            string recipeName = button.GetComponentInChildren<TextMeshProUGUI>().text;

            Debug.Log($"Initializing button: {recipeName}"); // ����� �޽��� ���

            if (recipes.ContainsKey(recipeName)) // ������ �����Ϳ� �ش� �̸��� �ִ��� Ȯ��
            {
                bool isUnlocked = recipes[recipeName].isUnlocked; // ������ �ر� ���� Ȯ��
                button.interactable = isUnlocked; // �ر� ���¿� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ ����

                // Ŭ�� �̺�Ʈ ����
                button.onClick.AddListener(() => OnRecipeSelect(recipeName, button));
            }
            else
            {
                Debug.LogWarning($"Recipe not found for button: {recipeName}");
            }
        }
    }

    // ���� ��ư ������ ���
    public void OnRecipeSelect(string recipeName, Button button)
    {
        if (recipes[recipeName].isUnlocked) // ������ �����ǰ� �رݵǾ����� Ȯ��
        {
            selectedRecipe = recipeName; // ���õ� ���� �̸� ����
            nextButton.SetActive(true); // "Next" ��ư Ȱ��ȭ

            // ��ư ���� ����
            ColorBlock colors = button.colors;
            colors.normalColor = Color.gray; // �⺻ ������ ȸ������ ����
            button.colors = colors;
        }
        else
        {
            StartCoroutine(ShowUnlockMessage()); // "�رݵ��� �ʾҽ��ϴ�!" �޽��� ǥ��
        }
    }

    // '����' ��ư ������ ���
    public void OnNextButtonClick()
    {
        BakingGameManager1.Instance.SetSelectedRecipe(selectedRecipe); // GameManager�� ���õ� ���� ����
        SceneManager.LoadScene("IngredientScene"); // IngredientScene���� ��ȯ
    }

    private IEnumerator ShowUnlockMessage()
    {
        unlockMessage.gameObject.SetActive(true); // �޽��� Ȱ��ȭ
        yield return new WaitForSeconds(1f); // 1�� ���� ���
        unlockMessage.gameObject.SetActive(false); // �޽��� ��Ȱ��ȭ
    }
}
