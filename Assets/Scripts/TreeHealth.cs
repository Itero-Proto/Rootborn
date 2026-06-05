using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TreeHealth : MonoBehaviour
{
    private bool firstHealHintShown = false;
    private HintPopup hintPopup;
    private bool firstDamageHintShown = false;
    [Header("UI")]
    public Image healthCircle;
    public TreeSystem treeSystem;
    [Header("Health")]
    public float maxHealth = 10f;
    public float currentHealth;
    [Header("Audio")]
    public AudioClip takeDamageSound;
    Coroutine healPulseRoutine;
    private AudioSource audioSource;
    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
        audioSource = GetComponent<AudioSource>();
        hintPopup = FindAnyObjectByType<HintPopup>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (!firstDamageHintShown)
        {
            firstDamageHintShown = true;

            if (hintPopup != null)
            {
                hintPopup.ShowHint(
                    LocalizationManager.Instance.GetText("hint_tree_damaged")
                );
            }
        }
        // 🔊 звук
        if (takeDamageSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);

            audioSource.PlayOneShot(
                takeDamageSound,
                0.75f
            );
        }
        UpdateUI();
        if (treeSystem != null)
            treeSystem.StartShake();
            treeSystem.LoseLevel();
        if (currentHealth <= 0f)
        {
            Die();
        }
    }
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (!firstHealHintShown)
        {
            firstHealHintShown = true;

            if (hintPopup != null)
            {
                hintPopup.ShowHint(
                    LocalizationManager.Instance.GetText("hint_tree_healed")
                );
            }
        }
        UpdateUI();

        if (healPulseRoutine != null)
            StopCoroutine(healPulseRoutine);

        healPulseRoutine = StartCoroutine(HealPulse());
    }
    IEnumerator HealPulse()
    {
        Vector3 original = healthCircle.transform.localScale;
        Vector3 target = original * 1.15f;

        float t = 0f;
        float speed = 10f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            healthCircle.transform.localScale = Vector3.Lerp(original, target, t);
            yield return null;
        }

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            healthCircle.transform.localScale = Vector3.Lerp(target, original, t);
            yield return null;
        }

        healthCircle.transform.localScale = original;
    }

    void UpdateUI()
    {
        float t = currentHealth / maxHealth;
        healthCircle.fillAmount = t;
    }
    public void Die()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.GameOver();
    }
}