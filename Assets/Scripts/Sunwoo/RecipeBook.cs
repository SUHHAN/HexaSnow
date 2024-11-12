using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public List<Recipe> recipes;

    void Start()
    {
        // 예시 레시피 데이터 초기화
        recipes = new List<Recipe>
        {
            new Recipe("Madeleine", true),
            new Recipe("Muffin", false),
            // 나머지 레시피 추가
        };
    }

    // 레시피 이름으로 레시피 검색
    public Recipe GetRecipeByName(string name)
    {
        return recipes.Find(r => r.recipeName == name);
    }
}

// 레시피 정보 구조체
[System.Serializable]
public class Recipe
{
    public string recipeName;
    public bool canBake;

    public Recipe(string name, bool bakeStatus)
    {
        recipeName = name;
        canBake = bakeStatus;
    }
}
