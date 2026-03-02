using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class TableView : MonoBehaviour
{
    [Header("方向")]
    [SerializeField] private DirectionType direction = DirectionType.Horizontal;
    [SerializeField]
    [Header("行数")] private int rowNum = 1;
    private int columnNum;
    [Header("是否中心缩放")] 
    [SerializeField]
    private bool isScale = false;
    [Header("节点")]
    [SerializeField]
    private GameObject temp;
    [Header("父节点")]
    [SerializeField]
    private RectTransform content;
    [Header("物体间的间隔")]
    [SerializeField]
    private Vector2 tempSpace;
    [Header("左或上间隔")]
    [SerializeField]
    private float leftTopSpace;
    [Header("右或下边间隔")]
    [SerializeField]
    private float rightBottomSpace;
    private int num;
    private ScrollRect scrollView;
    Dictionary<int, GameObject> itemList = new Dictionary<int, GameObject>();
    int minIdx = 0;
    int maxIdx = 0;
    int oldMinIdx = 0;
    int oldMaxIdx = 0;
    private Stack<GameObject> stack;
    private int count;
    private Vector2 size;
    private RectTransform contentRect;
    private Vector2 contentParentSize;
    Action<int, GameObject> action;
    Action<float, GameObject> scaleAction;
    private RectTransform scrollRect;
    private void Awake()
    {
        contentRect = content.GetComponent<RectTransform>();
        contentParentSize = content.parent.GetComponent<RectTransform>().sizeDelta;
        scrollView = GetComponent<ScrollRect>();
        scrollView.horizontal = false;
        scrollView.vertical = false;
        scrollRect = scrollView.GetComponent<RectTransform>();
    }

    public void SetNum(int num)
    {
        if (num <= 0)
        {
            return;
        }

        columnNum = (int)Mathf.Ceil((float)num / rowNum);
        oldMinIdx = 0;
        oldMaxIdx = 0;
        minIdx = 0;
        maxIdx = 0;
        itemList = new Dictionary<int, GameObject>();
        stack = new Stack<GameObject>();
        scrollView.GetComponent<ScrollRect>().onValueChanged.AddListener(OnUpdate);
        RectTransform tempRect = temp.GetComponent<RectTransform>();
        size = tempRect.sizeDelta * tempRect.localScale;
        Clear();
        if (direction == DirectionType.Horizontal)
        {
            scrollView.horizontal = true;
            content.anchorMin = new Vector2(0, 0);
            content.anchorMax = new Vector2(0, 1);
            content.pivot = new Vector2(0, 0.5f);
            content.sizeDelta = Vector2.zero;
            content.sizeDelta = new Vector2(columnNum * (size.x + tempSpace.x) -  tempSpace.x + leftTopSpace + rightBottomSpace, 0);
            count = (int) Mathf.Ceil((scrollRect.sizeDelta.x +  tempSpace.x) / size.x +  tempSpace.x);
        }
        else
        {
            scrollView.vertical = true;
            content.anchorMin = new Vector2(0, 1);
            content.anchorMax = new Vector2(1, 1);
            content.pivot = new Vector2(0.5f, 1);
            content.sizeDelta = Vector2.zero;
            content.sizeDelta = new Vector2(0,
                columnNum * (size.y + tempSpace.y) - tempSpace.y + leftTopSpace + rightBottomSpace);
            count = (int) Mathf.Ceil((scrollRect.sizeDelta.y + tempSpace.y) / (size.y + tempSpace.y));
        }
        this.num = num;
        ResetContentPos();
        RefreshData();
    }

    private void ResetContentPos()
    {
        contentRect.anchoredPosition = new Vector2(-contentParentSize.x / 2, contentParentSize.y / 2);
    }

    private void OnUpdate(Vector2 v)
    {
        if (num <= 0)
        {
            return;
        }
        RefreshData();
    }

    private void RefreshData(bool isRefreshAll = false)
    {
        if (direction == DirectionType.Horizontal)
        {
            minIdx = Mathf.Max(
                (int) ((-contentRect.anchoredPosition.x - leftTopSpace + tempSpace.x) / (size.x + tempSpace.x)), 0);
        }
        else
        {
            minIdx = Mathf.Max(
                (int) ((contentRect.anchoredPosition.y - leftTopSpace + tempSpace.y) /
                       (size.y + tempSpace.y)), 0);
        }
        minIdx = Mathf.Clamp(minIdx * rowNum, 0, num - rowNum);
        maxIdx = Mathf.Min(minIdx + (count + 1) * rowNum - 1, num - 1);
        for (int i = oldMinIdx; i < minIdx; i++)
        {
            stack.Push(itemList[i]);
            itemList[i].SetActive(false);
            itemList.Remove(i);
        }

        for (int i = maxIdx + 1; i <= oldMaxIdx; i++)
        {
            stack.Push(itemList[i]);
            itemList[i].SetActive(false);
            itemList.Remove(i);
        }

        oldMinIdx = minIdx;
        oldMaxIdx = maxIdx;
        for (int i = minIdx; i <= maxIdx; i++)
        {
            if (!isRefreshAll)
            {
                if (itemList.ContainsKey(i))
                {
                    continue;
                }
            }

            GameObject item;
            if (stack.Count > 0)
            {
                itemList[i] = stack.Pop();
            }
            else
            {
                itemList[i] = UIManager.Instance.AddUINode(temp, content.transform).gameObject;
                RectTransform rectTrans = (RectTransform) itemList[i].transform;
                if (direction == DirectionType.Horizontal)
                {
                    rectTrans.anchorMin = new Vector2(0f, 0.5f);
                    rectTrans.anchorMax = new Vector2(0f, 0.5f);
                }
                else
                {
                    rectTrans.anchorMin = new Vector2(0.5f, 1f);
                    rectTrans.anchorMax = new Vector2(0.5f, 1f);
                }
            }
            item = itemList[i];
            item.SetActive(true);
            int idxX = i / rowNum;
            int idxY =  i % rowNum;
            if (direction == DirectionType.Horizontal)
            {
                float startY = (rowNum - 1) / 2f * (size.y + tempSpace.y);
                item.GetComponent<RectTransform>().anchoredPosition = new Vector3((size.x + tempSpace.x) * idxX + leftTopSpace + size.x / 2,startY - idxY * (size.y +  tempSpace.y),0);
            }
            else
            {
                float startX = -(rowNum - 1) / 2f * (size.x + tempSpace.x);
                item.transform.localPosition = new Vector3(startX + idxY * (size.x +  tempSpace.x), (-size.y - tempSpace.y) * idxX - leftTopSpace - size.y / 2, 0);
            }

            action?.Invoke(i, item);
        }

        if (isScale)
        {
            UpdateScale();
        }
    }

    private void UpdateScale()
    {
        foreach (var data in itemList)
        {
            GameObject item = data.Value;
            var worldPos = item.transform.parent.TransformPoint(item.transform.localPosition);
            var pos = item.transform.parent.parent.InverseTransformPoint(worldPos);
            float scale;
            if (Mathf.Abs(pos.x) < 300.0f)
            {
                scale = Mathf.Lerp(1.2f, 1.0f, Mathf.Abs(pos.x) / 300.0f);
            }
            else
            {
                scale = 1;
            }

            scaleAction?.Invoke(scale, item);
        }
    }

    public void RefreshAllData()
    {
        RefreshData(true);
    }

    public void AddRefreshEvent(Action<int, GameObject> action)
    {
        this.action += action;
    }

    public void AddScaleEvent(Action<float, GameObject> action)
    {
        this.scaleAction += action;
    }

    private void Clear()
    {
        Ui.Instance.RemoveAllChildren(content);
        num = 0;
        itemList.Clear();
        stack.Clear();
    }
}