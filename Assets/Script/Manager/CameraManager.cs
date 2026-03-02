using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CameraManager : Singleton<CameraManager>
{
    public Image maskLayer;
    private CinemachineVirtualCamera playerCam;
    private Coroutine CameraCoroutine;
    private float startSize;
    private CinemachineConfiner cameraConfiner;
    CinemachineFramingTransposer transposer;
    private GameObject playerCamObj;
    private Transform boundaryNode;
    void Start()
    {
        playerCamObj = GameObject.FindGameObjectWithTag("PlayerCamera");
        playerCam = playerCamObj.GetComponent<CinemachineVirtualCamera>();
        startSize = playerCam.m_Lens.OrthographicSize;
        cameraConfiner = playerCamObj.GetComponent<CinemachineConfiner>();
        transposer = playerCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        boundaryNode = GameObject.FindWithTag("CameraBoundary").transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void PlayerCamScale(float orthoSize, float time, Action action = null)
    {
        if (CameraCoroutine != null)
        {
            StopCoroutine(CameraCoroutine);
            CameraCoroutine = null;
        }

        playerCam.m_Lens.OrthographicSize = startSize;
        CameraCoroutine = StartCoroutine(PlayerCamAction(orthoSize, time, action));
    }

    private IEnumerator PlayerCamAction(float orthoSize, float time, Action action)
    {
        float curTime = 0;
        float t;
        float size;
        maskLayer.raycastTarget = true;
        while (curTime < time)
        {
            t = 1 - Mathf.Pow(1 - (curTime / time), 3);
            size = Mathf.Lerp(startSize,orthoSize,  t);
            playerCam.m_Lens.OrthographicSize = size;
            Color color = maskLayer.color;
            color.a = Mathf.Lerp(0,1, t);
            maskLayer.color = color;
            curTime += Time.deltaTime;
            yield return null;
        }
        playerCam.m_Lens.OrthographicSize = orthoSize;
        action?.Invoke();
        yield return new WaitForSeconds(1f);
        curTime = 0;
        while (curTime < time)
        {
            t = Mathf.Pow(curTime / time, 2);
            size = Mathf.Lerp(orthoSize,startSize,  t);
            playerCam.m_Lens.OrthographicSize = size;
            Color color = maskLayer.color;
            color.a = Mathf.Lerp(1,0, t);
            maskLayer.color = color;
            curTime += Time.deltaTime;
            yield return null;
        }

        maskLayer.raycastTarget = false;
        playerCam.m_Lens.OrthographicSize = startSize;
    }

    public void ChangeMapAction(Action action)
    {
        PlayerCamScale(6f, 1f, () =>
        {
            action?.Invoke();
        });
    }

    //切换边界
    public void ChangeBoundary(GameObject boundaryRef)
    {
        Ui.Instance.RemoveAllChildren(boundaryNode);
        cameraConfiner.InvalidatePathCache();
        //创建边界
        GameObject boundary = Instantiate(boundaryRef, boundaryNode);
        boundary.transform.localPosition = Vector3.zero;
        PolygonCollider2D newCollider = boundary.GetComponent<PolygonCollider2D>();
        cameraConfiner.m_BoundingShape2D = null;
        cameraConfiner.m_BoundingShape2D = newCollider;
    }

    //抖动
    public void ShakeCamera(float time, float strength)
    {
        StartCoroutine(Shake(time, strength));
    }

    private IEnumerator Shake(float time, float strength)
    {
        Vector3 offSet = transposer.m_TrackedObjectOffset;
        while (time > 0)
        {
            Vector3 newPos = Random.insideUnitSphere * strength + offSet;
            transposer.m_TrackedObjectOffset = newPos;
            time -= Time.deltaTime;
            yield return null;
        }

        transposer.m_TrackedObjectOffset = offSet;
        yield return null;
    }
}
