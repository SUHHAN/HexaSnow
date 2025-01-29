using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public Image slotImage; // 슬롯에 표시될 재료 이미지
    private string ingredientName; // 선택된 재료 이름
    private bool isFilled = false; // 슬롯이 채워졌는지 여부

    // 슬롯 초기화
    public void InitializeSlot()
    {
        slotImage.sprite = null;
        slotImage.color = new Color(1, 1, 1, 0); // 슬롯을 빈 상태로 유지
        ingredientName = null;
        isFilled = false;
    }

    // 재료 슬롯에 추가
    public void AddIngredient(Sprite ingredientSprite, string name)
    {
        slotImage.sprite = ingredientSprite;
        slotImage.color = Color.white; // 이미지가 보이도록 불투명하게 설정
        ingredientName = name;
        isFilled = true;
    }

    // 슬롯 비우기
    public void RemoveIngredient()
    {
        InitializeSlot();
    }

    // 슬롯이 비어있는지 여부를 확인하는 메서드
    public bool IsFilled()
    {
        return isFilled;
    }

    public string GetIngredientName()
    {
        return ingredientName;
    }
}
