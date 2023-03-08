using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int power;

    // Start is called before the first frame update
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "GROUND")
        {
            Destroy(gameObject, 2f);
        }
        else if (col.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if (col.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
