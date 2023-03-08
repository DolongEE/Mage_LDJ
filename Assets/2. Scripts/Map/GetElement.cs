using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GetElement : MonoBehaviour
{
    public GameObject[] elements;
    public SphereCollider Heart;
    public GameObject earthMapPortal;
    PlayerCtrl playerCtrl;
    public bool anemo;
    public bool pyro;
    public bool hydro;
    public bool geo;


    private void Awake()
    {
        if (PhotonNetwork.isMasterClient)
        {
            playerCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
                 
        }        
    }
    void Update()
    {
        
        if(PhotonNetwork.isMasterClient)
        {
            if (playerCtrl.getPyro)
            {
                pyro = true;
                elements[0].SetActive(true);
            }
            if (playerCtrl.getHydro)
            {
                hydro = true;
                elements[1].SetActive(true);
            }
            if (playerCtrl.getAnemo)
            {
                anemo = true;
                elements[2].SetActive(true);
            }
            if (playerCtrl.getGeo)
            {
                geo = true;
                elements[3].SetActive(true);
            }

            if(anemo && pyro && hydro)
            {
                earthMapPortal.SetActive(true);
            }

            if (anemo && geo && pyro && hydro)
            {
                Heart.enabled = true;
            }

           
        }
    }
}
