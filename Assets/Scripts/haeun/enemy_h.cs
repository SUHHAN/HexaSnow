using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_h : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
                
        if (collision.gameObject.tag == "Ground") {
            Destroy(this.gameObject);
        }
        if (collision.gameObject.tag == "player") {
            Destroy(this.gameObject);
        }
    }
}
