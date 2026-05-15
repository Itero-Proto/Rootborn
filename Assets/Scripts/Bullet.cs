using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 1;

    [Header("VFX")]
    public GameObject hitVfxPrefab;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // ❌ не сталкиваемся сами с собой
        if (other.CompareTag("Player"))
            return;

        // 💥 точка попадания
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        // 💥 эффект попадания
        if (hitVfxPrefab != null)
        {
            Instantiate(hitVfxPrefab, hitPoint, Quaternion.identity);
        }

        // 👹 враг
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            EnemyController controller = other.GetComponent<EnemyController>();

            if (controller != null)
            {
                controller.AlertToPlayer();
            }
        }

        if (other.CompareTag("Tree"))
        {
            // просто уничтожаем
        }

        Destroy(gameObject);
    }
}