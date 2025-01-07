using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Card_h : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer cardRenderer;
    [SerializeField]
    private Sprite animalSprite;
    [SerializeField]
    private Sprite backSprite;

    // isFilped: 카드가 뒤집혔는지 확인하는 변수
    private bool isFilped = false;
    // isFilpping: 카드가 뒤집히는 중인지 확인하는 변수
    private bool isFilpping = false;

    private bool isMatched = false;
    public int cardID;

    public void SetCardID(int id) {
        this.cardID = id;
    }

    public void SetMatched() {
        isMatched = true;
    }
    
    public void SetAnimalSprite(Sprite sprite) {
        this.animalSprite = sprite;
    }

    public void FilpCard() {

        isFilpping = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(0f, originalScale.y, originalScale.z);

        transform.DOScale(targetScale, 0.15f).OnComplete(() => 
        {
            // 뒤집혔는지 아닌지를 확인할 수 있게 됨
            isFilped = !isFilped;

            if (isFilped) {
                cardRenderer.sprite = animalSprite;
            }
            else {
                cardRenderer.sprite = backSprite;
            }
            // 카드를 원상복귀 시킴
            transform.DOScale(originalScale, 0.15f).OnComplete(() => {
                isFilpping = false;
            });
        });
    }

    void OnMouseDown() {
        if (!isFilpping && !isMatched && !isFilped) {
            MatchGame_h.instance.CardClicked(this); // 자기자신 입력
        }
    }
}
