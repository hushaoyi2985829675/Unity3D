using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseBtn : MonoBehaviour
{
    private Button btn;

    [SerializeField] private GameObject layer;

    public void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(() => { UIManager.Instance.CloseLayer(layer.name); });
    }
}