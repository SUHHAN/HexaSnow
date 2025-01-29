using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> ownedIngredients = new List<string>(); // ������ ��� ���

    void Start()
    {
        Debug.Log("�κ��丮 �ʱ�ȭ ����"); // ������
        ownedIngredients.Add("Butter");
        ownedIngredients.Add("Egg");
        ownedIngredients.Add("Flour");
        ownedIngredients.Add("Sugar");
        ownedIngredients.Add("Milk");
        ownedIngredients.Add("CreamCheese");

        // ���� ��ϵ� ��� ��� ���
        foreach (string ingredient in ownedIngredients)
        {
            Debug.Log($"������ ���: {ingredient}");
        }
    }

    // ���� ���� Ȯ�� �޼���
    public bool HasIngredient(string ingredient)
    {
        return ownedIngredients.Contains(ingredient);
    }
}