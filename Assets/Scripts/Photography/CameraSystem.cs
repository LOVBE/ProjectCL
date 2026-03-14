using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    [Header("Tuning Khung Hình")]
    [SerializeField] private Transform cameraLens;
    [SerializeField] private float raycastDistance = 20f;
    [SerializeField] private LayerMask creatureLayer;
    [SerializeField] private LayerMask obstacleLayer;

    // MVP lưới đơn giản 3x3 bắn raycast
    [SerializeField] private int gridResolution = 3; 
    [SerializeField] private float spreadAngle = 5f;

    private bool isViewfinderOpen = false;

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Exploration)
            return;

        // Bật tắt Viewfinder
        if (Input.GetMouseButtonDown(1)) // M&M chuột phải
        {
            ToggleViewfinder(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            ToggleViewfinder(false);
        }

        // Bấm chụp
        if (isViewfinderOpen && Input.GetMouseButtonDown(0))
        {
            TakePhoto();
        }
    }

    private void ToggleViewfinder(bool state)
    {
        isViewfinderOpen = state;
        // Bật UI filter màn hình hoặc zoom camera field of view ở đây
        Debug.Log($"[Photography] Viewfinder: {(state ? "MỞ" : "ĐÓNG")}");
    }

    private void TakePhoto()
    {
        Debug.Log("[Photography] TẮCH! Bấm chụp.");

        // Bắn tia trung tâm để kiểm tra cản trở terrain
        Ray centerRay = new Ray(cameraLens.position, cameraLens.forward);
        if (Physics.Raycast(centerRay, out RaycastHit obsHit, raycastDistance, obstacleLayer))
        {
            Debug.LogWarning("[Photography] Góc chụp bị chặn bởi địa hình/vật cản lớn! Mất hình.");
            return; 
        }

        // Bắn lưới dò tìm sinh vật
        CreatureAI bestTarget = FindTargetInGrid(out float bestHitDistance);

        if (bestTarget != null)
        {
            int score = PhotoScoringSystem.Instance.CalculateScore(bestTarget, cameraLens.position, cameraLens.forward, bestHitDistance);
            
            // Tạm thời lưu điểm dưới dạng vàng demo hoặc đưa vào danh sách tổng kết
            EconomyManager.Instance?.AddGold(score); // Demo: Chuyển điểm thành Vàng luôn
        }
        else
        {
            Debug.Log("[Photography] Không trúng con sinh vật nào cả.");
        }
    }

    private CreatureAI FindTargetInGrid(out float hitDistance)
    {
        CreatureAI foundTarget = null;
        hitDistance = float.MaxValue;

        // Tạo lưới tia raycast dựa theo spreadAngle
        for (int x = -gridResolution / 2; x <= gridResolution / 2; x++)
        {
            for (int y = -gridResolution / 2; y <= gridResolution / 2; y++)
            {
                // Tính toán hướng lệch
                Vector3 direction = Quaternion.Euler(x * spreadAngle, y * spreadAngle, 0) * cameraLens.forward;
                Ray ray = new Ray(cameraLens.position, direction);

                if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, creatureLayer))
                {
                    CreatureAI creature = hit.collider.GetComponentInParent<CreatureAI>();
                    if (creature != null)
                    {
                        // Lấy mục tiêu gần trục giữa nhất (cho MVP ta lưu thằng đầu tiên hoặc gần nhất)
                        if (hit.distance < hitDistance)
                        {
                            hitDistance = hit.distance;
                            foundTarget = creature;
                        }
                    }
                }
            }
        }

        return foundTarget;
    }
}
