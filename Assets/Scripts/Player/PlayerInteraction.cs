using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Tuning Tương Tác")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Transform playercamera;

    private IInteractable currentHoveredItem;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Exploration)
            return;

        HandleCrosshairRaycast();

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    [SerializeField] private Transform raycastOrigin; // Thêm vị trí gốc bắn tia (vd: Mắt của Player)

    private void HandleCrosshairRaycast()
    {
        Camera mainCam = playercamera.GetComponent<Camera>();
        if (mainCam == null)
        {
            Debug.LogError("[LỖI] Vật gắn vào ô PlayerCamera không phải là Camera! Lấy tạm Camera.main");
            mainCam = Camera.main;
        }

        // 1. Tìm điểm giao cắt của Camera nhìn thứ 3 (Nhìn từ sau lưng)
        Ray camRay = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;
        
        // Bắn một tia nháp từ Camera để xem tâm nhìn đang rọi vào đâu (Đất, Tường, hay Hư không)
        if (Physics.Raycast(camRay, out RaycastHit camHit, interactDistance * 2))
        {
            targetPoint = camHit.point;
        }
        else
        {
            targetPoint = camRay.origin + camRay.direction * (interactDistance * 2);
        }

        // 2. Tính toán tia bắn THỰC SỰ: Từ mắt nhân vật (Góc nhìn thứ 1) hướng tới mục tiêu
        Vector3 originPos = (raycastOrigin != null) ? raycastOrigin.position : transform.position + Vector3.up * 0.6f;
        Vector3 aimDirection = (targetPoint - originPos).normalized;
        Ray interactRay = new Ray(originPos, aimDirection);

        // Vẽ tia đỏ liên tục để nhìn trong cửa sổ Scene
        Debug.DrawRay(interactRay.origin, interactRay.direction * interactDistance, Color.yellow);

        // Debug 1: Check xem có tia nào bắn trúng BẤT CỨ thứ gì không (KHÔNG LỌC LAYER)
        if (Physics.Raycast(interactRay, out RaycastHit anyHit, interactDistance))
        {
            if (anyHit.collider.gameObject.layer != LayerMask.NameToLayer("Interactable"))
            {
                // Nếu trúng mà khác layer Interactable, in ra cảnh báo để biết tia đang bị thứ gì chặn lại
                Debug.Log($"[Debug Tia] Tia Raycast đang bị chặn bởi: {anyHit.collider.gameObject.name} (Thuộc Layer: {LayerMask.LayerToName(anyHit.collider.gameObject.layer)}). Mất tín hiệu tới đồ vật phía sau!");
            }
        }
        else
        {
            Debug.Log("[Debug Tia] Tia Raycast đang bắn vào vùng không gian rỗng, không trúng bất kỳ thứ gì trong phạm vi 10 mét.");
        }

        // Kịch bản Cốt lõi: Lọc tia chỉ trúng Layer
        if (Physics.Raycast(interactRay, out RaycastHit hit, interactDistance, interactableLayer))
        {
            Debug.Log($"[Highlight] Tia ĐÃ TRÚNG Layer thành công vào đối tượng: {hit.collider.gameObject.name}!");
            
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            
            // Xử lý Highlight khi lia tâm chuột vào
            if (interactable != currentHoveredItem)
            {
                if (currentHoveredItem != null) currentHoveredItem.RemoveHighlight();
                currentHoveredItem = interactable;
                
                if (currentHoveredItem != null)
                {
                    Debug.Log($"[Highlight] Đã kích hoạt đổi màu xanh lá cho {hit.collider.gameObject.name}!");
                    currentHoveredItem.Highlight();
                }
                else
                {
                    Debug.LogError($"[LỖI] Vật {hit.collider.gameObject.name} có Layer đúng nhưng BẠN QUÊN KHÔNG GẮN SCRIPT 'PickupItem' cho nó!");
                }
            }
        }
        else
        {
            // Mất Focus
            if (currentHoveredItem != null)
            {
                Debug.Log("[Highlight] Đã lia góc nhìn ra chỗ khác, hủy màu xanh lá.");
                currentHoveredItem.RemoveHighlight();
                currentHoveredItem = null;
            }
        }
    }

    private void TryInteract()
    {
        if (currentHoveredItem != null)
        {
            currentHoveredItem.Interact(this.gameObject);
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
