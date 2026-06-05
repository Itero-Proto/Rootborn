using UnityEngine;

public class HealingFruit : MonoBehaviour
{
    public float healAmount = 1f;

    [Header("Audio")]
    public AudioClip healSound;

    [Header("VFX")]
    public GameObject healVfxPrefab;

    [Header("Shake")]
    public float shakeAmount = 0.03f;
    public float shakeSpeed = 25f;

    public float minShakeDelay = 3f;
    public float maxShakeDelay = 6f;
    public float minShakeDuration = 0.25f;
    public float maxShakeDuration = 2f;

    [Header("Movement")]
    public float attractDistance = 5f;
    public float moveSpeed = 8f;

    private TreeHealth tree;
    private Transform player;

    private bool isMovingToPlayer = false;

    private Vector3 startPos;
    private float randomOffset;

    private bool isShaking = false;
    private float shakeTimer;
    private float nextShakeTimer;

    void Start()
    {
        tree = FindAnyObjectByType<TreeHealth>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        randomOffset = Random.Range(0f, 100f);
        SetNextShake();

        if (!PlayerPrefs.HasKey("FirstHealingFruitHint"))
        {
            HintPopup hintPopup = FindAnyObjectByType<HintPopup>();

            if (hintPopup != null)
            {
                hintPopup.ShowHint(
                    LocalizationManager.Instance.GetText("hint_healing_fruit")
                );
            }

            PlayerPrefs.SetInt("FirstHealingFruitHint", 1);
            PlayerPrefs.Save();
        }
    }
    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist < attractDistance)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer)
        {
            MoveToPlayer();
        }
        else
        {
            ShakeIdle();
        }
    }

    void ShakeIdle()
    {
        // ожидание следующей тряски
        if (!isShaking)
        {
            nextShakeTimer -= Time.deltaTime;

            if (nextShakeTimer <= 0f)
            {
                isShaking = true;

                shakeTimer = Random.Range(
                    minShakeDuration,
                    maxShakeDuration
                );

                // запоминаем ТЕКУЩУЮ позицию
                startPos = transform.position;
            }

            return;
        }

        // тряска
        shakeTimer -= Time.deltaTime;

        float x = Mathf.Sin((Time.time + randomOffset) * shakeSpeed)
                  * shakeAmount;

        float z = Mathf.Cos((Time.time + randomOffset) * shakeSpeed * 1.3f)
                  * shakeAmount;

        transform.position = startPos + new Vector3(x, 0f, z);

        // конец тряски
        if (shakeTimer <= 0f)
        {
            isShaking = false;

            // возвращаемся в позицию,
            // с которой началась тряска
            transform.position = startPos;

            SetNextShake();
        }
    }
    void SetNextShake()
    {
        nextShakeTimer = Random.Range(
            minShakeDelay,
            maxShakeDelay
        );
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // ❤️ лечение дерева
        if (tree != null)
        {
            tree.Heal(healAmount);
        }

        // ✨ VFX
        if (healVfxPrefab != null)
        {
            Instantiate(
                healVfxPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        // 🔊 звук
        if (healSound != null)
        {
            GameObject temp = new GameObject("HealSound");

            temp.transform.position = transform.position;

            AudioSource a = temp.AddComponent<AudioSource>();
            a.PlayOneShot(healSound, 0.7f);

            Destroy(temp, healSound.length);
        }

        Destroy(gameObject);
    }
}