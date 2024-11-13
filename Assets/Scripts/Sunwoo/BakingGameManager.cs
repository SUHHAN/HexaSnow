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
    public GameObject ingredientSelectionPanel; // ��� ���� �г�
    public GameObject mixingPanel; // ���� �̴ϰ��� �г�
    public GameObject ovenPanel; // ���� �̴ϰ��� �г�
    public GameObject toppingPanel; // ���� ��� ���� �г�
    public GameObject resultPanel; // ��� �г�

    private bool isCooking = false; // ���� �� ����
    public RecipeBook recipeBook; // ������ ���� ��ũ��Ʈ
    public UIManager uiManager; // UI ���� ��ũ��Ʈ

    public GameObject recipeSelectionPopup; // ���� ���� ���� �˾�(StartPanel ����)


    void Start()
    {
        SetGameState(GameState.Start); // �ʱ� ���¸� ���� �ܰ�� ����
        isCooking = false; // ó������ ���� ���� �ƴ�
        recipeSelectionPopup.SetActive(false); // ó������ ���� ���� �˾� ��Ȱ��ȭ
        ingredientSelectionPanel.SetActive(false); // ���� ���� �� ��� ���� �г� ��Ȱ��ȭ
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
        Recipe selectedRecipe = recipeBook.GetRecipeByName(recipeName);

        if (selectedRecipe != null)
        {
            if (selectedRecipe.canBake)
            {
                // �رݵ� �������� ���
                SetGameState(GameState.IngredientSelection); // IngredientSelectionPanel�� ��ȯ
                recipeSelectionPopup.SetActive(false); // ������ ���� �˾� ��Ȱ��ȭ
            }
            else
            {
                // �رݵ��� ���� �������� ���
                uiManager.ShowMessage("not yet");
            }
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
