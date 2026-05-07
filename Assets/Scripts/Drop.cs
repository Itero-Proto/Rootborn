using UnityEngine;

public class Drop : MonoBehaviour
{
    [Header("Core")]
    public DropType type;
    private TreeSystem tree;

    [Header("Movement")]
    public float attractDistance = 5f;
    public float moveSpeed = 8f;

    [Header("Rotation")]
    public Transform visual; // 👈 сюда можно назначить модель (лучше использовать)
    public Vector3 minRotationSpeed = new Vector3(30f, 60f, 20f);
    public Vector3 maxRotationSpeed = new Vector3(90f, 180f, 60f);
    private Vector3 rotationSpeed;

    [Header("Audio")]
    public AudioClip pickupSound;

    private ResourceFlow flow;
    private Transform player;
    private bool isMovingToPlayer = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        flow = FindAnyObjectByType<ResourceFlow>();
        tree = FindAnyObjectByType<TreeSystem>();

        // 🎲 случайная скорость вращения по осям
        rotationSpeed = new Vector3(
            Random.Range(minRotationSpeed.x, maxRotationSpeed.x),
            Random.Range(minRotationSpeed.y, maxRotationSpeed.y),
            Random.Range(minRotationSpeed.z, maxRotationSpeed.z)
        );

        // если visual не задан — используем сам объект
        if (visual == null)
            visual = transform;
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
            RotateIdle(); // 👈 вращение только когда лежит
        }
    }

    void RotateIdle()
    {
        visual.Rotate(rotationSpeed * Time.deltaTime);
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            player.position,
            moveSpeed * Time.deltaTime
        );

        // 💫 можно оставить вращение даже во время полёта (выглядит круто)
        visual.Rotate(rotationSpeed * Time.deltaTime * 1.5f);

        if (Vector3.Distance(transform.position, player.position) < 0.5f)
        {
            TransferToTree();
        }
    }

    void TransferToTree()
    {
        if (flow != null)
        {
            flow.PlayFlow();
        }

        if (tree != null)
        {
            tree.ReceiveDrop(type);
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