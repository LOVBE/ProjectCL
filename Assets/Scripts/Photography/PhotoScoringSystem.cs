using UnityEngine;

public class PhotoScoringSystem : MonoBehaviour
{
    public static PhotoScoringSystem Instance { get; private set; }

    [Header("Tuning Trọng Số Chấm Điểm")]
    [SerializeField] private float maxDistanceScore = 40f;
    [SerializeField] private float maxAngleScore = 30f;
    [SerializeField] private float maxBehaviorScore = 30f;
    
    [Header("Tuning Khoảng Cách")]
    [SerializeField] private float optimalDistanceMin = 3f;
    [SerializeField] private float optimalDistanceMax = 6f;
    [SerializeField] private float maxValidDistance = 15f;

    public void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public int CalculateScore(CreatureAI creature, Vector3 cameraPosition, Vector3 cameraForward, float hitDistance)
    {
        Debug.Log($"[Scoring] Đang chấm điểm bức ảnh chụp: {creature.gameObject.name}");

        float distanceScore = CalculateDistanceScore(hitDistance);
        float angleScore = CalculateAngleScore(cameraForward, creature.transform.forward);
        float behaviorScore = CalculateBehaviorScore(creature.CurrentState);

        int totalScore = Mathf.RoundToInt(distanceScore + angleScore + behaviorScore);
        
        Debug.Log($"[Scoring] Kết quả: Khoảng cách ({distanceScore}) + Góc độ ({angleScore}) + Hành vi ({behaviorScore}) = {totalScore}");
        return totalScore;
    }

    private float CalculateDistanceScore(float distance)
    {
        if (distance >= optimalDistanceMin && distance <= optimalDistanceMax)
            return maxDistanceScore; // Khoảng cách hoàn hảo
            
        if (distance > maxValidDistance)
            return 0f; // Quá xa

        // Tính nội suy trượt giảm điểm nếu quá gần hoặc không nằm trong ngưỡng tối ưu
        if (distance < optimalDistanceMin)
        {
            return maxDistanceScore * (distance / optimalDistanceMin);
        }
        else
        {
            return maxDistanceScore * (1f - ((distance - optimalDistanceMax) / (maxValidDistance - optimalDistanceMax)));
        }
    }

    private float CalculateAngleScore(Vector3 camForward, Vector3 targetForward)
    {
        // camForward và targetForward đối đỉnh nhau -> chụp thẳng mặt (Dot = -1)
        // Cùng chiều -> Chụp sau lưng (Dot = 1)
        float dotProduct = Vector3.Dot(camForward, targetForward);

        // Map giá trị từ [-1, 1] sang trượt tuyến tính [maxScore, 0]
        // -1 (Mặt) -> MaxScore
        // 0 (Ngang hông) -> MaxScore / 2
        // 1 (Lưng) -> 0
        float alignmentNormalized = (1f - dotProduct) / 2f; 
        
        return maxAngleScore * alignmentNormalized;
    }

    private float CalculateBehaviorScore(CreatureState state)
    {
        switch (state)
        {
            case CreatureState.Flee:
                return 0f; // Bỏ chạy không có điểm
            case CreatureState.Alert:
                return maxBehaviorScore * 0.5f; // Đang cảnh giác, điểm TB
            case CreatureState.Wander:
                return maxBehaviorScore; // Tự nhiên, điểm tối đa
            case CreatureState.Curious:
                return maxBehaviorScore * 1.5f; // Bonus chụp lúc bị dụ dỗ
            default:
                return 0f;
        }
    }
}
