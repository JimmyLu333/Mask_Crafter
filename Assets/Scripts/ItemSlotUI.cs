using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [Header("UI组件")]
    public Image itemIcon;          // 物品图标
    public GameObject checkmark;    // 勾选标记

    [Header("颜色设置")]
    public Color normalColor = Color.white;         // 正常颜色
    public Color foundColor = new Color(0.6f, 0.6f, 0.6f, 0.8f); // 找到后的颜色

    // 私有变量
    private string itemName;
    private bool isFound = false;

    // 初始化物品槽
    public void Initialize(string name, Sprite icon)
    {
        itemName = name;

        // 设置图标
        if (itemIcon != null)
        {
            itemIcon.sprite = icon;
            itemIcon.color = normalColor;
        }

        // 隐藏勾选标记
        if (checkmark != null)
        {
            checkmark.SetActive(false);
        }

        isFound = false;

        Debug.Log($"物品槽初始化: {itemName}");
    }

    // 标记为已找到
    public void MarkAsFound()
    {
        if (isFound) return;

        isFound = true;
        Debug.Log($"物品槽 {itemName} 标记为已找到");

        // 显示勾选标记
        if (checkmark != null)
        {
            checkmark.SetActive(true);

            // 添加简单的动画
            StartCoroutine(CheckmarkAnimation());
        }

        // 改变图标颜色（变灰）
        if (itemIcon != null)
        {
            itemIcon.color = foundColor;
        }
    }

    // 勾选标记动画
    private System.Collections.IEnumerator CheckmarkAnimation()
    {
        if (checkmark == null) yield break;

        Transform checkmarkTransform = checkmark.transform;
        Vector3 originalScale = checkmarkTransform.localScale;

        // 缩放动画
        float duration = 0.3f;
        float elapsed = 0f;

        // 从0放大到1.2倍
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1.2f, elapsed / (duration / 2));
            checkmarkTransform.localScale = originalScale * scale;
            yield return null;
        }

        // 从1.2倍缩放到1倍
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 1f, elapsed / (duration / 2));
            checkmarkTransform.localScale = originalScale * scale;
            yield return null;
        }

        checkmarkTransform.localScale = originalScale;
    }

    // 获取物品名称
    public string GetItemName()
    {
        return itemName;
    }

    // 是否已找到
    public bool IsFound()
    {
        return isFound;
    }
}