using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TreeSystem : MonoBehaviour
{
    private HintPopup hintPopup;
    private bool organicTwoHintShown = false;
    private Coroutine shakeRoutine;
    private Vector3 targetScale;
    private Coroutine growRoutine;
    public AudioClip growSound;
    private AudioSource audioSource;
    public int organic;
    public int inorganic;
    public AudioClip uvMoveSound;
    public int level = 1;
    public int maxLevel = 15;

    [Header("UV Movement")]
    public float uvSpeed = 1f;
    private Material treeMaterial;

    [Header("Visual")]
    public Renderer treeRenderer;
    public TreeFleshGenerator fleshGenerator;
    private int lastOrganic = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (treeRenderer != null)
            treeMaterial = treeRenderer.material;

        targetScale = transform.localScale;
        hintPopup = FindAnyObjectByType<HintPopup>();
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

    public void ReceiveDrop(DropType type)
    {
        if (type == DropType.Organic)
        {
            organic++;
            StartCoroutine(OrganicPulse());

            if (!organicTwoHintShown && organic >= 2)
            {
                organicTwoHintShown = true;

                if (hintPopup != null)
                {
                    hintPopup.ShowHint(
                        LocalizationManager.Instance.GetText("hint_organic_two")
                    );
                }
            }
        }
        else
        {
            inorganic++;
            StartCoroutine(InorganicPulse());
            TryLevelUp();
        }
    }

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
        if (growSound != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(growSound, 1f);
        }

        StartShake();
        Vector3 scale = targetScale;
        scale.y *= 1.05f;
        scale.x *= 1.05f;
        scale.z *= 1.05f;

        targetScale = scale;

        if (growRoutine != null)
            StopCoroutine(growRoutine);

        growRoutine = StartCoroutine(SmoothGrow());
    }

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
    public void StartShake()
    {
        if (shakeRoutine != null)
        {
            StopCoroutine(shakeRoutine);
        }

        transform.localRotation = Quaternion.identity;

        shakeRoutine = StartCoroutine(ShakeRoutine());
    }
    IEnumerator ShakeRoutine()
    {
        Quaternion originalRot = transform.localRotation;

        float duration = 0.8f;
        float strength = 4f;

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;

            float fade = 1f - (t / duration);

            float angle =
                Mathf.Sin(t * 8f) * strength * fade;

            transform.localRotation =
                originalRot * Quaternion.Euler(0f, 0f, angle);

            yield return null;
        }

        transform.localRotation = originalRot;
    }
    public void LoseLevel()
    {
        if (level <= 1)
            return;

        level--;

        StartShake();

        Vector3 scale = targetScale;

        scale.y *= 0.95f;
        scale.x *= 0.95f;
        scale.z *= 0.95f;

        targetScale = scale;

        if (growRoutine != null)
            StopCoroutine(growRoutine);

        growRoutine = StartCoroutine(SmoothGrow());
    }
}