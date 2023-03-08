using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    private GameObject[] player1;
    private GameObject mSquare;
    private GameObject Warp;
    void Awake()
    {
        player1 = GameObject.FindGameObjectsWithTag("Player");
        mSquare = GameObject.Find("MagicSquareB");
        Warp = GameObject.Find("ForestWarp");
        Warp.SetActive(false);
    }
    private void OnTriggerEnter(Collider p1)
    {
        if (p1.gameObject.tag == "Player")
        {
            Debug.Log("Warp 활성화");
            Warp.SetActive(true);
            StartCoroutine(WarpMove(p1));
            
        }
        //if (p1.gameObject.tag == "Player")
        //{
        //    Debug.Log("순간이동");
        //    player1.transform.position = new Vector3(mSquare.transform.position.x, mSquare.transform.position.y + 2, mSquare.transform.position.z);

        //}
    }

    private void OnTriggerExit(Collider p1)
    {
        if (p1.gameObject.tag == "Player")
        {
            Debug.Log("Warp 활성화");
            Warp.SetActive(false);
        }
    }
    IEnumerator WarpMove(Collider p1)
    {
        yield return new WaitForSeconds(2f);
        if (Warp.gameObject.activeSelf == true)
        {
            Debug.Log("순간이동");
            p1.gameObject.transform.position = new Vector3(mSquare.transform.position.x, mSquare.transform.position.y + 2, mSquare.transform.position.z);
        }
        Warp.SetActive(false);
    }
}
