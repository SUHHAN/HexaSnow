using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyImageManager : MonoBehaviour
{

    [SerializeField]  private GameObject background;
    [SerializeField] private Sprite newImage;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("changeBackground", 4.01f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void changeBackground() {
        background.GetComponent<UnityEngine.UI.Image>().sprite = newImage;
    }
}
