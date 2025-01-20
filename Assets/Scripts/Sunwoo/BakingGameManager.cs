using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//enum���� �ܰ� ����
public enum GameState
{
    Start, // ���� �ܰ�
    IngredientSelection, // ��� ���� �ܰ�
    Mixing, // ���� �̴ϰ��� �ܰ�
    Oven, // ���� �̴ϰ��� �ܰ�
    Topping, // ���� �ܰ�
    Result // ��� �ܰ�
}

public class BakingGameManager : MonoBehaviour
{
    public GameState currentState; // ���� ���� ����
    public GameObject startPanel; // ó�� ���� �г�
    public GameObject recipeSelectionPopup; // ���� ���� ���� �˾�(StartPanel ����)
    public GameObject nextButton; // '����' ��ư

    public GameObject ingredientSelectionPanel; // ��� ���� �г�
    public GameObject addIngredientsButton; // '��� ���' ��ư
    public GameObject startIngredientPanel; // StartIngredient �г�
    public GameObject addingIngredientPanel; // AddingIngredient �г�
    public GameObject finishIngredientButton; // '��� ��� ��' ��ư
    private Recipe selectedRecipe = null; // ���õ� ������

    public GameObject mixingPanel; // ���� �̴ϰ��� �г�
    public GameObject ovenPanel; // ���� �̴ϰ��� �г�
    public GameObject toppingPanel; // ���� ��� ���� �г�
    public GameObject resultPanel; // ��� �г�

    private bool isCooking = false; // ���� �� ����
    public RecipeBook recipeBook; // ������ ���� ��ũ��Ʈ
    public UIManager uiManager; // UI ���� ��ũ��Ʈ

    public MixingGameManager mixingGameManager; // MixingGameManager ����

    void Start()
    {
        SetGameState(GameState.Start); // �ʱ� ���¸� ���� �ܰ�� ����
        isCooking = false; // ó������ ���� ���� �ƴ�
        recipeSelectionPopup.SetActive(false); // ó������ ���� ���� �˾� ��Ȱ��ȭ
        ingredientSelectionPanel.SetActive(false); // ���� ���� �� ��� ���� �г� ��Ȱ��ȭ
        nextButton.SetActive(false); // '����' ��ư ��Ȱ��ȭ
        addIngredientsButton.SetActive(false); // '��� ���' ��ư ��Ȱ��ȭ
        addingIngredientPanel.SetActive(false); // �ʱ⿡�� AddingIngredient �г��� ��Ȱ��ȭ
    }

