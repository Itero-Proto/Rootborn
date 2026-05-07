using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TreeSystem : MonoBehaviour
{
    [Header("UI")]
    public Slider healthSlider;
    private Vector3 targetScale;
    private Coroutine growRoutine;
    public AudioClip growSound;
    private AudioSource audioSource;
    public int organic;
    public int inorganic;
    public AudioClip uvMoveSound;
    public AudioClip takeDamageSound;
    public int level = 1;
    public int maxLevel = 15;

    [Header("Health")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("UV Movement")]
    public float uvSpeed = 1f;
    private Material treeMaterial;

    [Header("Visual")]
    public Renderer treeRenderer;
    public TreeFleshGenerator fleshGenerator;
    private AudioSource uvAudioSource;
    private int lastOrganic = 0;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (treeRenderer != null)
            treeMaterial = treeRenderer.material;

        targetScale = transform.localScale;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
    void Update()
    {
        if (organic > lastOrganic)
        {
            int diff = organic - lastOrganic;

            if (fleshGenerator != null)
                fleshGenerator.AddOrganic(diff);

            lastOrganic = organic;
        }
    }

    // ---------------- DAMAGE ----------------

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (takeDamageSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(takeDamageSound, 0.5f);
        }
        UpdateHealthUI();

        StartCoroutine(DamagePulse());

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    // ---------------- DROPS ----------------

    public void ReceiveDrop(DropType type)
    {
        if (type == DropType.Organic)
        {
            organic++;
            StartCoroutine(OrganicPulse());
        }
        else
        {
            inorganic++;
            StartCoroutine(InorganicPulse());
            TryLevelUp();
        }
    }

    // ---------------- LEVEL SYSTEM ----------------

    void TryLevelUp()
    {
        if (level >= maxLevel)
        {
            GameManager.Instance.EndGame("Tree reached max level");
            return;
        }

        level++;
        Grow();
    }
    void Grow()
    {
        // 🔊 Звук роста
        if (growSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(growSound, 1f);
        }
        Vector3 scale = targetScale;

        scale.y *= 1.05f;
        scale.x *= 1.05f;
        scale.z *= 1.05f;

        targetScale = scale;

        if (growRoutine != null)
            StopCoroutine(growRoutine);

        growRoutine = StartCoroutine(SmoothGrow());
    }

    // ---------------- VISUAL REACTIONS ----------------

    IEnumerator OrganicPulse()
    {
        yield return StartCoroutine(UVMove(1.2f));
    }

    IEnumerator InorganicPulse()
    {
        yield return PulseScale(0.95f, 0.15f);
        Grow();
    }
    
    IEnumerator SmoothGrow()
    {
        Vector3 start = transform.localScale;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime; // ≈ 1 секунда

            transform.localScale = Vector3.Lerp(start, targetScale, t);

            yield return null;
        }

        transform.localScale = targetScale;
    }
    IEnumerator UVMove(float direction)
    {
        if (treeMaterial == null) yield break;

        if (uvMoveSound != null)
        {
            GameObject temp = new GameObject("uvMoveSound");
            temp.transform.position = transform.position;

            AudioSource a = temp.AddComponent<AudioSource>();
            a.PlayOneShot(uvMoveSound, 0.5f);

            Destroy(temp, uvMoveSound.length);
        }

        float t = 0f;
        Vector2 offset = treeMaterial.mainTextureOffset;

        while (t < 1f)
        {
            t += Time.deltaTime;

            offset.x += direction * uvSpeed * Time.deltaTime;
            treeMaterial.mainTextureOffset = offset;

            yield return null;
        }

    }
    IEnumerator PulseScale(float targetMultiplier, float time)
    {
        Vector3 original = transform.localScale;
        Vector3 target = original * targetMultiplier;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transform.localScale = Vector3.Lerp(original, target, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            transform.localScale = Vector3.Lerp(target, original, t);
            yield return null;
        }
    }

    IEnumerator DamagePulse()
    {
        Vector3 original = transform.localScale;

        transform.localScale = original * 0.9f;
        yield return new WaitForSeconds(0.1f);
        transform.localScale = original;
    }
}