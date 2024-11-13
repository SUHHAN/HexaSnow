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
            new Recipe("Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "Baking Powder" }),
            new Recipe("Choco Muffin", new List<string> { "Butter", "Egg", "Sugar", "Baking Powder", "Flour", "Milk", "Cocoa Powder", "Chocolate Chips (Topping)" }),
            new Recipe("Blueberry Muffin", new List<string> { "Butter", "Egg", "Sugar", "Flour", "Baking Powder", "Milk", "Blueberry (Topping)" }),
            new Recipe("Cookie", new List<string> { "Butter", "Sugar Powder", "Egg", "Flour", "Almond Powder" }),
            new Recipe("Pound Cake", new List<string> { "Butter", "Egg", "Flour", "Sugar" }), // ���鷻�� ���� ���
            new Recipe("Financier", new List<string> { "Egg Whites", "Almond Powder", "Flour", "Honey", "Sugar", "Browned Butter" }),
            new Recipe("Basque Cheesecake", new List<string> { "Cream Cheese", "Sugar", "Egg", "Heavy Cream" }),
            new Recipe("Scone", new List<string> { "Flour", "Sugar", "Baking Powder", "Butter", "Egg", "Milk", "Heavy Cream" }),
            new Recipe("Castella", new List<string> { "Meringue", "Egg Yolk", "Sugar", "Flour" }),
            new Recipe("Tart", new List<string> { "Cream Cheese", "Condensed Milk", "Heavy Cream", "Sugar", "Flour", "Almond Powder", "Sugar Powder", "Butter", "Egg" })
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
