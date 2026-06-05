using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private bool firstShotHintShown = false;
    public HintPopup hintPopup;
    [Header("Movement VFX")]
    public Animator dustAnimator;
    public bool inputBlocked;
    public float deathAnimationTime = 2f;
    [Header("VFX")]
    public GameObject hitVfxPrefab;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public UmbilicalCord cord;
    [Header("Footsteps")]
    public AudioClip[] footstepSounds;
    [Range(0f, 1f)]
    public float footstepVolume = 0.7f;
    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform shootPoint;
    public float fireRate = 0.3f;
    public AudioClip shootSound;

    private AudioSource audioSource;
    private Rigidbody rb;
    private Vector3 movement;
    private float fireTimer;
    private Animator anim;

    [Header("Camera Shake")]
    public float shakeDuration = 0.15f;
    public float shakeMagnitude = 0.15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        StartCoroutine(ShowStartHint());
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
    IEnumerator ShowStartHint()
    {
        yield return new WaitForSeconds(1f);

        hintPopup.ShowHint(
        LocalizationManager.Instance.GetText("hint_who_am_i"));
    }
    public void PlayFootstep()
    {
        if (footstepSounds != null &&
            footstepSounds.Length > 0)
        {
            AudioClip clip =
                footstepSounds[
                    Random.Range(0, footstepSounds.Length)
                ];

            audioSource.pitch =
                Random.Range(0.92f, 1.08f);

            audioSource.PlayOneShot(
                clip,
                footstepVolume
            );
        }
    }
    void HandleInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        movement = new Vector3(h, 0, v).normalized;
    }
    void Move()
    {
        Vector3 velocity = movement * moveSpeed;

        anim.SetFloat("Speed", movement.magnitude);

        rb.linearVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        if (dustAnimator != null)
        {
            dustAnimator.SetBool(
                "Moving",
                movement.magnitude > 0.1f
            );
        }
    }
    void HandleRotation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);

            Vector3 direction = hitPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    void HandleShooting()
    {
        if (inputBlocked) return;

        fireTimer -= Time.deltaTime;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            anim.SetTrigger("Shoot");

            fireTimer = fireRate;
        }
    }
    public void Shoot()
    {
        Instantiate(bulletPrefab, shootPoint.position, shootPoint.rotation);

        if (!firstShotHintShown)
        {
            firstShotHintShown = true;

            hintPopup.ShowHint(
                LocalizationManager.Instance.GetText("hint_first_shot")
            );
        }

        if (shootSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(shootSound);
        }
    }
    void ClampDistance()
    {
        if (cord == null) return;

        Vector3 offset = transform.position - cord.tree.position;

        if (offset.magnitude > cord.currentMaxDistance)
        {
            Vector3 clampedPos =
                cord.tree.position +
                offset.normalized * cord.currentMaxDistance;

            rb.MovePosition(clampedPos);
        }
    }

    public void PlayHitFeedback()
    {
        if (anim != null)
        {
            anim.SetTrigger("Hit");
        }
        if (hitVfxPrefab != null)
        {
            Instantiate(
                hitVfxPrefab,
                transform.position,
                Quaternion.identity
            );
        }
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(
                shakeDuration,
                shakeMagnitude
            );
        }

        if (DamageFlash.Instance != null)
        {
            DamageFlash.Instance.Flash(
                new Color(0.35f, 0f, 0.35f, 0.4f)
            );
        }
    }
    public void Die()
    {
        inputBlocked = true;

        rb.linearVelocity = Vector3.zero;

        if (anim != null)
        {
            anim.SetTrigger("Die");
        }
    }
}