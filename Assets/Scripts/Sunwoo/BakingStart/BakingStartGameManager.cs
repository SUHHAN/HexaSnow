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
    public GameObject nextButton; // '����' ��ư. IngredientScene���� �Ѿ�� ��ư
    public TextMeshProUGUI unlockMessage; // '�رݵ��� �ʾҽ��ϴ�!' �ȳ� UI
    public Transform recipeButtonContainer; // ���� ��ư ��ġ�� �����̳�
    public GameObject recipeButtonPrefab; // ���� ��ư ������

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
        foreach (var recipe in recipes) // ��� ������ �ݺ�
        {
            GameObject button = Instantiate(recipeButtonPrefab, recipeButtonContainer); // ��ư ������ ����
            button.GetComponentInChildren<Text>().text = recipe.Key; // ��ư �ؽ�Ʈ�� ���� �̸� ����

            bool isUnlocked = recipe.Value.isUnlocked; // ������ �ر� ���� Ȯ��
            button.GetComponent<Button>().interactable = isUnlocked; // �ر� ���¿� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ

            button.GetComponent<Button>().onClick.AddListener(() => OnRecipeSelect(recipe.Key)); // ��ư Ŭ�� �̺�Ʈ ����
        }
    }

    // ���� ��ư ������ ���
    public void OnRecipeSelect(string recipeName)
    {
        if (recipes[recipeName].isUnlocked) // ������ ������ �رݵǾ����� Ȯ��
        {
            selectedRecipe = recipeName; // ���õ� ���� �̸� ����
            nextButton.SetActive(true); // "Next" ��ư Ȱ��ȭ
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
