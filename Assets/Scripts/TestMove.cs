using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    [Header("移动速度")]
    [SerializeField] float speed = 6.0F;
    [Header("旋转灵敏度")]
    [SerializeField] float lookSensitivity = 2;
    private Vector3 moveDirection = Vector3.zero;
    private Transform _trans;
    float yRotation;
    float xRotation;
    float currentXRotation;
    float currentYRotation;
    float yRotationV;
    float xRotationV;
    public bool canMove = true;
    void Start()
    {
        _trans = transform;
        yRotation = _trans.localEulerAngles.y;
        xRotation = _trans.localEulerAngles.x;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(2))
        {
            canMove = !canMove;
        }
        if (!canMove) return;
        //视野方向移动
        moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        moveDirection *= speed * Time.deltaTime;
        _trans.Translate(moveDirection);
        //转向相机目标
        if (Input.GetMouseButton(1))
        {  
            yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
            xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
            xRotation = Mathf.Clamp(xRotation, -80, 100);
            _trans.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }    

        //上下运动
        if (Input.GetKey(KeyCode.Q))
        {
            _trans.Translate(-Vector3.up * speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            _trans.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }
}