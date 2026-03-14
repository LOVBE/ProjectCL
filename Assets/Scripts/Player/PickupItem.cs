using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemID itemType;
    [SerializeField] private int amount = 1;

    private Renderer itemRenderer;
    private Color originalColor;
    private bool isHighlighted;

    private void Start()
    {
        itemRenderer = GetComponent<Renderer>();
        if (itemRenderer != null)
        {
            originalColor = itemRenderer.material.color;
        }
    }

    public void Interact(GameObject interactor)
    {
        PlayerInventory.Instance?.AddItem(itemType, amount);
        Destroy(gameObject);
    }

    public void Highlight()
    {
        if (isHighlighted || itemRenderer == null) return;
        isHighlighted = true;
        
        // MVP: Đổi màu xanh lá để làm highlight
        itemRenderer.material.color = Color.green;
    }

    public void RemoveHighlight()
    {
        if (!isHighlighted || itemRenderer == null) return;
        isHighlighted = false;
        
        // Trả về màu cũ
        itemRenderer.material.color = originalColor;
    }
}
