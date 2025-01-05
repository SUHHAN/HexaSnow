using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board_h : MonoBehaviour
{
    [SerializeField]
    private GameObject cardPrefab;
    void Start()
    {
        // 처음 보드를 초기화 하는 작업
        InitBoard();
    }

    void InitBoard() {
            float spaceX = 1.4f; // 열 간격 (수평)
            float spaceY = 2.0f;
            int rowCount = 4;
            int colCount = 5;

            // 행 계산식 :  (row - (int)(rowCount / 2)) * spaceY
            // col
            // 0 - 3 = -3 
            // 1 - 3 = -2  
            // 2 - 3 = -1 + 0.5
            // 3 - 3 = 0 + 0
            // 4 - 3 = 1 + 
            // 5 - 3 = 2

            for (int row = 0; row < rowCount; row++) {
                for (int col = 0; col < colCount; col++) {
                    float posY = (row - (int)(rowCount / 2)) * spaceY;
                    float posX = (col - (colCount - 1) / 2.0f) * spaceX;
                    Vector3 pos = new Vector3(posX, posY, 0f);
                    Instantiate(cardPrefab, pos, Quaternion.identity);
                }
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
}
