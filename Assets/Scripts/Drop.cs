using UnityEngine;

public class Drop : MonoBehaviour
{
    private HintPopup hintPopup;
    [Header("Core")]
    public DropType type;
    private TreeSystem tree;
    [Header("Bounce")]
    public float minBounceHeight = 0.1f;
    public float maxBounceHeight = 0.4f;

    public float minBounceSpeed = 2f;
    public float maxBounceSpeed = 6f;

    public float minBounceDelay = 0.5f;
    public float maxBounceDelay = 3f;

    private Vector3 startPos;

    private bool isBouncing = false;
    private float bounceTimer;

    private float currentBounceHeight;
    private float currentBounceSpeed;
    private float bounceProgress;
    [Header("Movement")]
    public float attractDistance = 5f;
    public float moveSpeed = 8f;

    [Header("Audio")]
    public AudioClip pickupSound;
    [Header("VFX")]
    public GameObject organicPickupVfx;
    public GameObject inorganicPickupVfx;
    private ResourceFlow flow;
    private Transform player;
    private bool isMovingToPlayer = false;

    void Start()
    {
        hintPopup = FindAnyObjectByType<HintPopup>();
        startPos = transform.position;
        SetNextBounceDelay();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        flow = FindAnyObjectByType<ResourceFlow>();
        tree = FindAnyObjectByType<TreeSystem>();
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
            BounceIdle();
        }
    }
    void BounceIdle()
    {
        // ждём перед прыжком
        if (!isBouncing)
        {
            bounceTimer -= Time.deltaTime;

            if (bounceTimer <= 0f)
            {
                isBouncing = true;

                currentBounceHeight =
                    Random.Range(minBounceHeight, maxBounceHeight);

                currentBounceSpeed =
                    Random.Range(minBounceSpeed, maxBounceSpeed);

                bounceProgress = 0f;
            }

            return;
        }

        // сам прыжок
        bounceProgress += Time.deltaTime * currentBounceSpeed;

        float y = Mathf.Sin(bounceProgress * Mathf.PI)
                  * currentBounceHeight;

        Vector3 pos = transform.position;
        pos.y = startPos.y + y;
        transform.position = pos;

        // прыжок закончился
        if (bounceProgress >= 1f)
        {
            isBouncing = false;

            pos.y = startPos.y;
            transform.position = pos;

            SetNextBounceDelay();
        }
    }
    void SetNextBounceDelay()
    {
        bounceTimer = Random.Range(minBounceDelay, maxBounceDelay);
    }
    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, player.position) < 0.5f)
        {
            TransferToTree();
        }
    }
    void TransferToTree()
    {
        // 💥 VFX в момент подбора
        GameObject vfxPrefab = null;

        switch (type)
        {
            case DropType.Organic:
                vfxPrefab = organicPickupVfx;
                break;

            case DropType.Inorganic:
                vfxPrefab = inorganicPickupVfx;
                break;
        }

        if (vfxPrefab != null)
        {
            Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        }

        // 🌿 поток по кишке
        if (flow != null)
        {
            flow.PlayFlow(type);
        }

        // 🌳 передача дереву
        if (tree != null)
        {
            tree.ReceiveDrop(type);
        }

        if (hintPopup != null)
        {
            // Первый раз гарантированно
            if (!PlayerPrefs.HasKey("FirstRemainsHint"))
            {
                hintPopup.ShowHint(
                    LocalizationManager.Instance.GetText("hint_first_remains")
                );

                PlayerPrefs.SetInt("FirstRemainsHint", 1);
                PlayerPrefs.Save();
            }
            // Потом иногда
            else if (Random.value < 0.15f)
            {
                hintPopup.ShowHint(
                    LocalizationManager.Instance.GetText("hint_first_remains")
                );
            }
        }
        // 🔊 звук
        if (pickupSound != null)
        {
            GameObject temp = new GameObject("PickupSound");
            temp.transform.position = transform.position;

            AudioSource a = temp.AddComponent<AudioSource>();
            a.PlayOneShot(pickupSound, 0.5f);

            Destroy(temp, pickupSound.length);
        }

        Destroy(gameObject);
    }
}