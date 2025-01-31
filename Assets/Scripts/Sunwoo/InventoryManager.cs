using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();

    void Start()
    {
        // ���� �ʱ� ���
        AddIngredient("Butter", 3);
        AddIngredient("Egg", 5);
        AddIngredient("Flour", 4);
        AddIngredient("Sugar", 2);
        AddIngredient("Milk", 3);
        AddIngredient("CreamCheese", 2);
        AddIngredient("Choco", 1);
        AddIngredient("Lemon", 1);

        // ���� ��ϵ� ��� ��� ���
        foreach (var ingredient in ingredientCounts)
        {
            Debug.Log($"������ ���: {ingredient.Key}, ����: {ingredient.Value}");
        }
    }

    // ��� �߰�
    public void AddIngredient(string ingredient, int count = 1)
    {
        if (ingredientCounts.ContainsKey(ingredient))
        {
            ingredientCounts[ingredient] += count;
        }
        else
        {
            ingredientCounts[ingredient] = count;
        }
    }

    // ��� ���� ���� (���)
    public bool UseIngredient(string ingredient)
    {
        if (ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0)
        {
            ingredientCounts[ingredient]--;
            return true;
        }
        return false;
    }

    // ��� ���� ����
    public bool HasIngredient(string ingredient)
    {
        return ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0;
    }
}