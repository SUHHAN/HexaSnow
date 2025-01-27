using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingGameManager1 : MonoBehaviour
{
    public static BakingGameManager1 Instance { get; private set; } // Singleton �������� GameManager �ν��Ͻ� ����

    private string selectedRecipe; // ���õ� ���� �̸� ����

    void Awake()
    {
        if (Instance == null) // Instance�� ����ִٸ�
        {
            Instance = this; // ���� ��ü�� Instance�� ����
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ������Ʈ ����
        }
        else
        {
            Destroy(gameObject); // �ߺ� ������ ������Ʈ�� �ı�
        }
    }

    public void SetSelectedRecipe(string recipe)
    {
        selectedRecipe = recipe; // ���õ� ���� �̸� ����
    }

    public string GetSelectedRecipe()
    {
        return selectedRecipe; // ����� ���� �̸� ��ȯ
    }
}
