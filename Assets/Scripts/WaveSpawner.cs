using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
[RequireComponent(typeof(AudioSource))]
public class WaveSpawner : MonoBehaviour
{
    private HintPopup hintPopup;
    public bool wavesStarted = false;
    [Header("Prefabs")]
    public GameObject meleeEnemyPrefab;
    public GameObject shooterEnemyPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int currentWave = 0;
    public float timeBetweenWaves = 5f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public float waveTextDuration = 2f;

    [Header("Audio")]
    public AudioClip waveStartSound;
    private AudioSource audioSource;

    [Header("References")]
    public Transform player;
    public UpgradeManager upgradeManager;

    private List<GameObject> aliveEnemies = new List<GameObject>();

    private float timer;
    private bool waveInProgress = false;
    private bool waitingForUpgrade = false;

    void Start()
    {
        hintPopup = FindAnyObjectByType<HintPopup>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();

        timer = timeBetweenWaves;

        if (waveText != null)
            waveText.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!wavesStarted)
            return;
        CleanDeadEnemies();

        // ---------------- WAVE END ----------------
        if (waveInProgress &&
            aliveEnemies.Count == 0 &&
            !waitingForUpgrade)
        {
            waveInProgress = false;
            waitingForUpgrade = true;

            upgradeManager.ShowUpgrades();
        }

        // ---------------- UPGRADE DONE ----------------
        if (waitingForUpgrade && !upgradeManager.isChoosing)
        {
            waitingForUpgrade = false;
            timer = timeBetweenWaves;
        }

        // ---------------- NEXT WAVE TIMER ----------------
        if (!waveInProgress && !waitingForUpgrade)
        {
            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StartNextWave();
            }
        }
    }

    // ---------------- WAVES ----------------

    void StartNextWave()
    {
        currentWave++;
        if (hintPopup != null)
        {
            if (currentWave == 1)
                StartCoroutine(ShowWaveHint("hint_first_wave"));
            if (currentWave == 2)
                StartCoroutine(ShowWaveHint("hint_second_wave"));
            if (currentWave == 3)
                StartCoroutine(ShowWaveHint("hint_third_wave"));
        }
        waveInProgress = true;

        int enemyCount = currentWave;

        // 🔊 Звук начала волны
        if (waveStartSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(waveStartSound, 0.7f);
        }

        // 📝 Показ текста
        if (waveText != null)
        {
            StartCoroutine(ShowWaveText($"DAY {currentWave}"));
        }

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        Debug.Log($"DAY {currentWave} started | Enemies: {enemyCount}");
    }
    IEnumerator ShowWaveHint(string localizationKey)
    {
        yield return new WaitForSeconds(2.5f);

        hintPopup.ShowHint(
            LocalizationManager.Instance.GetText(localizationKey)
        );
    }
    IEnumerator ShowWaveText(string text)
    {
        waveText.gameObject.SetActive(true);
        waveText.text = text;

        RectTransform rt = waveText.rectTransform;

        Vector3 startScale = Vector3.zero;
        Vector3 targetScale = Vector3.one;

        float t = 0f;
        float duration = 0.3f;

        // 🔼 Появление
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        rt.localScale = targetScale;

        yield return new WaitForSeconds(waveTextDuration);

        // 🔽 Исчезновение
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            rt.localScale = Vector3.Lerp(targetScale, startScale, t);
            yield return null;
        }

        waveText.gameObject.SetActive(false);
    }
    void SpawnEnemy()
    {
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject prefab = (Random.value < 0.6f)
            ? meleeEnemyPrefab
            : shooterEnemyPrefab;

        GameObject enemy = Instantiate(prefab, point.position, Quaternion.identity);

        EnemyController ec = enemy.GetComponent<EnemyController>();

        if (ec != null)
        {
            ec.player = player;
        }

        aliveEnemies.Add(enemy);
    }

    // ---------------- CLEANUP ----------------

    void CleanDeadEnemies()
    {
        aliveEnemies.RemoveAll(e => e == null);
    }
    public void StartWaves()
    {
        wavesStarted = true;
    }
}