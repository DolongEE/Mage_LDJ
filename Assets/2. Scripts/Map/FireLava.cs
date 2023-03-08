using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLava : MonoBehaviour
{
    //public Transform respawn;


    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerCtrl>().isDie = true;
        }
    }
}
