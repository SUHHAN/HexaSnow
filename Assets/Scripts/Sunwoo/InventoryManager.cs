using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<string> ownedIngredients = new List<string>(); // ������ ��� ���

    void Start()
    {
        // �ʱ� ��� ����
        ownedIngredients.Add("Butter");
        ownedIngredients.Add("Egg");
        ownedIngredients.Add("Flour");
        ownedIngredients.Add("Sugar");
    }

    // ���� ���� Ȯ�� �޼���
    public bool HasIngredient(string ingredient)
    {
        return ownedIngredients.Contains(ingredient);
    }
}