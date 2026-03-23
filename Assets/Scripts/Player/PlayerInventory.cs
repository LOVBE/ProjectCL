using System.Collections.Generic;
using UnityEngine;
using System;

public enum ItemID
{
    GlowingMushroom,
    SmellyRoot,
    IceMint,
    MischiefBerry,
    SilentStepPotion,
    MeatBombPotion,
    HeatResistPotion,
    SmokePowder
}

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance { get; private set; }

    // Dùng Dictionary để quản lý số lượng
    public Dictionary<ItemID, int> inventory = new Dictionary<ItemID, int>();
    public Action<ItemID, int> OnInventoryChanged;
    public Action OnInventoryContentsChanged;

    public IReadOnlyDictionary<ItemID, int> Items => inventory;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(ItemID id, int amount = 1)
    {
        if (inventory.ContainsKey(id))
            inventory[id] += amount;
        else
            inventory.Add(id, amount);
            
        Debug.Log($"[Inventory] Đã nhặt: {id} x{amount}. Tổng: {inventory[id]}");
        OnInventoryChanged?.Invoke(id, inventory[id]);
        OnInventoryContentsChanged?.Invoke();
    }

    public bool HasItem(ItemID id, int amount = 1)
    {
        return inventory.ContainsKey(id) && inventory[id] >= amount;
    }

    public int GetItemCount(ItemID id)
    {
        return inventory.TryGetValue(id, out int count) ? count : 0;
    }

    public bool RemoveItem(ItemID id, int amount = 1)
    {
        if (HasItem(id, amount))
        {
            inventory[id] -= amount;
            if (inventory[id] <= 0)
                inventory.Remove(id);
                
            Debug.Log($"[Inventory] Đã dùng: {id} x{amount}.");
            OnInventoryChanged?.Invoke(id, inventory.ContainsKey(id) ? inventory[id] : 0);
            OnInventoryContentsChanged?.Invoke();
            return true;
        }
        return false;
    }

    // Nếu ngất xỉu, có thể xoá một số đồ chưa lưu trữ.
    public void ClearUnsavedItems()
    {
        // Tuỳ theo thiết kế, MVP reset hết.
        inventory.Clear();
        Debug.Log("[Inventory] Mất toàn bộ đồ do ngất xỉu.");
        OnInventoryChanged?.Invoke(ItemID.GlowingMushroom, 0); // Demo trigger update UI
        OnInventoryContentsChanged?.Invoke();
    }
}
