using UnityEngine;
using UnityEngine.UI; // Cần dùng thư viện UI của Unity (Text, Image)

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject explorationUI;
    public GameObject craftingUI;
    public GameObject endOfDayUI;

    [Header("End Of Day Info")]
    public Text dayText;       // Hiển thị ngày (vd: Day 2)
    public Text goldText;      // Hiển thị tổng vàng

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleStateChanged;
            GameManager.Instance.OnDayEnded += UpdateEndOfDayUI;
        }
        
        // Hide all initially
        explorationUI?.SetActive(false);
        craftingUI?.SetActive(false);
        endOfDayUI?.SetActive(false);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleStateChanged;
            GameManager.Instance.OnDayEnded -= UpdateEndOfDayUI;
        }
    }

    private void HandleStateChanged(GameManager.GameState newState)
    {
        explorationUI?.SetActive(newState == GameManager.GameState.Exploration);
        craftingUI?.SetActive(newState == GameManager.GameState.Crafting);
        endOfDayUI?.SetActive(newState == GameManager.GameState.Management);

        if (newState == GameManager.GameState.Management)
        {
            UpdateEndOfDayUI(GameManager.Instance.CurrentDayCount);
        }
    }

    private void UpdateEndOfDayUI(int currentDay)
    {
        if (dayText != null) dayText.text = $"CUỐI NGÀY {currentDay}";
        if (goldText != null && EconomyManager.Instance != null) 
            goldText.text = $"Tổng Vàng: {EconomyManager.Instance.CurrentGold}";
    }

    // Button Event for "Next Day" button on EndOfDayUI
    public void OnClickNextDay()
    {
        GameManager.Instance?.StartNextDay();
    }
}
