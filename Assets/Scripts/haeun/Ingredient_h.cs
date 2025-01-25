using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ingredient_h : MonoBehaviour
{

    public int Ingredient_ID;
    public int currentNum = 0;
    private int ScoreMultiplier = 3; // 1개 구매 당 배점



    void Start()
    {
        InitNum();
        
    }
    public void SetPrice(int price) {
        this.ScoreMultiplier = price;
    }

    public void SetIngredientID(int id)
    {
        this.Ingredient_ID = id;
    }

    public void InitNum()
    {
        Transform panel = this.transform.Find("numPanel");
        Transform panel2 = this.transform.Find("pricePanel");
        
        if (panel != null)
        {
            TextMeshProUGUI textComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = $"{currentNum}";
            }
        }

        if (panel2 != null)
        {
            TextMeshProUGUI textComponent1 = panel2.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent1 != null)
            {
                textComponent1.text = $"{ScoreMultiplier}";
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

            // ScrollbarManager의 ingre_Num 리스트 업데이트
            ScrollbarManager.Instance.ingre_Num[Ingredient_ID] = currentNum;
        }
        else
        {
            Debug.Log("더 이상 구매할 수 없습니다.");
        }
    }
}
