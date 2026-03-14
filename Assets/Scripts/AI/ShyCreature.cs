using UnityEngine;

public class ShyCreature : CreatureAI
{
    [Header("Tuning Di Chuyển Sóc Đuôi Đèn")]
    public float walkSpeed = 2f;
    public float fleeSpeed = 6f;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;

    [Header("Tuning Cảm Biến")]
    public float detectionRadius = 8f; // Khoảng cách nghe thấy tiếng động người chơi
    public float fleeRadius = 4f;    // Khoảng cách quá gần sẽ hoảng sợ bỏ chạy
    public LayerMask playerLayer;

    private float timer;
    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();
        timer = wanderTimer;
    }

    private void Start()
    {
        ChangeState(CreatureState.Wander);
        
        // MVP: Tự động tìm Player
        PlayerController p = FindObjectOfType<PlayerController>();
        if (p != null) playerTransform = p.transform;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Exploration)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;
        CheckSensors();

        switch (CurrentState)
        {
            case CreatureState.Wander:
                DoWander();
                break;
            case CreatureState.Alert:
                DoAlert();
                break;
            case CreatureState.Flee:
                DoFlee();
                break;
            case CreatureState.Curious:
                // Chưa triển khai vật phẩm mồi nhử trong MVP này
                break;
        }
    }

    private void CheckSensors()
    {
        if (playerTransform == null) return;

        float distToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (distToPlayer <= fleeRadius)
        {
            ChangeState(CreatureState.Flee);
        }
        else if (distToPlayer <= detectionRadius && CurrentState != CreatureState.Flee)
        {
            // Kiểm tra xem người chơi có đang dùng Thuốc Bước Chân Êm không
            bool hasSilentStepMode = false; // Tương lai tích hợp Inventory check effect
            
            if (!hasSilentStepMode)
            {
                ChangeState(CreatureState.Alert);
            }
        }
        else if (distToPlayer > detectionRadius && CurrentState == CreatureState.Alert)
        {
            ChangeState(CreatureState.Wander);
        }
    }

    private void DoWander()
    {
        agent.speed = walkSpeed;
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    private void DoAlert()
    {
        // Dừng lại nghe ngóng, hướng mặt về phía người chơi
        agent.isStopped = true;
        
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        direction.y = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
    }

    private void DoFlee()
    {
        agent.isStopped = false;
        agent.speed = fleeSpeed;

        Vector3 fleeDirection = (transform.position - playerTransform.position).normalized;
        Vector3 fleePos = transform.position + fleeDirection * fleeRadius * 2f;
        
        agent.SetDestination(fleePos);
    }

    // Tiện ích lấy điểm ngẫu nhiên trên NavMesh
    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        UnityEngine.AI.NavMeshHit navHit;
        UnityEngine.AI.NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    public override void ChangeState(CreatureState newState)
    {
        if (CurrentState == newState) return;
        base.ChangeState(newState);

        if (newState == CreatureState.Alert)
        {
            Debug.Log($"[{gameObject.name}] nghe thấy tiếng động! Cảnh giác.");
            // Hiện icon (?) cảnh báo trên đầu
        }
        else if (newState == CreatureState.Flee)
        {
            Debug.Log($"[{gameObject.name}] hoảng sợ bỏ chạy!");
            // Hiện icon (!) đỏ
        }
    }
}
