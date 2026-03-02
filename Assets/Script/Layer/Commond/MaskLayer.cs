using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskLayer : MonoBehaviour
{
    private PanelBase panel;

    [SerializeField]
    Button maskBtn;

    void Awake()
    {
        maskBtn.onClick.AddListener(OnCloseClick);
    }

    public void SetPanel(PanelBase panel)
    {
        this.panel = panel;
    }

    private void OnCloseClick()
    {
        UIManager.Instance.CloseLayer(panel.name);
    }
}