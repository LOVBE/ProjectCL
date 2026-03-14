using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour
{
    [Header("Tuning Lò Đun")]
    public float currentTemperature = 20f;
    public float maxSafeTemperature = 150f;
    public float heatIncreaseRate = 10f;
    public float coolingRate = 5f;

    [Header("Trạng thái")]
    public bool isFireOn = false;
    private List<ItemID> currentIngredients = new List<ItemID>();

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Crafting)
        {
            isFireOn = false;
            return;
        }

        HandleTemperature();
        CheckRecipesContinuously();
    }

    public void ToggleFire()
    {
        isFireOn = !isFireOn;
        Debug.Log($"[Cauldron] Bếp lửa: {(isFireOn ? "BẬT" : "TẮT")}.");
    }

    private void HandleTemperature()
    {
        if (isFireOn)
        {
            currentTemperature += heatIncreaseRate * Time.deltaTime;
        }
        else if (currentTemperature > 20f)
        {
            currentTemperature -= coolingRate * Time.deltaTime;
        }

        // Logic Nổ lò
        if (currentTemperature > maxSafeTemperature)
        {
            ExplodeCauldron();
        }

        // Đổi màu nước (Visual Feedbacks - Mockup)
        // Renderer.material.color = Color.Lerp(Color.blue, Color.red, currentTemperature/maxSafeTemperature);
    }

    public void AddIngredient(ItemID item)
    {
        currentIngredients.Add(item);

        // Hiệu ứng vật lý (Ví dụ thả lá Bạc Hà giảm nhiệt độ)
        if (item == ItemID.IceMint)
        {
            currentTemperature = Mathf.Max(20f, currentTemperature - 30f);
            Debug.Log("[Cauldron] Thả IceMint, nhiệt độ giảm mạnh!");
        }
    }

    private void CheckRecipesContinuously()
    {
        if (currentIngredients.Count == 0) return;

        // Công thức: Bom Mùi (1 Nấm Phát Quang + 1 Rễ Cây Bốc Mùi) ở nhiệt độ > 80
        if (currentIngredients.Contains(ItemID.GlowingMushroom) && currentIngredients.Contains(ItemID.SmellyRoot))
        {
            if (currentTemperature >= 80f && currentTemperature <= 120f)
            {
                CreatePotion(ItemID.MeatBombPotion);
                return;
            }
        }

        // Công thức: Thuốc Kháng Nhiệt (1 IceMint + 1 Bất kỳ)
        if (currentIngredients.Contains(ItemID.IceMint) && currentIngredients.Count >= 2)
        {
            // Ice Mint nấu ở nhiệt độ thấp
            if (currentTemperature >= 30f && currentTemperature <= 60f)
            {
                CreatePotion(ItemID.HeatResistPotion);
                return;
            }
        }
    }

    private void CreatePotion(ItemID potionID)
    {
        Debug.Log($"[Cauldron] Giả kim THÀNH CÔNG! Tạo ra: {potionID}");
        PlayerInventory.Instance.AddItem(potionID, 1);
        
        // Clear vạc
        currentIngredients.Clear();
        isFireOn = false;
        currentTemperature = 20f;
    }

    private void ExplodeCauldron()
    {
        Debug.LogError("[Cauldron] BÙM! Vạc quá nhiệt, phát nổ!");
        
        currentIngredients.Clear();
        isFireOn = false;
        currentTemperature = 20f;

        // Trừ thể lực hoặc phạt ép end day
        GameManager.Instance?.OnPlayerFainted("Tai nạn phòng thí nghiệm (Vạc nổ)");
    }
}
