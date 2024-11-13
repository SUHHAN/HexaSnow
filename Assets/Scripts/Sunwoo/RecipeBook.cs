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
            new Recipe("Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "Baking Powder" }),
            new Recipe("Choco Muffin", new List<string> { "Butter", "Egg", "Sugar", "Baking Powder", "Flour", "Milk", "Cocoa Powder", "Chocolate Chips (Topping)" }),
            new Recipe("Blueberry Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "Baking Powder", "Milk", "Blueberry (Topping)" }),
            new Recipe("Cookie", new List<string> { "Butter", "Sugar Powder", "Egg", "Flour", "Almond Powder" }),
            new Recipe("Pound Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar" }), // 마들렌과 같은 재료
            new Recipe("Financier", new List<string> { "Egg Whites", "Almond Powder", "Flour", "Honey", "Sugar", "Browned Butter" }),
            new Recipe("Basque Cheesecake", new List<string> { "Cream Cheese", "Sugar", "Egg", "Heavy Cream" }),
            new Recipe("Scone", new List<string> { "Flour", "Sugar", "Baking Powder", "Butter", "Egg", "Milk", "Heavy Cream" }),
            new Recipe("Castella", new List<string> { "Meringue", "Egg Yolk", "Sugar", "Flour" }),
            new Recipe("Tart", new List<string> { "Cream Cheese", "Condensed Milk", "Heavy Cream", "Sugar", "Flour", "Almond Powder", "Sugar Powder", "Butter", "Egg" })
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
