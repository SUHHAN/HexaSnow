using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyImageManager : MonoBehaviour
{

    [SerializeField]  private GameObject background;
    [SerializeField] private Sprite newImage;
    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Lobby2")
        {
            this.enabled = false; // lobby2에서는 스크립트 비활성화
            return;
        }
        
        Invoke("changeBackground", 4.01f);
        Debug.Log("This script is attached to: " + gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void changeBackground() {
        background.GetComponent<UnityEngine.UI.Image>().sprite = newImage;
    }
}
