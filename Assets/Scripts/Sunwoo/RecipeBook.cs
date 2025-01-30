using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public List<Recipe> recipes;

    void Start()
    {
        // 레시피 데이터 초기화
        recipes = new List<Recipe>
        {
            new Recipe("Madeleine", new List<string> { "Butter", "Egg", "Flour", "Sugar" }, true),
            new Recipe("Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "BakingPowder" }),
            new Recipe("Cookie", new List<string> { "Butter", "Sugar", "SugarPowder", "EggYellow", "Flour", "AlmondPowder" }),
            new Recipe("Pound Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar" }), // 마들렌과 같은 재료
            new Recipe("Financier", new List<string> { "EggWhites", "AlmondPowder", "Flour", "Honey", "Sugar", "BrownedButter" }),
            new Recipe("Basque Cheesecake", new List<string> { "CreamCheese", "Sugar", "Egg", "WhCream" }),
            new Recipe("Scone", new List<string> { "Flour", "Sugar", "BakingPowder", "Butter", "Egg", "Milk", "WhCream" }),
            new Recipe("Tart", new List<string> { "CreamCheese", "ConMilk", "WhCream", "Sugar", "Flour", "AlmondPowder", "SugarPowder", "Butter", "Egg" }),
            new Recipe("Slice Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar", "Milk" }),
            new Recipe("Doughnut", new List<string> { "Butter", "Egg", "Flour", "Sugar", "Milk" })
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
    public string recipeName; // 레시피 이름
    public List<string> ingredients; // 재료 목록
    public bool canBake; // 해금 여부. 기본값은 해금되지 않음으로

    public Recipe(string name, List<string> ingr, bool bakeStatus = false)
    {
        recipeName = name;
        ingredients = ingr;
        canBake = bakeStatus;
    }
}
