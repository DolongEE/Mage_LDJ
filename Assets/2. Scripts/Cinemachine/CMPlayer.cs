using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;

public class CMPlayer : MonoBehaviour
{
    // 플레이어 시야
    Transform playerEye;
    CinemachineVirtualCameraBase cmPlayer;

    // 마우스 이동속도
    public float mouseSpeed = 0.5f;
    float mouseX = 0f;
    float mouseY = 0f;

    public bool isUiOpen;

    private bool moveCamera;
    void Awake()
    {
        isUiOpen = false;
        // 처음 카메라 할당 안함
        this.enabled = false;

        cmPlayer = GetComponent<CinemachineVirtualCameraBase>();
    }

    void Start()
    {
        moveCamera = false;
        playerEye = cmPlayer.Follow;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Alt키를 눌렀을때 화면 고정
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            if (!isUiOpen)
            {
                isUiOpen = true;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                moveCamera = false;
            }
            else
            {
                isUiOpen = false;
                moveCamera = true;
            }
        }
        // Alt키가 눌리지 않았을때
        else if (!isUiOpen)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            moveCamera = true;
        }
    }

    void FixedUpdate()
    {
        if (moveCamera)
        {
            MoveCamera();
        }
    }

    // 마우스에 따라 바라보는 방향이 정해짐
    void MoveCamera()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSpeed;
        mouseY += Input.GetAxis("Mouse Y") * mouseSpeed;

        mouseY = Mathf.Clamp(mouseY, -55.0f, 30.0f);

        playerEye.eulerAngles = new Vector3(-mouseY, mouseX, 0);
    }
}
