using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCooking : MonoBehaviour
{
    public GameObject recipePanel;

    public void OnStartButtonClicked()
    {
        recipePanel.SetActive(true);
    }
}