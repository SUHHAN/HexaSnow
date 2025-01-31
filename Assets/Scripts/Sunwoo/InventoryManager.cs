using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private Dictionary<string, int> ingredientCounts = new Dictionary<string, int>();

    void Start()
    {
        // 예제 초기 재료
        AddIngredient("Butter", 3);
        AddIngredient("Egg", 5);
        AddIngredient("Flour", 4);
        AddIngredient("Sugar", 2);
        AddIngredient("Milk", 3);
        AddIngredient("CreamCheese", 2);
        AddIngredient("Choco", 1);
        AddIngredient("Lemon", 1);

        // 현재 등록된 재료 목록 출력
        foreach (var ingredient in ingredientCounts)
        {
            Debug.Log($"소지한 재료: {ingredient.Key}, 개수: {ingredient.Value}");
        }
    }

    // 재료 추가
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

    // 재료 개수 감소 (사용)
    public bool UseIngredient(string ingredient)
    {
        if (ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0)
        {
            ingredientCounts[ingredient]--;
            return true;
        }
        return false;
    }

    // 재료 소지 여부
    public bool HasIngredient(string ingredient)
    {
        return ingredientCounts.ContainsKey(ingredient) && ingredientCounts[ingredient] > 0;
    }
}