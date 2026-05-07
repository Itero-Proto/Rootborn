using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public GameObject organicDropPrefab;
    public GameObject inorganicDropPrefab;
    private Animator anim;
    public AudioClip hitSound;
    private AudioSource audioSource;
    public GameObject hitVfxPrefab;
    public AudioClip deathSound;
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // 💥 camera shake
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.1f, 0.1f);
        }

        // 🔥 VFX
        if (hitVfxPrefab != null)
        {
            Instantiate(hitVfxPrefab, transform.position, Quaternion.identity);
        }

        if (hitSound != null)
        {
            audioSource.PlayOneShot(hitSound, 1f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            anim.SetTrigger("Hit");
        }
    }
    void SpawnDrop()
    {
        GameObject prefab;

        if (Random.value < 0.5f)
            prefab = organicDropPrefab;
        else
            prefab = inorganicDropPrefab;

        Instantiate(prefab, transform.position, Quaternion.identity);
    }
    public void Die()
    {
        // 🔊 Звук смерти (не привязан к объекту)
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        SpawnDrop();

        anim.SetTrigger("Die");

        // ❗ Отключаем ВСЁ поведение
        GetComponent<EnemyController>().enabled = false;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = false;

        // ❗ Останавливаем физику
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        // ❗ (опционально) отключить коллайдер
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        // ❗ Сам скрипт тоже можно выключить
        this.enabled = false;

        Destroy(gameObject, 2f);
    }
}