using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonChildEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Button button;
    private Color pressedColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        pressedColor = button.colors.pressedColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ChildWithParentButtonClickEffect(button.transform);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        RemoveButtonClickEffect(button.transform);
    }

    public void ChildWithParentButtonClickEffect(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            //做特效
            PlayEffect(child);
            ChildWithParentButtonClickEffect(child);
        }
    }

    public void RemoveButtonClickEffect(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            //做特效
            RemoveEffect(child);
            ChildWithParentButtonClickEffect(child);
        }
    }

    private void PlayEffect(Transform child)
    {
        Image img = child.GetComponent<Image>();
        if (img)
        {
            img.color = MultiplyColor(img.color, pressedColor);
        }
    }

    public Color MultiplyColor(Color colorA, Color colorB)
    {
        return new Color(colorA.r * colorB.r, colorA.g * colorB.g, colorA.b * colorB.b, colorA.a * colorB.a);
    }

    private void RemoveEffect(Transform child)
    {
        Image img = child.GetComponent<Image>();
        if (img)
        {
            img.color = DivisionColor(img.color, pressedColor);
        }
    }

    public Color DivisionColor(Color colorA, Color colorB)
    {
        return new Color(colorA.r / colorB.r, colorA.g / colorB.g, colorA.b / colorB.b, colorA.a / colorB.a);
    }
}