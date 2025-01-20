using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> ownedIngredients = new List<string>(); // 소지한 재료 목록

    void Start()
    {
        // 초기 재료 설정
        ownedIngredients.Add("Butter");
        ownedIngredients.Add("Egg");
        ownedIngredients.Add("Flour");
        ownedIngredients.Add("Sugar");
    }

    // 소지 여부 확인 메서드
    public bool HasIngredient(string ingredient)
    {
        return ownedIngredients.Contains(ingredient);
    }
}