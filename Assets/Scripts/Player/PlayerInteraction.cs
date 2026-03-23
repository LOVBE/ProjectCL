using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Tuning Tương Tác")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform playercamera;
    [SerializeField] private Transform raycastOrigin; // Thêm vị trí gốc bắn tia (vd: Mắt của Player)
    [SerializeField] private bool verboseRaycastLogging;

    private IInteractable currentHoveredItem;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Exploration)
        {
            ClearHoveredItem();
            return;
        }

        HandleCrosshairRaycast();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (verboseRaycastLogging) Debug.Log("[Input] Đã ghi nhận người chơi BẤM PHÍM E.");
            TryInteract();
        }
    }

    [Header("Tuning nội bộ")]
    [SerializeField] private float maxInteractDistance = 15f; // Khoảng cách tối đa tia tương tác (nên >= khoảng cách Camera)

    private void HandleCrosshairRaycast()
    {
        ResolveReferences();

        Camera mainCam = playercamera != null ? playercamera.GetComponent<Camera>() : null;
        if (mainCam == null)
        {
            mainCam = Camera.main;
        }

        if (mainCam == null)
        {
            if (verboseRaycastLogging) Debug.LogWarning("[Raycast] Không tìm thấy Camera nào!");
            return;
        }

        // ======== BƯỚC 1: Tia nháp từ Camera TPS xuyên qua Crosshair ========
        Ray camRay = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        float camProbeDistance = 100f; // Bắn xa 100m để chắc chắn chạm được vật thể dù Camera ở rất xa
        Vector3 targetPoint;
        
        // RaycastAll để quét xuyên qua cơ thể Player
        RaycastHit[] hits = Physics.RaycastAll(camRay, camProbeDistance);
        bool foundHit = false;
        RaycastHit bestHit = new RaycastHit();
        float minDist = float.MaxValue;

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.root == transform.root) continue;
            
            if (h.distance < minDist)
            {
                minDist = h.distance;
                bestHit = h;
                foundHit = true;
            }
        }

        if (foundHit)
        {
            targetPoint = bestHit.point;
            if (verboseRaycastLogging) Debug.Log($"[Tia Nháp] Camera bắn trúng '{bestHit.collider.gameObject.name}' tại {targetPoint} (khoảng cách {minDist:F1}m)");
        }
        else
        {
            targetPoint = camRay.origin + camRay.direction * camProbeDistance;
            if (verboseRaycastLogging) Debug.Log($"[Tia Nháp] Camera không trúng gì, đặt targetPoint ở xa {camProbeDistance}m");
        }

        // ======== BƯỚC 2: Tia tương tác THỰC SỰ từ mắt nhân vật tới targetPoint ========
        Vector3 originPos = (raycastOrigin != null) ? raycastOrigin.position : transform.position + Vector3.up * 0.6f;
        Vector3 aimDirection = (targetPoint - originPos).normalized;
        Ray interactRay = new Ray(originPos, aimDirection);

        // Vẽ 2 tia debug nhìn trong cửa sổ Scene
        Debug.DrawRay(camRay.origin, camRay.direction * camProbeDistance, Color.cyan); // Tia Camera (xanh dương)
        Debug.DrawRay(interactRay.origin, interactRay.direction * maxInteractDistance, Color.yellow); // Tia tương tác (vàng)

        if (verboseRaycastLogging)
        {
            Debug.Log($"[Tia Chính] Gốc: {originPos}, Hướng: {aimDirection}, TầmXa: {maxInteractDistance}m");
        }

        // ======== BƯỚC 3: Debug - kiểm tra tia có chạm BẤT CỨ thứ gì không ========
        if (Physics.Raycast(interactRay, out RaycastHit anyHit, maxInteractDistance))
        {
            if (verboseRaycastLogging)
            {
                Debug.Log($"[Debug Tia] Trúng '{anyHit.collider.gameObject.name}' (Layer: {LayerMask.LayerToName(anyHit.collider.gameObject.layer)}, Dist: {anyHit.distance:F1}m)");
            }
        }
        else if (verboseRaycastLogging)
        {
            Debug.Log($"[Debug Tia] Tia từ mắt nhân vật KHÔNG trúng bất kỳ thứ gì trong {maxInteractDistance}m!");
        }

        // ======== BƯỚC 4: Lọc theo Layer Interactable ========
        if (Physics.Raycast(interactRay, out RaycastHit hit, maxInteractDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            if (interactable != currentHoveredItem)
            {
                if (currentHoveredItem != null) currentHoveredItem.RemoveHighlight();
                currentHoveredItem = interactable;
                
                if (currentHoveredItem != null)
                {
                    currentHoveredItem.Highlight();
                    if (verboseRaycastLogging) Debug.Log($"[Highlight] Đang ngắm trúng: {hit.collider.gameObject.name}");
                }
                else
                {
                    Debug.LogError($"[LỖI] Vật {hit.collider.gameObject.name} có Layer đúng nhưng thiếu IInteractable!");
                }
            }
        }
        else
        {
            ClearHoveredItem();
        }
    }

    private void TryInteract()
    {
        if (currentHoveredItem != null)
        {
            if (verboseRaycastLogging) Debug.Log($"[Tương Tác] Đang gọi lệnh Interact() cho vật thể ngắm trúng!");
            IInteractable interactable = currentHoveredItem;
            ClearHoveredItem();
            interactable.Interact(this.gameObject);
        }
        else
        {
            if (verboseRaycastLogging) Debug.Log("[Tương Tác] Thất bại: Bấm E nhưng crosshair không bắt được IInteractable nào (LayerMask Nothing?).");
        }
    }

    private void ResolveReferences()
    {
        if (playercamera == null)
        {
            Camera childCamera = GetComponentInChildren<Camera>();
            if (childCamera != null)
            {
                playercamera = childCamera.transform;
            }
            else if (Camera.main != null)
            {
                playercamera = Camera.main.transform;
            }
        }

        if (raycastOrigin == null)
        {
            raycastOrigin = playercamera != null ? playercamera : transform;
        }
    }

    private void ClearHoveredItem()
    {
        if (currentHoveredItem != null)
        {
            currentHoveredItem.RemoveHighlight();
            currentHoveredItem = null; // Reset sau khi nhặt
        }
    }
}

public interface IInteractable
{
    void Interact(GameObject interactor);
    void Highlight();
    void RemoveHighlight();
}
