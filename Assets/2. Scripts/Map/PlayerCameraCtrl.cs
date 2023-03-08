using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraCtrl : MonoBehaviour
{
    [SerializeField]
    private Transform characterBody;
    [SerializeField]
    private Transform cameraArm;

    Animator anim;
    Rigidbody rigid;

    public float moveForce = 15f;
    public float jumpForce = 5f;
    private bool isJump;

    private void Awake()
    {
        isJump = false;
        rigid= GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = characterBody.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        LookAround();
        Move();
        Jump();
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        //anim.SetBool("isMove", isMove);
        if(isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            characterBody.forward = lookForward;
            transform.position += moveDir * Time.deltaTime * moveForce;
        }
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;
        float x = camAngle.x - mouseDelta.y;

        if(x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

    }

    private void Jump()
    {
        if (Input.GetButton("Jump") && !isJump)
        {
            isJump = true;
            rigid.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "GROUND")
        {
            isJump = false;
        }
    }
}
