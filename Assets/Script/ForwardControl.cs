using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ForwardControl : MonoBehaviour, IPointerDownHandler
{
    private float oldPos;
    private bool isDraggingScene;
    private bool isOverUI;
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    protected virtual void HandlePointerDown(PointerEventData eventData)
    {
    }
    private void Start()
    {
        oldPos = Input.mousePosition.x;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDraggingScene = false;
            isOverUI = false;
            if (EventSystem.current != null)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    isOverUI = EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                }
                else
                {
                    isOverUI = EventSystem.current.IsPointerOverGameObject();
                }
            }
            if (!isOverUI)
            {
                isDraggingScene = true;
                oldPos = Input.mousePosition.x;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDraggingScene = false;
        }
        if (Input.GetMouseButton(0) && isDraggingScene)
        {
            Vector3 currentEuler = transform.eulerAngles;
            transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y + (Input.mousePosition.x - oldPos) * 0.3f, currentEuler.z);
            oldPos = Input.mousePosition.x;
        }
    }
}
