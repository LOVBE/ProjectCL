using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Tài chính")]
    public int CurrentGold;
    public int WeeklyTaxThreshold = 500;
    
    [Header("Tuần")]
    public int TaxCycleDays = 7;
    private int daysToTax;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        daysToTax = TaxCycleDays;
        GameManager.Instance.OnDayEnded += HandleDayEnd;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnDayEnded -= HandleDayEnd;
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        Debug.Log($"[EconomyManager] Thu được {amount} Vàng. Tổng: {CurrentGold}");
    }

    public bool SpendGold(int amount)
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            Debug.Log($"[EconomyManager] Đã tiêu {amount} Vàng. Còn: {CurrentGold}");
            return true;
        }
        else
        {
            Debug.LogWarning("[EconomyManager] Tiền không đủ!");
            return false;
        }
    }

    private void HandleDayEnd(int currentDay)
    {
        daysToTax--;
        Debug.Log($"[EconomyManager] Còn {daysToTax} ngày nữa đóng thuế phòng thí nghiệm.");

        if (daysToTax <= 0)
        {
            ProcessWeeklyTax();
            daysToTax = TaxCycleDays;
        }
    }

    private void ProcessWeeklyTax()
    {
        Debug.Log("[EconomyManager] == NGÀY ĐÓNG THUẾ ==");
        if (SpendGold(WeeklyTaxThreshold))
        {
            Debug.Log($"[EconomyManager] Đã đóng thành công {WeeklyTaxThreshold} Vàng tiền thuế mướn phòng!");
        }
        else
        {
            Debug.LogError($"[EconomyManager] KHÔNG THỂ ĐÓNG THUẾ! GAME OVER (Vỡ nợ).");
            // Kích hoạt Game Over hoặc mất đồ tại đây
        }
    }
}
