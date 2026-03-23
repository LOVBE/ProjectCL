using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Exploration, Crafting, Management, GameOver }
    public GameState CurrentState { get; private set; }
    
    public int CurrentDayCount { get; private set; } = 1;
    public int LastCompletedDayCount { get; private set; }
    public string LastFaintReason { get; private set; } = string.Empty;
    public int LastFaintGoldPenalty { get; private set; }
    public string GameOverReason { get; private set; } = string.Empty;
    public bool HasEndedCurrentDay => hasEndedCurrentDay;
    public bool IsGameOver => CurrentState == GameState.GameOver;

    [Header("Faint Penalty")]
    [SerializeField] private int faintGoldPenalty = 30;
    [SerializeField] private bool clearInventoryOnFaint = true;

    [Header("Testing Events")]
    public Action<GameState> OnGameStateChanged;
    public Action<int> OnDayEnded;
    public Action<int> OnDayStarted;
    public Action<string> OnPlayerFaintedSignal; // Dùng khi hết the lực/Dưỡng khí
    public Action<string> OnGameOverSignal;

    private bool hasEndedCurrentDay;
    private bool hasInitializedState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            EnsureRuntimeSupportSystems();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.Exploration);
        OnDayStarted?.Invoke(CurrentDayCount);
    }

    public void ChangeState(GameState newState)
    {
        if (hasInitializedState && CurrentState == newState)
        {
            return;
        }

        hasInitializedState = true;
        CurrentState = newState;
        Debug.Log($"[GameManager] Chuyển trạng thái sang: {newState}");
        
        switch (newState)
        {
            case GameState.Exploration:
                // Setup thám hiểm, block chuột FPS
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
            case GameState.Crafting:
                // Mở khoá chuột để tương tác phòng thì nghiệm
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case GameState.Management:
                // Hiện bảng tổng kết và mở khoá chuột
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                EndDay();
                break;
            case GameState.GameOver:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    public void OnPlayerFainted(string reason)
    {
        if (CurrentState == GameState.GameOver || CurrentState == GameState.Management)
        {
            return;
        }

        LastFaintReason = reason;
        LastFaintGoldPenalty = EconomyManager.Instance != null
            ? EconomyManager.Instance.ApplyFaintPenalty(faintGoldPenalty)
            : 0;

        if (clearInventoryOnFaint)
        {
            PlayerInventory.Instance?.ClearUnsavedItems();
        }

        Debug.LogWarning($"[GameManager] Người chơi ngất xỉu vì: {reason}");
        OnPlayerFaintedSignal?.Invoke(reason);
        
        // Hết vòng lặp ngay lập tức và cưỡng chế qua Management hoặc Exploration ngày tiếp.
        // Tùy theo doc, phạt nếu ở Exploration. Chuyển thẳng tới Management để thông báo phạt.
        ChangeState(GameState.Management);
    }

    public void EndDay()
    {
        if (hasEndedCurrentDay)
        {
            return;
        }

        hasEndedCurrentDay = true;
        LastCompletedDayCount = CurrentDayCount;
        Debug.Log($"[GameManager] Kết thúc ngày {LastCompletedDayCount}.");
        OnDayEnded?.Invoke(LastCompletedDayCount);
        
        // MVP: có thể tự động vòng lặp lại Exploration (mô phỏng khi người chơi bấm nút "Next Day" trên UI).
        // Đối với thực tế, nên đợi người chơi bấm UI. Tạm thời MVP để UI gọi hàm NextDay().
    }
    
    public void StartNextDay()
    {
        if (CurrentState == GameState.GameOver)
        {
            return;
        }

        CurrentDayCount = Mathf.Max(CurrentDayCount + (hasEndedCurrentDay ? 0 : 1), LastCompletedDayCount + 1);
        hasEndedCurrentDay = false;
        LastFaintReason = string.Empty;
        LastFaintGoldPenalty = 0;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.ResetVitals();
        }

        ChangeState(GameState.Exploration);
        OnDayStarted?.Invoke(CurrentDayCount);
    }

    public void TriggerGameOver(string reason)
    {
        if (CurrentState == GameState.GameOver)
        {
            return;
        }

        GameOverReason = reason;
        Debug.LogError($"[GameManager] GAME OVER: {reason}");
        OnGameOverSignal?.Invoke(reason);
        ChangeState(GameState.GameOver);
    }

    private void EnsureRuntimeSupportSystems()
    {
        EnsureComponent<UIManager>();
        EnsureComponent<AlchemyManager>();

        if (GetComponent<EconomyManager>() == null)
        {
            gameObject.AddComponent<EconomyManager>();
        }
    }

    private T EnsureComponent<T>() where T : Component
    {
        T existing = GetComponent<T>();
        if (existing == null)
        {
            existing = gameObject.AddComponent<T>();
        }

        return existing;
    }
}
