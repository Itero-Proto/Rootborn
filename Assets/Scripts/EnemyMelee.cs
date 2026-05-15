using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public float damage = 1f;
    public float attackCooldown = 1f;

    private float timer;

    private PlayerController player;
    private TreeHealth tree;

    private Animator anim;
    private AudioSource audioSource;
    private EnemyController controller;

    [Header("Audio")]
    public AudioClip attackSound;

    private bool isGameEnded;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        player = FindAnyObjectByType<PlayerController>();
        tree = FindAnyObjectByType<TreeHealth>();
        controller = GetComponent<EnemyController>();

        isGameEnded = false;
    }

    void Update()
    {
        if (isGameEnded) return;

        timer -= Time.deltaTime;
    }

    void OnCollisionStay(Collision collision)
    {
        if (isGameEnded) return;
        if (!collision.collider.CompareTag("Player")) return;
        if (player == null) return;

        if (timer <= 0f)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (isGameEnded) return;

        if (controller != null)
            controller.StartAttacking();

        if (anim != null)
            anim.SetTrigger("Attack");

        if (attackSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound, 0.5f);
        }

        timer = attackCooldown;
    }

    public void DealDamage()
    {
        if (isGameEnded) return;

        if (tree != null)
            tree.TakeDamage(damage);

        if (DamageFlash.Instance != null)
        {
            DamageFlash.Instance.Flash(
                new Color(0.35f, 0f, 0.35f, 0.4f)
            );
        }

        if (player != null)
            player.PlayHitFeedback();
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

        timer = attackCooldown;

        if (controller != null)
            controller.EndAttacking();
    }
}