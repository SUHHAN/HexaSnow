using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public List<Recipe> recipes;

    void Start()
    {
        // ������ ������ �ʱ�ȭ
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
            Debug.LogError("DataManager �Ǵ� GameData�� ã�� �� �����ϴ�!");
            return;
        }

        int currentDate = DataManager.Instance.gameData.date;
        Debug.Log($"���� ��¥: {currentDate}");

        if (currentDate >= 1) UnlockRecipe("Madeleine");               // Day 1
        if (currentDate >= 2) { UnlockRecipe("Muffin"); UnlockRecipe("Cookie"); }  // Day 2
        if (currentDate >= 3) { UnlockRecipe("Pound Cake"); UnlockRecipe("Basque Cheesecake"); }  // Day 3
        if (currentDate >= 4) { UnlockRecipe("Financier"); UnlockRecipe("Scone"); }  // Day 4
        if (currentDate >= 5) { UnlockRecipe("Tart"); UnlockRecipe("Slice Cake"); }  // Day 5
        if (currentDate >= 6) UnlockRecipe("Doughnut");               // Day 6

        Debug.Log("������ �ر� �Ϸ�!");
    }

    private void UnlockRecipe(string recipeName)
    {
        Recipe recipe = recipes.Find(r => r.recipeName == recipeName);
        if (recipe != null)
        {
            recipe.canBake = true;
            Debug.Log($"{recipeName} �رݵ�!");
        }
        else
        {
            Debug.LogError($"{recipeName} �����Ǹ� ã�� �� �����ϴ�!");
        }
    }

    // ������ �̸����� ������ �˻�
    public Recipe GetRecipeByName(string name)
    {
        return recipes.Find(r => r.recipeName == name);
    }
}

// ������ ���� ����ü
[System.Serializable]
public class Recipe
{
    public string recipeName; // ������ �̸�
    public List<string> ingredients; // ��� ���
    public bool canBake; // �ر� ����. �⺻���� �رݵ��� ��������

    public Recipe(string name, List<string> ingr, bool bakeStatus = false)
    {
        recipeName = name;
        ingredients = ingr;
        canBake = bakeStatus;
    }
}
