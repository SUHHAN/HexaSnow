using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScrollbarManager : MonoBehaviour
{
    public static ScrollbarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ScrollbarManager>();
            }
            return _instance;
        }
    }

    public GameObject itemPrefab; // 아이템 Prefab
    public Transform content; // Content 오브젝트

    [SerializeField] private TextMeshProUGUI UseScText;

    private static ScrollbarManager _instance;

    private int FinalSc = 0; // 초기 점수
    private int SumSc;

    void Start()
    {
        AddItems(10); // 10개의 아이템 추가
        CalculateNum(); // 초기 SumSc 계산
    }

    public void AddItems(int itemCount)
    {
        int Ingredient_Index = 1;

        for (int i = 1; i <= itemCount; i++)
        {
            GameObject item = Instantiate(itemPrefab, content);

            // 카드 스프라이트의 인덱스 지정 변수
            Ingredient_h ingredient = item.GetComponent<Ingredient_h>();
            int ingredientID = Ingredient_Index++;
            ingredient.SetIngredientID(ingredientID);

            item.GetComponentInChildren<TextMeshProUGUI>().text = $"{i} | 안녕하십니까";
        }
    }

    public void SetFinalScore(int score)
    {
        FinalSc = score;
        CalculateNum();
    }

    private void CalculateNum()
    {
        SumSc = FinalSc;
        UpdateScoreDisplay();
    }

    public bool TryPurchase(int requiredScore)
    {
        if (SumSc >= requiredScore)
        {
            SumSc -= requiredScore;
            UpdateScoreDisplay();
            return true;
        }
        else
        {
            UpdateScoreDisplay();
            return false;
        }
    }

    public void InitAllNum()
    {
        // 모든 아이템의 currentNum을 초기화
        foreach (Transform child in content)
        {
            Ingredient_h ingredient = child.GetComponent<Ingredient_h>();
            if (ingredient != null)
            {
                ingredient.currentNum = 0;
                ingredient.InitNum(); // UI 갱신
            }
        }

        // 사용 가능 점수를 초기 상태로 복구
        CalculateNum();

        Debug.Log("모든 currentNum이 초기화되고 점수가 복구되었습니다.");
    }

    private void UpdateScoreDisplay()
    {
        UseScText.text = $"사용 가능 포인트 : {SumSc}";
    }
}
