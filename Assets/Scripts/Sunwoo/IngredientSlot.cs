using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    public Image slotImage; // ���Կ� ǥ�õ� ��� �̹���
    private string ingredientName; // ���õ� ��� �̸�
    private bool isFilled = false; // ������ ä�������� ����

    // ���� �ʱ�ȭ
    public void InitializeSlot()
    {
        slotImage.sprite = null;
        slotImage.color = new Color(1, 1, 1, 0); // ������ �� ���·� ����
        ingredientName = null;
        isFilled = false;
    }

    // ��� ���Կ� �߰�
    public void AddIngredient(Sprite ingredientSprite, string name)
    {
        slotImage.sprite = ingredientSprite;
        slotImage.color = Color.white; // �̹����� ���̵��� �������ϰ� ����
        ingredientName = name;
        isFilled = true;
    }

    // ���� ����
    public void RemoveIngredient()
    {
        InitializeSlot();
    }

    // ������ ����ִ��� ���θ� Ȯ���ϴ� �޼���
    public bool IsFilled()
    {
        return isFilled;
    }

    public string GetIngredientName()
    {
        return ingredientName;
    }
}
