using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public List<Recipe> recipes;
    private int currentDate;

    void Start()
    {
        recipes = new List<Recipe>
        {
            new Recipe("Madeleine", new List<string> { "Butter", "Egg", "Flour", "Sugar" }, true),
            new Recipe("Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "BakingPowder", "Milk" }, false),
            new Recipe("Cookie", new List<string> { "Butter", "Sugar", "SugarPowder", "EggYellow", "Flour", "AlmondPowder" }, true),
            new Recipe("PoundCake", new List<string> { "Butter", "Egg", "Flour", "Sugar" }, false),
            new Recipe("BasqueCheesecake", new List<string> { "CreamCheese", "Sugar", "Egg", "WhCream" }, false),
            new Recipe("Financier", new List<string> { "EggWhites", "AlmondPowder", "Flour", "Honey", "Sugar", "BrownedButter" }, false),
            new Recipe("Scone", new List<string> { "Flour", "Sugar", "BakingPowder", "Butter", "Egg", "Milk", "WhCream" }, false),
            new Recipe("Tart", new List<string> { "CreamCheese", "ConMilk", "WhCream", "Sugar", "Flour", "AlmondPowder", "SugarPowder", "Butter", "Egg" }, false),
            new Recipe("Macaroon", new List<string> { "Butter", "Sugar", "EggWhites", "AlmondPowder", "SugarPowder", "WhCream" }, false),
            new Recipe("SliceCake", new List<string> { "Butter", "Egg", "Flour", "Sugar", "Milk" }, false),
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

        GameData dateGD = DataManager.Instance.LoadGameData();
        currentDate = dateGD.date;
        Debug.Log($"현재 날짜: {currentDate}");

        // 레시피 해금 로직 수정
        if (currentDate >= 1) { UnlockRecipe("Madeleine"); UnlockRecipe("Cookie"); }  // Day 1
        if (currentDate >= 2) { UnlockRecipe("Muffin"); UnlockRecipe("PoundCake"); } // Day 2
        if (currentDate >= 3) { UnlockRecipe("Financier"); UnlockRecipe("BasqueCheesecake"); } // Day 3
        if (currentDate >= 4) { UnlockRecipe("Tart"); UnlockRecipe("Scone"); } // Day 4
        if (currentDate >= 5) { UnlockRecipe("Macaroon"); UnlockRecipe("Doughnut"); UnlockRecipe("SliceCake"); } // Day 5

        Debug.Log("레시피 해금 완료!");
        DebugUnlockedRecipes(); // 해금된 레시피 디버깅 출력
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

    // 디버깅용 해금된 레시피 목록 출력
    private void DebugUnlockedRecipes()
    {
        Debug.Log("==== 현재 해금된 레시피 목록 ====");
        foreach (Recipe recipe in recipes)
        {
            if (recipe.canBake)
            {
                Debug.Log($"- {recipe.recipeName} (해금됨)");
            }
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
    public bool canBake; // 해금 여부

    public Recipe(string name, List<string> ingr, bool bakeStatus = false)
    {
        recipeName = name;
        ingredients = ingr;
        canBake = bakeStatus;
    }
}
