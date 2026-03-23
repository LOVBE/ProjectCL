using UnityEngine;
using System;

public class EconomyManager : MonoBehaviour
{
    public static EconomyManager Instance { get; private set; }

    [Header("Tài chính")]
    public int CurrentGold;
    public int WeeklyTaxThreshold = 500;
    
    [Header("Tuần")]
    public int TaxCycleDays = 7;
    private int daysToTax;

    public int DaysToTax => daysToTax;

    public Action<int> OnGoldChanged;
    public Action<int> OnDaysToTaxChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        daysToTax = Mathf.Max(1, TaxCycleDays);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnDayEnded += HandleDayEnd;
        }

        NotifyGoldChanged();
        NotifyDaysToTaxChanged();
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
        NotifyGoldChanged();
    }

    public bool SpendGold(int amount)
    {
        if (CurrentGold >= amount)
        {
            CurrentGold -= amount;
            Debug.Log($"[EconomyManager] Đã tiêu {amount} Vàng. Còn: {CurrentGold}");
            NotifyGoldChanged();
            return true;
        }
        else
        {
            Debug.LogWarning("[EconomyManager] Tiền không đủ!");
            return false;
        }
    }

    public int ApplyFaintPenalty(int amount)
    {
        if (amount <= 0 || CurrentGold <= 0)
        {
            return 0;
        }

        int actualPenalty = Mathf.Min(CurrentGold, amount);
        CurrentGold -= actualPenalty;
        Debug.LogWarning($"[EconomyManager] Bị trừ {actualPenalty} Vàng do ngất xỉu. Còn: {CurrentGold}");
        NotifyGoldChanged();
        return actualPenalty;
    }

    private void HandleDayEnd(int currentDay)
    {
        daysToTax--;
        Debug.Log($"[EconomyManager] Còn {daysToTax} ngày nữa đóng thuế phòng thí nghiệm.");
        NotifyDaysToTaxChanged();

        if (daysToTax <= 0)
        {
            ProcessWeeklyTax();
            daysToTax = TaxCycleDays;
            NotifyDaysToTaxChanged();
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
            GameManager.Instance?.TriggerGameOver("Không đủ vàng để đóng thuế phòng thí nghiệm.");
        }
    }

    private void NotifyGoldChanged()
    {
        OnGoldChanged?.Invoke(CurrentGold);
    }

    private void NotifyDaysToTaxChanged()
    {
        OnDaysToTaxChanged?.Invoke(daysToTax);
    }
}
