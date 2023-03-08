using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour 
{
    public int power;
    public Collider co;
    bool isStop;

    //충돌이 일어나면 코루틴으로 연속충돌을 방지한다
    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            StartCoroutine(this.ResetColl());
        }
    }

    private void OnParticleCollision(GameObject other)
    {

    }

    IEnumerator ResetColl()
    {
        co.enabled = false;
        yield return new WaitForSeconds(1.5f);
        co.enabled = true;

        yield return null;
    }
}
