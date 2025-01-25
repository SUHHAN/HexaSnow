using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ingredient_h : MonoBehaviour
{
    public int Ingredient_ID;
    public int currentNum = 0;

    private const int ScoreMultiplier = 3; // 1개 구매 당 배점

    void Start()
    {
        InitNum();
    }

    public void SetIngredientID(int id)
    {
        this.Ingredient_ID = id;
    }

    public void InitNum()
    {
        Transform panel = this.transform.Find("numPanel");
        if (panel != null)
        {
            TextMeshProUGUI textComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"{currentNum}";
            }
        }
    }

    public void GetIngredient()
    {
        // 필요한 배점 계산
        int requiredScore = ScoreMultiplier;

        // ScrollbarManager에서 점수를 확인 및 차감
        if (ScrollbarManager.Instance.TryPurchase(requiredScore))
        {
            currentNum++;
            Transform panel = this.transform.Find("numPanel");
            if (panel != null)
            {
                TextMeshProUGUI textComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = $"{currentNum}";
                }
            }
        }
        else
        {
            Debug.Log("더 이상 구매할 수 없습니다.");
        }
    }
}
