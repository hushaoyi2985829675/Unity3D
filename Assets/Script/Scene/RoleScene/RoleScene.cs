using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

public class RoleScene : SceneBase
{
    public GameObject testObj;
    private CinemachineVirtualCamera virtualCamera;
    private bool isDown;
    private float posX;
    private GameObject roleObj;
    private float rotation;
    private float speed = 0.5f;
    private float initialRotationY;
    public override void onEnter(params object[] data)
    {
       
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Role"))
                {
                    MouseSatrt();
                }
            }
        }

        if (Input.GetMouseButton(0) && isDown)
        {
            MouseDrag();
        }
        
        if (Input.GetMouseButtonUp(0) && isDown)
        {
            RoleResetRotation();
        }
    }

    public void RefreshUI(int id)
    {
        Ui.Instance.RemoveAllChildren(transform);
        roleObj = RoleManager.Instance.GetBaseRoleInstance(id,transform);
    }

    private void MouseSatrt()
    {
        roleObj.transform.DOKill();
        float currentY = roleObj.transform.localEulerAngles.y;
        posX = Input.mousePosition.x;
        isDown = true;
        rotation = 0;
        if (currentY > 180)
        {
            rotation = currentY - 360;
        }
        else
        {
            rotation = currentY;
        }
    }

    private void MouseDrag()
    {
        float curPosX = Input.mousePosition.x;
        
        if (posX == curPosX)
        {
            return;
        }
        
        float deltaY = (posX - curPosX) * speed;
        rotation += deltaY;
        rotation = Mathf.Clamp(rotation, -179.9f, 180f);
        roleObj.transform.localRotation = Quaternion.Euler(
            roleObj.transform.localEulerAngles.x,
            rotation,
            roleObj.transform.localEulerAngles.z
        );
        posX = curPosX;
    }

    private void RoleResetRotation()
    {
        isDown = false;
        float time = math.abs(rotation) / 180 * 0.5f;
        roleObj.transform.DOLocalRotate(new Vector3(0, 0, 0),time );
    }

    public override void onExit(params object[] data)
    {
        
    }
}
