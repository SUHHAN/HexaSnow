using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ingredient_h : MonoBehaviour
{
<<<<<<< HEAD

    public int Ingredient_ID;
    public int currentNum = 0;
    private int ScoreMultiplier = 3; // 1개 구매 당 배점


=======
    public int Ingredient_ID;
    public int currentNum = 0;

    private const int ScoreMultiplier = 3; // 1개 구매 당 배점
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)

    void Start()
    {
        InitNum();
<<<<<<< HEAD
        
    }
    public void SetPrice(int price) {
        this.ScoreMultiplier = price;
=======
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
    }

    public void SetIngredientID(int id)
    {
        this.Ingredient_ID = id;
    }

    public void InitNum()
    {
        Transform panel = this.transform.Find("numPanel");
<<<<<<< HEAD
        Transform panel2 = this.transform.Find("pricePanel");
        
=======
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
        if (panel != null)
        {
            TextMeshProUGUI textComponent = panel.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
<<<<<<< HEAD
                textComponent.text = $"{currentNum}개";
            }
        }

        if (panel2 != null)
        {
            TextMeshProUGUI textComponent1 = panel2.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent1 != null)
            {
                textComponent1.text = $"{ScoreMultiplier}원";
=======
                textComponent.text = $"{currentNum}";
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
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
<<<<<<< HEAD
                    textComponent.text = $"{currentNum}개";
                }
            }

            // ScrollbarManager의 ingre_Num 리스트 업데이트
            ScrollbarManager.Instance.ingre_Num[Ingredient_ID] = currentNum;
=======
                    textComponent.text = $"{currentNum}";
                }
            }
>>>>>>> parent of e621967 (Merge branch 'main' into jsssun)
        }
        else
        {
            Debug.Log("더 이상 구매할 수 없습니다.");
        }
    }
}
