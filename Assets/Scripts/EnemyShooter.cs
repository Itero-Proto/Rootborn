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

    private float timer;
    private Animator anim;
    private EnemyController controller;
    private AudioSource audioSource;

    private bool isAttacking = false;
    private float attackFailSafeTimer = 0f;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        anim = GetComponentInChildren<Animator>();
        controller = GetComponent<EnemyController>();
        audioSource = GetComponent<AudioSource>();

        timer = 0f;
    }

    void Update()
    {
        if (player == null || controller == null) return;

        timer -= Time.deltaTime;

        float dist = Vector3.Distance(transform.position, player.position);

        // 🔥 Запуск атаки
        if (dist < attackRange && timer <= 0f && !isAttacking)
        {
            StartAttack();
        }

        // 🔧 Fail-safe (если анимация сломалась)
        if (isAttacking)
        {
            attackFailSafeTimer -= Time.deltaTime;

            if (attackFailSafeTimer <= 0f)
            {
                EndAttack();
            }
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        attackFailSafeTimer = 2f; // чуть больше длины анимации

        controller.StartAttacking(); // стоп движения
        anim.SetTrigger("Attack");

        timer = fireRate;
    }

    // 👉 ВЫЗЫВАЕТСЯ ИЗ ANIMATION EVENT
    public void Shoot()
    {
        if (bulletPrefab != null && shootPoint != null)
        {
            Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        }

        // 🔊 звук
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
}