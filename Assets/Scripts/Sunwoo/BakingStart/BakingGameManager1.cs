using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakingGameManager1 : MonoBehaviour
{
    public static BakingGameManager1 Instance { get; private set; } // Singleton 패턴으로 GameManager 인스턴스 관리

    private string selectedRecipe; // 선택된 제과 이름 저장

    void Awake()
    {
        if (Instance == null) // Instance가 비어있다면
        {
            Instance = this; // 현재 객체를 Instance로 설정
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 오브젝트 유지
        }
        else
        {
            Destroy(gameObject); // 중복 생성된 오브젝트는 파괴
        }
    }

    public void SetSelectedRecipe(string recipe)
    {
        selectedRecipe = recipe; // 선택된 제과 이름 저장
    }

    public string GetSelectedRecipe()
    {
        return selectedRecipe; // 저장된 제과 이름 반환
    }
}
