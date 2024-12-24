using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager_h : MonoBehaviour
{
    [SerializeField]
    private GameObject enemy;
    void Start()
    {
        StartCoroutine(CreateenemyRoutine());
    }

    IEnumerator CreateenemyRoutine() {
        while(true)
        {
            CreateEnemy();
            yield return new WaitForSeconds(1);
        }
    }


    private void CreateEnemy() {
        Vector3 pos = new Vector3(0,6,0);
        Instantiate(enemy, pos, Quaternion.identity);
    }
}
