using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemID itemType;
    [SerializeField] private int amount = 1;

    private Renderer itemRenderer;
    private Color originalColor;
    private bool isHighlighted;

    public ItemID ItemType => itemType;

    private void Awake()
    {
        CacheRenderer();
    }

    public void Interact(GameObject interactor)
    {
        Debug.Log($"[PickupItem] Vật thể '{gameObject.name}' đang được nhặt bởi '{interactor.name}'!");
        
        if (PlayerInventory.Instance == null)
        {
            Debug.LogError("[PickupItem] LỖI: PlayerInventory.Instance bị NULL, không có túi đồ để cất!");
        }
        
        PlayerInventory.Instance?.AddItem(itemType, amount);
        Destroy(gameObject);
        Debug.Log($"[PickupItem] Đã hủy '{gameObject.name}' thành công.");
    }

    public void Configure(ItemID newItemType, int newAmount, Color baseColor)
    {
        itemType = newItemType;
        amount = newAmount;

        CacheRenderer();
        if (itemRenderer != null)
        {
            itemRenderer.material.color = baseColor;
            originalColor = baseColor;
        }
    }

    public void Highlight()
    {
        CacheRenderer();
        if (isHighlighted || itemRenderer == null) return;
        isHighlighted = true;
        
        // MVP: Đổi màu xanh lá để làm highlight
        itemRenderer.material.color = Color.green;
    }

    public void RemoveHighlight()
    {
        CacheRenderer();
        if (!isHighlighted || itemRenderer == null) return;
        isHighlighted = false;
        
        // Trả về màu cũ
        itemRenderer.material.color = originalColor;
    }

    private void CacheRenderer()
    {
        if (itemRenderer == null)
        {
            itemRenderer = GetComponent<Renderer>();
        }

        if (itemRenderer != null && !isHighlighted)
        {
            originalColor = itemRenderer.material.color;
        }
    }
}
