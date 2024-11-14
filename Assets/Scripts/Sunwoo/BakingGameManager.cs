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
    public GameObject bowl; // Bowl ������Ʈ
    public GameObject addIngredientsButton; // '��� ���' ��ư

    public GameObject mixingPanel; // ���� �̴ϰ��� �г�
    public GameObject ovenPanel; // ���� �̴ϰ��� �г�
    public GameObject toppingPanel; // ���� ��� ���� �г�
    public GameObject resultPanel; // ��� �г�

    private bool isCooking = false; // ���� �� ����
    public RecipeBook recipeBook; // ������ ���� ��ũ��Ʈ
    public UIManager uiManager; // UI ���� ��ũ��Ʈ
    private Recipe selectedRecipe = null; // ���õ� �����Ǹ� �����ϴ� ����

    void Start()
    {
        SetGameState(GameState.Start); // �ʱ� ���¸� ���� �ܰ�� ����
        isCooking = false; // ó������ ���� ���� �ƴ�
        recipeSelectionPopup.SetActive(false); // ó������ ���� ���� �˾� ��Ȱ��ȭ
        ingredientSelectionPanel.SetActive(false); // ���� ���� �� ��� ���� �г� ��Ȱ��ȭ
        nextButton.SetActive(false); // '����' ��ư ��Ȱ��ȭ
        bowl.SetActive(false); // Bowl ������Ʈ ��Ȱ��ȭ
        addIngredientsButton.SetActive(false); // '��� ���' ��ư ��Ȱ��ȭ
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
            // IngredientSelectionPanel Ȱ��ȭ �� Bowl�� '��� ���' ��ư ǥ��
            bowl.SetActive(true);
            addIngredientsButton.SetActive(true);
        }
        else
        {
            // �ٸ� ������ ���� ��Ȱ��ȭ
            bowl.SetActive(false);
            addIngredientsButton.SetActive(false);
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
            selectedRecipe = recipe; // ���õ� ������ ����
            uiManager.HighlightRecipeButton(recipeName); // ���õ� ������ ���� ǥ�� (üũ �Ǵ� ���� ����)
            nextButton.SetActive(true); // '����' ��ư Ȱ��ȭ
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
        }
    }

    // �޴� ���� �� ��� ���� �ܰ�� ��ȯ�ϴ� �޼���
    // �޴� ������ ������ �� ȣ���Ұ���
    public void OnStartComplete()
    {
        SetGameState(GameState.IngredientSelection);
    }

    // ��� ���� ������ �� ȣ��
    public void OnIngredientSelectionComplete()
    {
        SetGameState(GameState.Mixing);
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
