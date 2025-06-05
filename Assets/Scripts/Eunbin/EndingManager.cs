using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
     [SerializeField] private GameData GD = new GameData();
    // Start is called before the first frame update
    void Start()
    {
        GD = DataManager.Instance.LoadGameData();
        if(GD.date==11){
            if (GD.EndingCount==3){
                SceneManager.LoadScene("happyEnding");
            }
            else {
                SceneManager.LoadScene("badEnding");
            }
        }
    }
}
