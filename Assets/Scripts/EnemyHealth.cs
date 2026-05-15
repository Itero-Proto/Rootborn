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
    public AudioClip deathSound;
    public GameObject deathVfxPrefab;
    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.1f, 0.1f);
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
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        if (deathVfxPrefab != null)
        {
            Instantiate(deathVfxPrefab, transform.position, Quaternion.identity);
        }
        SpawnDrop();

        anim.SetTrigger("Die");

        GetComponent<EnemyController>().enabled = false;

        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null)
            shooter.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Vector3.zero;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        this.enabled = false;

        Destroy(gameObject, 2f);
    }
}