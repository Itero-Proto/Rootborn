using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public float damage = 1f;
    public float attackCooldown = 1f;
    private float timer;

    private Animator anim;
    private AudioSource audioSource;

    public TreeSystem tree;

    [Header("Audio")]
    public AudioClip attackSound;

    [Header("VFX")]
    public GameObject hitVfxPrefab;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();

        tree = FindAnyObjectByType<TreeSystem>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
    }

    void Attack(Vector3 hitPoint)
    {
        anim.SetTrigger("Attack");

        // 🔊 звук атаки
        if (attackSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(attackSound, 0.5f);
        }

        // 💥 VFX
        if (hitVfxPrefab != null)
        {
            Instantiate(hitVfxPrefab, hitPoint, Quaternion.identity);
        }
    }

    public void DealDamage()
    {
        if (tree != null)
            tree.TakeDamage(damage);
    }

    void OnCollisionStay(Collision collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (timer <= 0f)
        {
            Vector3 hitPoint = collision.contacts[0].point;

            Attack(hitPoint);
            DealDamage(); // 👈 теперь урон реально применяется

            timer = attackCooldown;
        }
    }
}