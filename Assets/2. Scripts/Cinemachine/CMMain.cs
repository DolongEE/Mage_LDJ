using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CMMain : MonoBehaviour
{
    CinemachineBlendListCamera blendList;

    GameObject cmPlayer;
    GameObject cmCharacterSelect;
    CinemachineVirtualCameraBase csCMPlayer;
    CinemachineVirtualCameraBase csCMSelectCharacter;
    void Awake()
    {
        blendList = this.GetComponent<CinemachineBlendListCamera>();

        blendList.m_Loop = false;

        cmPlayer = GameObject.FindGameObjectWithTag("CMPLAYER");
        cmCharacterSelect = GameObject.Find("CMSelectCharacter");

        csCMPlayer = cmPlayer.GetComponent<CinemachineVirtualCameraBase>();
        csCMSelectCharacter = cmCharacterSelect.GetComponent<CinemachineVirtualCameraBase>();
    }

    public void GameStart()
    {
            blendList.m_Instructions[0].m_VirtualCamera = csCMSelectCharacter;
            blendList.m_Instructions[1].m_VirtualCamera = csCMPlayer;

            blendList.m_Instructions[1].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            blendList.m_Instructions[1].m_Blend.m_Time = 2.0f;
    }
}
