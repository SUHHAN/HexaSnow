using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchGame_h : MonoBehaviour
{
    public static MatchGame_h instance;
    private List<Card_h> allCards;

    void Awake() {
        if (instance == null){
            instance = this;
        }
    }
    void Start()
    {
        Board_h board = FindObjectOfType<Board_h>();
        allCards = board.GetCards();

        StartCoroutine("FilpAllCardsRoutine");
    }

    IEnumerator FilpAllCardsRoutine() {
        yield return new WaitForSeconds(0.5f);
        FilpAllCards();
        yield return new WaitForSeconds(3f);
        FilpAllCards();
        yield return new WaitForSeconds(0.5f);
    }

    void FilpAllCards() {
        foreach (Card_h card in allCards) {
            card.FilpCard();
        }
    }
}
