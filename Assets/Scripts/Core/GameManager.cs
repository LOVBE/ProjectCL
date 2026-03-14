using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Exploration, Crafting, Management }
    public GameState CurrentState { get; private set; }
    
    public int CurrentDayCount { get; private set; } = 1;

    [Header("Testing Events")]
    public Action<GameState> OnGameStateChanged;
    public Action<int> OnDayEnded;
    public Action<string> OnPlayerFaintedSignal; // Dùng khi hết the lực/Dưỡng khí

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeState(GameState.Exploration);
    }

    public void ChangeState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"[GameManager] Chuyển trạng thái sang: {newState}");
        OnGameStateChanged?.Invoke(newState);
        
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
        }
    }

    public void OnPlayerFainted(string reason)
    {
        Debug.LogWarning($"[GameManager] Người chơi ngất xỉu vì: {reason}");
        OnPlayerFaintedSignal?.Invoke(reason);
        
        // Hết vòng lặp ngay lập tức và cưỡng chế qua Management hoặc Exploration ngày tiếp.
        // Tùy theo doc, phạt nếu ở Exploration. Chuyển thẳng tới Management để thông báo phạt.
        ChangeState(GameState.Management);
    }

    public void EndDay()
    {
        CurrentDayCount++;
        Debug.Log($"[GameManager] Kết thúc ngày. Ngày hiện tại: {CurrentDayCount}");
        OnDayEnded?.Invoke(CurrentDayCount);
        
        // MVP: có thể tự động vòng lặp lại Exploration (mô phỏng khi người chơi bấm nút "Next Day" trên UI).
        // Đối với thực tế, nên đợi người chơi bấm UI. Tạm thời MVP để UI gọi hàm NextDay().
    }
    
    public void StartNextDay()
    {
        ChangeState(GameState.Exploration);
    }
}
