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
            new Recipe("Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "BakingPowder" }, false),
            new Recipe("Cookie", new List<string> { "Butter", "Sugar", "SugarPowder", "EggYellow", "Flour", "AlmondPowder" }, false),
            new Recipe("Pound Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar" }, false),
            new Recipe("Basque Cheesecake", new List<string> { "CreamCheese", "Sugar", "Egg", "WhCream" }, false),
            new Recipe("Financier", new List<string> { "EggWhites", "AlmondPowder", "Flour", "Honey", "Sugar", "BrownedButter" }, false),
            new Recipe("Scone", new List<string> { "Flour", "Sugar", "BakingPowder", "Butter", "Egg", "Milk", "WhCream" }, false),
            new Recipe("Tart", new List<string> { "CreamCheese", "ConMilk", "WhCream", "Sugar", "Flour", "AlmondPowder", "SugarPowder", "Butter", "Egg" }, false),
            new Recipe("Slice Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar", "Milk" }, false),
            new Recipe("Doughnut", new List<string> { "Butter", "Egg", "Flour", "Sugar", "Milk" }, false)
        };

        OpenRecipesByDate();
    }

    private void OpenRecipesByDate()
    {
        if (DataManager.Instance == null || DataManager.Instance.gameData == null)
        {
            Debug.LogError("DataManager 또는 GameData를 찾을 수 없습니다!");
            return;
        }

        int currentDate = DataManager.Instance.gameData.date;
        Debug.Log($"현재 날짜: {currentDate}");

        if (currentDate >= 1) UnlockRecipe("Madeleine");               // Day 1
        if (currentDate >= 2) { UnlockRecipe("Muffin"); UnlockRecipe("Cookie"); }  // Day 2
        if (currentDate >= 3) { UnlockRecipe("Pound Cake"); UnlockRecipe("Basque Cheesecake"); }  // Day 3
        if (currentDate >= 4) { UnlockRecipe("Financier"); UnlockRecipe("Scone"); }  // Day 4
        if (currentDate >= 5) { UnlockRecipe("Tart"); UnlockRecipe("Slice Cake"); }  // Day 5
        if (currentDate >= 6) UnlockRecipe("Doughnut");               // Day 6

        Debug.Log("레시피 해금 완료!");
    }

    private void UnlockRecipe(string recipeName)
    {
        Recipe recipe = recipes.Find(r => r.recipeName == recipeName);
        if (recipe != null)
        {
            recipe.canBake = true;
            Debug.Log($"{recipeName} 해금됨!");
        }
        else
        {
            Debug.LogError($"{recipeName} 레시피를 찾을 수 없습니다!");
        }
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
