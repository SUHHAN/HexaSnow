using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_h : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;

    [SerializeField]
    private Sprite[] cardSprites;

    private List<int> cardIDList = new List<int>();
    private List<Card_h> cardList = new List<Card_h>();
    void Start()
    {
        GenerateCardID();
        ShuffleCardID();
        // 처음 보드를 초기화 하는 작업
        InitBoard();
    }

    // 카드의 ID를 부여해주는 함수
    void GenerateCardID() {
        // 0, 0, 1, 1, ..., 9, 9
        for (int i = 0; i< cardSprites.Length; i++) {
            cardIDList.Add(i);
            cardIDList.Add(i);
        }
    }

    // 반복문을 통해서, 순회를 하여 값을 랜덤하게 바꿔주는 함수
    void ShuffleCardID() {
        int cardCount = cardIDList.Count;
        for (int i = 0; i < cardCount; i++) {
            int randomIndex = Random.Range(i,cardCount);
            int temp = cardIDList[randomIndex];
            cardIDList[randomIndex] = cardIDList[i];
            cardIDList[i] = temp;

            Debug.Log(cardIDList[i]);
        }
    }

    void InitBoard() {
            float spaceX = 1.4f; // 열 간격 (수평)
            float spaceY = 2.0f;
            int rowCount = 4;
            int colCount = 5;

            // 카드 스프라이트의 인덱스 지정 변수
            int cardIndex = 0;

            for (int row = 0; row < rowCount; row++) {
                for (int col = 0; col < colCount; col++) {
                    float posY = (row - (int)(rowCount / 2)) * spaceY;
                    float posX = (col - (colCount - 1) / 2.0f) * spaceX;
                    Vector3 pos = new Vector3(posX, posY, 0f);
                    
                    GameObject cardObject = Instantiate(cardPrefab, pos, Quaternion.identity);
                    Card_h card = cardObject.GetComponent<Card_h>();

                    int cardID = cardIDList[cardIndex++];
                    card.SetCardID(cardID);

                    card.SetAnimalSprite(cardSprites[cardID]);
                    // 각 카드의 사진과 index를 저장한 카드 오브젝트를 card list에 저장
                    cardList.Add(card);
                }
            }
        }

    public List<Card_h> GetCards() {
        return cardList;
    }
}

//     int rowCount = 6; // 행 개수 (6개)
    //     int colCount = 4; // 열 개수 (4개)

    //     float screenWidth = 5.0f; // 보드의 전체 가로 크기
    //     float screenHeight = 8.0f; // 보드의 전체 세로 크기

    //     // 행과 열 간 간격 계산
    //     float spaceX = screenWidth / (colCount - 1); // 열 간격 (보드 크기에 맞게)
    //     float spaceY = screenHeight / (rowCount - 1); // 행 간격 (보드 크기에 맞게)

    //     // 중심으로 카드 배치
    //     for (int row = 0; row < rowCount; row++) {
    //         for (int col = 0; col < colCount; col++) {
    //             float posY = (row - (rowCount - 1) / 2.0f) * spaceY; // 행 위치
    //             float posX = (col - (colCount - 1) / 2.0f) * spaceX; // 열 위치
    //             Vector3 pos = new Vector3(posX, posY, 0f);
    //             Instantiate(cardPrefab, pos, Quaternion.identity); // 카드 배치
    //         }
    //     }
    // }
