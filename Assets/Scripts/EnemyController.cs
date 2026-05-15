using UnityEngine;

public enum EnemyState
{
    Wander,
    ChasePlayer,
    Attack
}

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour
{
    private float attackFailSafeTimer = 0f;
    private bool hasTarget => player != null && !GameManager.GameEnded;
    private Animator anim;
    [Header("Targets")]
    public Transform player;
    [Header("Spawn Move")]
    public float spawnMoveDuration = 2f;
    public float spawnMoveSpeed = 4f;

    private float spawnTimer;
    private bool isInSpawnMove = true;
    private Vector3 spawnTarget;
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;

    [Header("Behavior Weights")]
    [Range(0, 1)] public float chasePlayerChance = 0.5f;
    private bool isAttacking;
    private Rigidbody rb;
    private EnemyState state;

    private Vector3 wanderDir;
    private float changeDirTimer;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        ChooseInitialState();
        PickNewWanderDirection();
        spawnTimer = spawnMoveDuration;

        // 👉 цель один раз фиксируем
        if (player != null)
            spawnTarget = player.position;
        else
            spawnTarget = transform.position + transform.forward * 3f;
    }
    void Update()
    {
        if (GameManager.GameEnded)
        {
            isAttacking = false;
            state = EnemyState.Wander;
            rb.linearVelocity = Vector3.zero;
            anim.SetFloat("Speed", 0);
            return;
        }

        if (isAttacking)
        {
            attackFailSafeTimer -= Time.deltaTime;

            if (attackFailSafeTimer <= 0f)
                EndAttacking();
        }

        if (isInSpawnMove)
        {
            SpawnMove();
            return;
        }

        switch (state)
        {
            case EnemyState.Wander:
                Wander();
                break;

            case EnemyState.ChasePlayer:
                if (!hasTarget)
                {
                    Wander();
                    return;
                }

                Chase(player.position);
                break;
        }

        if (!GameManager.GameEnded)
            DecideState();
    }
    void ResetAI()
    {
        isAttacking = false;
        state = EnemyState.Wander;
        rb.linearVelocity = Vector3.zero;
        anim.SetFloat("Speed", 0);
    }
    void OnEnable()
    {
        GameManager.OnGameEnded += ResetAI;
    }
    void OnDisable()
    {
        GameManager.OnGameEnded -= ResetAI;
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
    }

    void ChooseInitialState()
    {
        float r = Random.value;

        if (r < chasePlayerChance)
            state = EnemyState.ChasePlayer;
        else
            state = EnemyState.Wander;
    }
    void SpawnMove()
    {
        spawnTimer -= Time.deltaTime;

        Vector3 dir = (spawnTarget - transform.position).normalized;
        dir.y = 0;

        // движение
        Vector3 velocity = dir * spawnMoveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, 0, velocity.z);

        // поворот
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        anim.SetFloat("Speed", dir.magnitude);

        if (spawnTimer <= 0f)
        {
            isInSpawnMove = false;
        }
    }
    void DecideState()
    {
        if (!hasTarget) return;

        float distPlayer = Vector3.Distance(transform.position, player.position);

        if (distPlayer < detectionRange)
            state = EnemyState.ChasePlayer;
    }
    void Wander()
    {
        changeDirTimer -= Time.deltaTime;

        if (changeDirTimer <= 0f)
        {
            PickNewWanderDirection();
        }

        Move(wanderDir);
    }

    void PickNewWanderDirection()
    {
        wanderDir = new Vector3(
            Random.Range(-1f, 1f),
            0,
            Random.Range(-1f, 1f)
        ).normalized;

        changeDirTimer = Random.Range(1f, 3f);
    }

    // ---------------- CHASE ----------------

    void Chase(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;

        Move(dir);
    }

    // ---------------- MOVE ----------------
    void Move(Vector3 dir)
    {
        // ✅ поворот ВСЕГДА работает
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);

        // ✅ во время атаки только стопаем движение
        if (isAttacking)
        {
            rb.linearVelocity = Vector3.zero;
            anim.SetFloat("Speed", 0);
            return;
        }

        anim.SetFloat("Speed", dir.magnitude);

        Vector3 velocity = dir * moveSpeed;
        rb.linearVelocity = new Vector3(velocity.x, 0, velocity.z);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DangerZone"))
        {
            GetComponent<EnemyHealth>().Die();
        }
    }
    public void AlertToPlayer()
    {
        state = EnemyState.ChasePlayer;
    }
    public void StartAttacking()
    {
        if (GameManager.GameEnded) return;

        isAttacking = true;
        attackFailSafeTimer = 1.25f;
    }
    public void EndAttacking()
    {
        isAttacking = false;
    }
}