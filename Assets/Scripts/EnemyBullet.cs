using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 3f;
    public int damage = 1;
    public TreeHealth tree;

    [Header("VFX")]
    public GameObject hitVfxPrefab;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

        if (tree == null)
            tree = GameObject.FindGameObjectWithTag("Tree").GetComponent<TreeHealth>();

        Destroy(gameObject, lifeTime);
    }

    public void DealDamage()
    {
        if (tree != null)
            tree.TakeDamage(damage);
    }
    void OnTriggerEnter(Collider other)
    {
        Vector3 hitPoint = other.ClosestPoint(transform.position);

        if (hitVfxPrefab != null)
        {
            Instantiate(
                hitVfxPrefab,
                hitPoint,
                Quaternion.identity
            );
        }

        if (other.CompareTag("Player"))
        {
            PlayerController player =
                other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.PlayHitFeedback();
            }

            DealDamage();
        }

        Destroy(gameObject);
    }
}