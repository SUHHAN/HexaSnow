using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeData", menuName = "Baking/Recipe")]
public class RecipeData : ScriptableObject
{
    public string dessertName;
    public string[] ingredients;
    public bool isUnlocked;
}
