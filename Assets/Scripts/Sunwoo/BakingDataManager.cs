using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingDataManager : MonoBehaviour
{
    public static BakingDataManager Instance;
    public List<string> selectedIngredients = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearSelectedIngredients()
    {
        selectedIngredients.Clear();
    }
}
