using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EnemyShooter : MonoBehaviour
{
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 1.5f;
    public float attackRange = 10f;
    [Header("References")]
    public Transform player;
    [Header("Audio")]
    public AudioClip shootSound;
    public AudioClip enemySound;
    [Range(0f, 1f)]
    public float enemyVolume = 0.3f;
    public float minEnemySoundDelay = 4f;
    public float maxEnemySoundDelay = 10f;
    private float enemySoundTimer;

    private float timer;
    private Animator anim;
    private EnemyController controller;
    private AudioSource audioSource;
    private bool isAttacking;
    private float attackFailSafeTimer;

    private bool isGameEnded;

    void Start()
    {
        enemySoundTimer = Random.Range(minEnemySoundDelay, maxEnemySoundDelay);
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyController>();
        audioSource = GetComponent<AudioSource>();

        timer = 0f;
    }

    void Update()
    {
        if (isGameEnded) return;
        if (player == null || controller == null) return;
        enemySoundTimer -= Time.deltaTime;

        if (enemySoundTimer <= 0f)
        {
            EnemySound();

            enemySoundTimer = Random.Range(
                minEnemySoundDelay,
                maxEnemySoundDelay
            );
        }
        timer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        if (!isAttacking && dist < attackRange && timer <= 0f)
        {
            StartAttack();
        }

        if (isAttacking)
        {
            attackFailSafeTimer -= Time.deltaTime;

            if (attackFailSafeTimer <= 0f)
                EndAttack();
        }
    }

    void StartAttack()
    {
        if (isGameEnded) return;

        isAttacking = true;
        attackFailSafeTimer = 1f;

        controller.StartAttacking();
        anim.SetTrigger("Attack");

        timer = fireRate;
    }

    public void Shoot()
    {
        if (isGameEnded) return;

        if (bulletPrefab != null && shootPoint != null && player != null)
        {
            GameObject bullet = Instantiate(
                bulletPrefab,
                shootPoint.position,
                Quaternion.identity
            );

            Vector3 dir = (player.position - shootPoint.position).normalized;
            dir.y = 0f;

            bullet.transform.forward = dir;
        }

        if (shootSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound, 0.5f);
        }

        EndAttack();
    }

    void EndAttack()
    {
        isAttacking = false;

        if (controller != null)
            controller.EndAttacking();
    }

    void OnEnable()
    {
        GameManager.OnGameEnded += ResetAI;
    }

    void OnDisable()
    {
        GameManager.OnGameEnded -= ResetAI;
    }

    void ResetAI()
    {
        isGameEnded = true;

        isAttacking = false;
        timer = fireRate;
        attackFailSafeTimer = 0f;

        if (controller != null)
            controller.EndAttacking();
    }
    void EnemySound()
    {
        if (enemySound == null || audioSource == null)
            return;

        audioSource.pitch = Random.Range(0.9f, 1.1f);

        audioSource.PlayOneShot(
            enemySound,
            enemyVolume
        );
    }
}