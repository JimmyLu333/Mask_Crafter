using UnityEngine;
using UnityEngine.EventSystems;

public class GameItem : MonoBehaviour, IPointerClickHandler
{
    [Header("物品信息")]
    public string itemName;          // 物品名称（必须与ItemData中的一致）

    [Header("视觉设置")]
    public Color foundColor = new Color(0.6f, 0.6f, 0.6f, 0.4f); // 找到后的颜色

    // 私有变量
    private bool isFound = false;
    private GameManager gameManager;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        // 获取组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // 查找GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("找不到GameManager！");
        }

        // 确保有Collider
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<BoxCollider2D>();
            Debug.Log($"为 {itemName} 添加了BoxCollider2D");
        }
    }

    // 鼠标/触摸点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isFound)
        {
            Debug.Log($"{itemName} 已经找到了，不能再点击");
            return;
        }

        Debug.Log($"点击物品: {itemName}");

        if (gameManager != null)
        {
            gameManager.OnItemClicked(this);
        }
        else
        {
            Debug.LogError("GameManager为空！");
        }
    }

    // 标记为已找到（由GameManager调用）
    public void MarkAsFound()
    {
        if (isFound) return;

        isFound = true;
        Debug.Log($"物品 {itemName} 被标记为已找到");

        // 改变颜色（变灰/半透明）
        if (spriteRenderer != null)
        {
            spriteRenderer.color = foundColor;
        }

        // 禁用碰撞器，防止再次点击
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }

        // 可选：添加缩放动画
        StartCoroutine(FoundAnimation());
    }

    // 简单的找到动画
    private System.Collections.IEnumerator FoundAnimation()
    {
        Vector3 originalScale = transform.localScale;
        float duration = 0.3f;
        float elapsed = 0f;

        // 先放大
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.2f, elapsed / (duration / 2));
            transform.localScale = originalScale * scale;
            yield return null;
        }

        // 再缩小
        elapsed = 0f;
        while (elapsed < duration / 2)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1.2f, 0.9f, elapsed / (duration / 2));
            transform.localScale = originalScale * scale;
            yield return null;
        }

        // 恢复正常大小
        transform.localScale = originalScale;
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