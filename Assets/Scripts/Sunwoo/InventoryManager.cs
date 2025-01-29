using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> ownedIngredients = new List<string>(); // 소지한 재료 목록

    void Start()
    {
        Debug.Log("인벤토리 초기화 시작"); // 디버깅용
        ownedIngredients.Add("Butter");
        ownedIngredients.Add("Egg");
        ownedIngredients.Add("Flour");
        ownedIngredients.Add("Sugar");
        ownedIngredients.Add("Milk");
        ownedIngredients.Add("CreamCheese");

        // 현재 등록된 재료 목록 출력
        foreach (string ingredient in ownedIngredients)
        {
            Debug.Log($"소지한 재료: {ingredient}");
        }
    }

    // 소지 여부 확인 메서드
    public bool HasIngredient(string ingredient)
    {
        return ownedIngredients.Contains(ingredient);
    }
}