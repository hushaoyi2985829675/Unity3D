using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabItem : PanelBase
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Text text;
    private Image image;
    
    private int id;
    private Action<int> callback;
    public override void onEnter(params object[] data)
    {
        image = button.GetComponent<Image>();
        button.onClick.AddListener(OnClick);
    }

    public override void onShow(params object[] data)
    {
      
    }

    public void Init(int tagId,Action<int> callback)
    {
        id = tagId;
        this.callback = callback;
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (id == 1)
        {
            text.text = "角色";
        }
        else if (id == 2)
        {
            text.text = "故事";
        }
        else if (id == 3)
        {
            text.text = "属性";
        }
    }

    public void RefreshSel(bool isSel)
    {
        
        Sprite sprite = isSel ? Ui.Instance.GetSprite("BlueTitle") : Ui.Instance.GetSprite("DialogueTitle");
        image.sprite = sprite;
        Color color = Color.black;
        if (isSel)
        {
            // ColorUtility.TryParseHtmlString("#767676", out color);
            color = Color.white;
        }
        text.color = color;
    }

    private void OnClick()
    {
        if (callback != null)
        {
            callback(id);
        }
    }

    public override void onExit()
    {
        
    }
}
