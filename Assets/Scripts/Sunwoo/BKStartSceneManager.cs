using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BKStartSceneManager : MonoBehaviour
{
    public Button startButton; // Start ��ư
    public GameObject StartPanel;

    void Start()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);

        if (startButton != null)
        {
            startButton.onClick.AddListener(LoadBakingScene); // ��ư Ŭ�� �̺�Ʈ ���
        }
        else
        {
            Debug.LogError("Start ��ư�� �Ҵ���� �ʾҽ��ϴ�!");
        }
        UiLogicManager.Instance.LoadMoneyData();

    }

    // Start ��ư Ŭ�� �� ������ �޼���
    public void LoadBakingScene()
    {
        SceneManager.LoadScene("Baking 1");
    }
}
