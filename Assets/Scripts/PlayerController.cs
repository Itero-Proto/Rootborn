using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public bool inputBlocked;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public UmbilicalCord cord;
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 0.3f;
    public AudioClip shootSound;
    private AudioSource audioSource;
    private Rigidbody rb;
    private Vector3 movement;
    private float fireTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleInput();
        HandleRotation();
        HandleShooting();
    }

    void FixedUpdate()
    {
        Move();
        ClampDistance();
    }

    // ---------------- MOVEMENT ----------------

    void HandleInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        movement = new Vector3(h, 0, v).normalized;
    }

    void Move()
    {
        Vector3 velocity = movement * moveSpeed;

        rb.linearVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );
    }

    // ---------------- ROTATION (FIXED) ----------------

    void HandleRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // ❗ ВАЖНО: НЕ через Rigidbody
                transform.rotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f);
            }
        }
    }
    void HandleShooting()
    {
        if (inputBlocked) return;

        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }
    void Shoot()
    {
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);
        // 🔊 Звук выстрела
        if (shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound);
        }
    }

    // ---------------- LIMIT DISTANCE ----------------
    void ClampDistance()
    {
        if (cord == null) return;

        Vector3 offset = transform.position - cord.tree.position;

        if (offset.magnitude > cord.currentMaxDistance)
        {
            Vector3 clampedPos = cord.tree.position + offset.normalized * cord.currentMaxDistance;
            rb.MovePosition(clampedPos);
        }
    }
}