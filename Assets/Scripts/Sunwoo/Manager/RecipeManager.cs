using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ �ر� ���� Ȯ��
public class RecipeManager : MonoBehaviour
{
    public RecipeData[] recipes; // ScriptableObject�� ����
    public GameObject nextButton;
    public GameObject lockedMessage;

    private RecipeData selectedRecipe;

    public void SelectRecipe(int index)
    {
        selectedRecipe = recipes[index];
        if (selectedRecipe.isUnlocked)
        {
            nextButton.SetActive(true); // '����' ��ư Ȱ��ȭ
        }
        else
        {
            ShowLockedMessage(); // �رݵ��� ���� ��� �޽��� ǥ��
        }
    }

    private void ShowLockedMessage()
    {
        lockedMessage.SetActive(true);
        Invoke("HideLockedMessage", 2f); // 2�� �� �����
    }

    private void HideLockedMessage()
    {
        lockedMessage.SetActive(false);
    }
}
