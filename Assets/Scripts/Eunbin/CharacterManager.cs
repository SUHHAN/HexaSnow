using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Expression
{
    set, //기본 표정
    Happy,  // 행복한 표정
    Normal, // 보통 표정
    Bad     // 나쁜 표정
}
public class CharacterManager : MonoBehaviour
{
    public Sprite man;
    public Sprite happyman;
    public Sprite normalman;
    public Sprite badman;
    public Sprite girl;
    public Sprite happygirl;
    public Sprite normalgirl;
    public Sprite badgirl;
    public Sprite shortgirl;
    public Sprite happyshortgirl;
    public Sprite normalshortgirl;
    public Sprite badshortgirl;

    public Sprite child;
    public Sprite happychild;
    public Sprite normalchild;
    public Sprite badchild;
    public Sprite old_man;
    public Sprite happyold_man;
    public Sprite normalold_man;
    public Sprite badold_man;
    public Sprite spcMan;
    public Sprite happyspcMan;
    public Sprite normalspcMan;
    public Sprite badspcMan;



    // 캐릭터별 표정 스프라이트를 담을 Dictionary
    private Dictionary<string, Dictionary<Expression, Sprite>> customerExpressions = new Dictionary<string, Dictionary<Expression, Sprite>>();

    void Start()
    {
        // 캐릭터와 표정 스프라이트들을 매핑합니다.
        customerExpressions["man"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, man },
            { Expression.Happy, happyman },
            { Expression.Normal, normalman },
            { Expression.Bad, badman }
        };

        customerExpressions["girl"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, girl },
            { Expression.Happy, happygirl },
            { Expression.Normal, normalgirl },
            { Expression.Bad, badgirl }
        };

        customerExpressions["shortgirl"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, shortgirl },
            { Expression.Happy, happyshortgirl },
            { Expression.Normal, normalshortgirl},
            { Expression.Bad, badshortgirl }
        };
        customerExpressions["child"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, child },
            { Expression.Happy, happychild },
            { Expression.Normal, normalchild},
            { Expression.Bad, badchild }
        };
        customerExpressions["old_man"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, old_man },
            { Expression.Happy, happyold_man },
            { Expression.Normal, normalold_man},
            { Expression.Bad, badold_man }
        };
        customerExpressions["spcMan"] = new Dictionary<Expression, Sprite>
        {
            { Expression.set, spcMan },
            { Expression.Happy, happyspcMan },
            { Expression.Normal, normalspcMan},
            { Expression.Bad, badspcMan }
        };


    }

    // 표정 변경 함수
    public void ChangeFace(GameObject customer, Expression expression)
{
    Debug.LogError("얼굴 바뀌는 중");
    // customer의 Image 컴포넌트 가져오기
    SpriteRenderer customerRenderer = customer.GetComponent<SpriteRenderer>();
    if (customerRenderer == null)
    {
        Debug.LogError("고객의 SpriteRenderer 컴포넌트가 없습니다.");
        return;
    }
    // 표정 변경
    string characterType = customer.name.ToLower();
    Debug.Log($"[ChangeFace] 캐릭터: {characterType}, 표정: {expression}");


    // 표정에 맞는 스프라이트를 customerImage에 할당
    if (customerExpressions.ContainsKey(characterType) && customerExpressions[characterType].ContainsKey(expression))
    {
        customerRenderer.sprite = customerExpressions[characterType][expression];
        Debug.Log($"[ChangeFace] {characterType}의 표정이 {expression}으로 변경되었습니다.");
    }
    else
    {
        Debug.LogError("해당 캐릭터 또는 표정이 존재하지 않습니다.");
    }
}

}
