using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 1;

    public TreeSystem tree;

    [Header("VFX")]
    public GameObject hitVfxPrefab;

    [Header("Camera Shake")]
    public float shakeDuration = 0.08f;
    public float shakeMagnitude = 0.08f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        if (tree == null)
            tree = GameObject.FindGameObjectWithTag("Tree").GetComponent<TreeSystem>();

        Destroy(gameObject, lifeTime);
    }

    public void DealDamage()
    {
        if (tree != null)
            tree.TakeDamage(damage);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        Vector3 hitPoint = other.ClosestPoint(transform.position);

        // 💥 VFX
        if (hitVfxPrefab != null)
        {
            Instantiate(hitVfxPrefab, hitPoint, Quaternion.identity);
        }

        // 📸 CAMERA SHAKE (вот здесь)
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(shakeDuration, shakeMagnitude);
        }

        DealDamage();
        Destroy(gameObject);
    }
}