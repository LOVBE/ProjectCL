using UnityEngine;

public class AlchemyManager : MonoBehaviour
{
    public static AlchemyManager Instance { get; private set; }

    [Header("Cauldron")]
    [SerializeField] private Cauldron mainCauldron;
    [SerializeField] private bool autoCreateCraftingStation = true;

    [Header("Sample Exploration Items")]
    [SerializeField] private bool autoSpawnSampleIngredients = true;
    [SerializeField] private float stationSpawnDistance = 4f;
    [SerializeField] private float samplePickupSpacing = 1.5f;

    public Cauldron MainCauldron => mainCauldron;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        EnsureMainCauldron();

        if (autoSpawnSampleIngredients)
        {
            EnsureSampleIngredientPickups();
        }
    }

    public bool TryDropItemIntoCauldron(ItemID itemID)
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Crafting)
        {
            return false;
        }

        EnsureMainCauldron();
        if (mainCauldron == null)
        {
            Debug.LogWarning("[Alchemy] Không tìm thấy Vạc để thao tác.");
            return false;
        }

        if (!CanUseAsIngredient(itemID))
        {
            Debug.LogWarning($"[Alchemy] {itemID} không phải nguyên liệu thả vào Vạc.");
            return false;
        }

        if (PlayerInventory.Instance == null || !PlayerInventory.Instance.HasItem(itemID, 1))
        {
            Debug.LogWarning($"[Alchemy] Không có {itemID} trong túi đồ!");
            return false;
        }

        PlayerInventory.Instance.RemoveItem(itemID, 1);
        mainCauldron.AddIngredient(itemID);
        Debug.Log($"[Alchemy] Đã thả {itemID} vào Vạc.");
        return true;
    }

    public void ToggleMainCauldronFire()
    {
        EnsureMainCauldron();
        if (mainCauldron != null)
        {
            mainCauldron.ToggleFire();
        }
    }

    public void ReturnToExploration()
    {
        GameManager.Instance?.ChangeState(GameManager.GameState.Exploration);
    }

    public void EndCraftingDay()
    {
        GameManager.Instance?.ChangeState(GameManager.GameState.Management);
    }

    public bool CanUseAsIngredient(ItemID itemID)
    {
        switch (itemID)
        {
            case ItemID.GlowingMushroom:
            case ItemID.SmellyRoot:
            case ItemID.IceMint:
            case ItemID.MischiefBerry:
                return true;
            default:
                return false;
        }
    }

    private void EnsureMainCauldron()
    {
        if (mainCauldron != null)
        {
            return;
        }

        mainCauldron = FindObjectOfType<Cauldron>();
        if (mainCauldron == null && autoCreateCraftingStation)
        {
            mainCauldron = CreateRuntimeCauldronStation();
        }
    }

    private void EnsureSampleIngredientPickups()
    {
        float groundY = 0.35f;
        PickupItem existingPickup = FindObjectOfType<PickupItem>();
        if (existingPickup != null)
        {
            groundY = existingPickup.transform.position.y;
        }

        Transform player = FindObjectOfType<PlayerController>()?.transform;
        Vector3 basePosition = player != null
            ? player.position + player.right * -3f + player.forward * 2f
            : new Vector3(-4f, groundY, -5f);
        basePosition.y = groundY;

        EnsurePickupExists(ItemID.GlowingMushroom, basePosition, new Color(0.3f, 0.9f, 0.45f), "Item_GlowingMushroom_Runtime");
        EnsurePickupExists(ItemID.SmellyRoot, basePosition + new Vector3(samplePickupSpacing, 0f, 0f), new Color(0.55f, 0.3f, 0.1f), "Item_SmellyRoot_Runtime");
        EnsurePickupExists(ItemID.IceMint, basePosition + new Vector3(samplePickupSpacing * 2f, 0f, 0f), new Color(0.4f, 0.85f, 1f), "Item_IceMint_Runtime");
        EnsurePickupExists(ItemID.MischiefBerry, basePosition + new Vector3(samplePickupSpacing * 3f, 0f, 0f), new Color(0.8f, 0.2f, 0.45f), "Item_MischiefBerry_Runtime");
    }

    private void EnsurePickupExists(ItemID itemID, Vector3 position, Color color, string objectName)
    {
        PickupItem[] pickups = FindObjectsOfType<PickupItem>();
        for (int i = 0; i < pickups.Length; i++)
        {
            if (pickups[i].ItemType == itemID)
            {
                return;
            }
        }

        GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pickupObject.name = objectName;
        pickupObject.layer = LayerMask.NameToLayer("Interactable");
        pickupObject.transform.position = position;
        pickupObject.transform.localScale = Vector3.one * 0.6f;

        PickupItem pickup = pickupObject.AddComponent<PickupItem>();
        pickup.Configure(itemID, 1, color);
    }

    private Cauldron CreateRuntimeCauldronStation()
    {
        GameObject cauldronObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cauldronObject.name = "Runtime_CauldronStation";
        cauldronObject.layer = LayerMask.NameToLayer("Interactable");

        Transform player = FindObjectOfType<PlayerController>()?.transform;
        Vector3 spawnPosition = player != null
            ? player.position + player.forward * stationSpawnDistance + player.right * 1.5f
            : new Vector3(3f, 0.5f, 3f);
        spawnPosition.y = 0.5f;

        cauldronObject.transform.position = spawnPosition;
        cauldronObject.transform.localScale = new Vector3(1.2f, 0.5f, 1.2f);

        Renderer renderer = cauldronObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = new Color(0.18f, 0.18f, 0.22f);
        }

        return cauldronObject.AddComponent<Cauldron>();
    }
}
