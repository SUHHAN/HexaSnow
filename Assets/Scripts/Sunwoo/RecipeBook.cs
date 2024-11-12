using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public List<Recipe> recipes;

    void Start()
    {
        // ���� ������ ������ �ʱ�ȭ
        recipes = new List<Recipe>
        {
            new Recipe("Madeleine", true),
            new Recipe("Muffin", false),
            // ������ ������ �߰�
        };
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
    public string recipeName;
    public bool canBake;

    public Recipe(string name, bool bakeStatus)
    {
        recipeName = name;
        canBake = bakeStatus;
    }
}