    // ���� ���¸� �����ϴ� �޼���. �Ű������� ���� ���¸� ���� currentState �����ϰ� ui�� ������
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        UpdateUI();
    }

    // currentState�� ���� �� ���¿� �ش��ϴ� �гθ� Ȱ��ȭ�ϰ� �������� ����
    void UpdateUI()
    {
        startPanel.SetActive(currentState == GameState.Start);
        ingredientSelectionPanel.SetActive(currentState == GameState.IngredientSelection);
        if (currentState == GameState.IngredientSelection)
        {
            // IngredientSelectionPanel Ȱ��ȭ, '��� ���' ��ư ǥ��
            startIngredientPanel.SetActive(true);
            addingIngredientPanel.SetActive(false);
        }
        mixingPanel.SetActive(currentState == GameState.Mixing);
        ovenPanel.SetActive(currentState == GameState.Oven);
        toppingPanel.SetActive(currentState == GameState.Topping);
        resultPanel.SetActive(currentState == GameState.Result);

        recipeSelectionPopup.SetActive(false);
    }

    // '����' ��ư Ŭ�� �� ȣ��
    public void OnStartButtonClick()
    {
        if (!isCooking)
        {
            isCooking = true;
            // '����' ��ư�� �ִ� StartPanel�� ��ư�� ����� ���� ���� ���� �˾��� Ȱ��ȭ
            startPanel.transform.Find("StartButton").gameObject.SetActive(false); // ���� ��ư ��Ȱ��ȭ
            recipeSelectionPopup.SetActive(true); // ���� ���� ���� �˾� Ȱ��ȭ
        }
    }

    // ������ ���� �� ȣ��
    public void OnRecipeSelected(string recipeName)
    {
        Recipe recipe = recipeBook.GetRecipeByName(recipeName);

        if (recipe != null && recipe.canBake)
        {
            uiManager.HighlightRecipeButton(recipeName); // ���� ���� ���
            selectedRecipe = (uiManager.GetCurrentlySelectedButton() != null) ? recipe : null; // ��ư ���� ���� �� ���õ� �����ǵ� ����
            nextButton.SetActive(selectedRecipe != null); // ���õ� �����ǰ� ���� ���� '����' ��ư Ȱ��ȭ
        }
        else
        {
            // �رݵ��� ���� �������� ��� �޽��� ǥ��
            uiManager.ShowMessage("�رݵ��� ���� �������Դϴ�!");
        }
    }

    // '����' ��ư Ŭ�� �� ȣ��
    public void OnNextButtonClick()
    {
        if (selectedRecipe != null)
        {
            SetGameState(GameState.IngredientSelection); // IngredientSelectionPanel�� ��ȯ
            recipeSelectionPopup.SetActive(false); // ������ ���� �˾� ��Ȱ��ȭ
            nextButton.SetActive(false); // '����' ��ư ��Ȱ��ȭ
            startIngredientPanel.SetActive(true);
            addIngredientsButton.SetActive(true); // AddIngredient ��ư Ȱ��ȭ
        }
    }

    // '��� ���' ��ư ������ �� ��� �����.
    public void OnAddIngredientsClick()
    {
        startIngredientPanel.SetActive(false); // StartIngredient �г� ��Ȱ��ȭ
        addingIngredientPanel.SetActive(true); // AddingIngredient �г� Ȱ��ȭ
        uiManager.GenerateIngredientButtons(); // ������ ��ῡ ���� ��ư ����
    }

    // �޴� ���� �� ��� ���� �ܰ�� ��ȯ�ϴ� �޼���
    // �޴� ������ ������ �� ȣ���Ұ���
    public void OnStartComplete()
    {
        SetGameState(GameState.IngredientSelection);
        uiManager.GenerateIngredientButtons();
    }

    // '��� ���� �Ϸ�' ��ư Ŭ�� �� ȣ��
    public void OnFinishIngredientSelection()
    {
        // ������ ��ᰡ �����ǿ� �´��� ����
        if (VerifyIngredients())
        {
            Debug.Log("��ᰡ �����ǿ� ��ġ�մϴ�!");
            // ���� �ܰ�� �̵��ϴ� �ڵ� �߰� ����
        }
        else
        {
            Debug.Log("������ ��ᰡ �����ǿ� ��ġ���� �ʽ��ϴ�.");
        }
        if (currentState == GameState.IngredientSelection)
        {
            SetGameState(GameState.Mixing); // Mixing �ܰ�� ��ȯ
            mixingGameManager.ActivateMixingPanel(); // MixingGameManager�� ActivateMixingPanel ȣ��
        }
    }

    // ���õ� ��ᰡ �����ǿ� ��ġ�ϴ��� ����
    bool VerifyIngredients()
    {
        if (selectedRecipe == null) return false;

        List<string> requiredIngredients = selectedRecipe.ingredients;
        List<string> selectedIngredientNames = uiManager.GetSelectedIngredientNames();

        foreach (string ingredient in requiredIngredients)
        {
            if (!selectedIngredientNames.Contains(ingredient))
            {
                return false; // �ʿ��� ��ᰡ ���õ��� ����
            }
        }
        return true;
    }

    // ���� ���� ������ ȣ��
    public void OnMixingComplete()
    {
        SetGameState(GameState.Oven);
    }

    // ���� ���� ������ ȣ��
    public void OnOvenComplete()
    {
        SetGameState(GameState.Topping);
    }

    // ���� ������ ȣ��
    public void OnToppingComplete()
    {
        SetGameState(GameState.Result);
    }
}
