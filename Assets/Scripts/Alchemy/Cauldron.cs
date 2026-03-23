using System;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : MonoBehaviour, IInteractable
{
    private const float AmbientTemperature = 20f;

    [Header("Tuning Lò Đun")]
    public float currentTemperature = 20f;
    public float maxSafeTemperature = 150f;
    public float heatIncreaseRate = 10f;
    public float coolingRate = 5f;

    [Header("Trạng thái")]
    public bool isFireOn = false;
    private List<ItemID> currentIngredients = new List<ItemID>();

    [Header("Visual")]
    [SerializeField] private Renderer cauldronRenderer;
    [SerializeField] private Color highlightColor = new Color(0.45f, 0.9f, 0.55f, 1f);

    private Color originalColor;
    private bool isHighlighted;

    public float CurrentTemperature => currentTemperature;
    public float MaxSafeTemperature => maxSafeTemperature;
    public bool IsFireOn => isFireOn;
    public IReadOnlyList<ItemID> CurrentIngredients => currentIngredients;

    public Action OnCauldronStateChanged;
    public Action<ItemID> OnPotionCreated;

    private void Awake()
    {
        CacheRenderer();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Crafting)
        {
            if (isFireOn)
            {
                isFireOn = false;
                NotifyStateChanged();
            }
            return;
        }

        HandleTemperature();
        CheckRecipesContinuously();
    }

    public void ToggleFire()
    {
        isFireOn = !isFireOn;
        Debug.Log($"[Cauldron] Bếp lửa: {(isFireOn ? "BẬT" : "TẮT")}.");
        NotifyStateChanged();
    }

    private void HandleTemperature()
    {
        float previousTemperature = currentTemperature;

        if (isFireOn)
        {
            currentTemperature += heatIncreaseRate * Time.deltaTime;
        }
        else if (currentTemperature > AmbientTemperature)
        {
            currentTemperature -= coolingRate * Time.deltaTime;
        }

        currentTemperature = Mathf.Max(AmbientTemperature, currentTemperature);

        // Logic Nổ lò
        if (currentTemperature > maxSafeTemperature)
        {
            ExplodeCauldron();
            return;
        }

        if (!Mathf.Approximately(previousTemperature, currentTemperature))
        {
            NotifyStateChanged();
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
            currentTemperature = Mathf.Max(AmbientTemperature, currentTemperature - 30f);
            Debug.Log("[Cauldron] Thả IceMint, nhiệt độ giảm mạnh!");
        }

        NotifyStateChanged();
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
        currentTemperature = AmbientTemperature;
        OnPotionCreated?.Invoke(potionID);
        NotifyStateChanged();
    }

    private void ExplodeCauldron()
    {
        Debug.LogError("[Cauldron] BÙM! Vạc quá nhiệt, phát nổ!");
        
        currentIngredients.Clear();
        isFireOn = false;
        currentTemperature = AmbientTemperature;
        NotifyStateChanged();

        // Trừ thể lực hoặc phạt ép end day
        GameManager.Instance?.OnPlayerFainted("Tai nạn phòng thí nghiệm (Vạc nổ)");
    }

    public void Interact(GameObject interactor)
    {
        GameManager.Instance?.ChangeState(GameManager.GameState.Crafting);
    }

    public void Highlight()
    {
        CacheRenderer();
        if (isHighlighted || cauldronRenderer == null)
        {
            return;
        }

        isHighlighted = true;
        cauldronRenderer.material.color = highlightColor;
    }

    public void RemoveHighlight()
    {
        CacheRenderer();
        if (!isHighlighted || cauldronRenderer == null)
        {
            return;
        }

        isHighlighted = false;
        cauldronRenderer.material.color = originalColor;
    }

    private void CacheRenderer()
    {
        if (cauldronRenderer == null)
        {
            cauldronRenderer = GetComponent<Renderer>();
        }

        if (cauldronRenderer != null && !isHighlighted)
        {
            originalColor = cauldronRenderer.material.color;
        }
    }

    private void NotifyStateChanged()
    {
        OnCauldronStateChanged?.Invoke();
    }
}
