using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;      // 物品名称（重要：必须唯一）
    public Sprite itemIcon;      // 物品图标（显示在底部面板）
    public bool isFound = false; // 是否已找到

    // 构造函数（可选）
    public ItemData(string name, Sprite icon)
    {
        itemName = name;
        itemIcon = icon;
    }
}