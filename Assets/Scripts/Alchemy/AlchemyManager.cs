using UnityEngine;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance { get; private set; }

    [SerializeField] private Cauldron mainCauldron;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Crafting)
            return;

        // MVP: Mô phỏng người chơi click UI để thả vật phẩm vào vạc (Bấm phím số để test)
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryDropItemIntoCauldron(ItemID.GlowingMushroom);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryDropItemIntoCauldron(ItemID.SmellyRoot);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryDropItemIntoCauldron(ItemID.IceMint);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryDropItemIntoCauldron(ItemID.MischiefBerry);
        
        // Bấm phím F để bật/tắt lửa đun vạc
        if (Input.GetKeyDown(KeyCode.F))
        {
            mainCauldron.ToggleFire();
        }
    }

    public void TryDropItemIntoCauldron(ItemID itemID)
    {
        if (PlayerInventory.Instance.HasItem(itemID, 1))
        {
            PlayerInventory.Instance.RemoveItem(itemID, 1);
            mainCauldron.AddIngredient(itemID);
            Debug.Log($"[Alchemy] Đã thả {itemID} vào Vạc.");
        }
        else
        {
            Debug.LogWarning($"[Alchemy] Không có {itemID} trong túi đồ!");
        }
    }
}
