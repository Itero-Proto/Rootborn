using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject endGamePanel;
    public GameObject gameOverPanel;
    public GameObject hiddenEndPanel;
    public Image fadeImage;

    [Header("Audio")]
    public AudioClip endGameSound;
    public AudioClip gameOverSound;
    public AudioClip hiddenEndSound;

    private AudioSource audioSource;
    private bool gameEnded = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // === НАСТРОЙКА FPS ===
        // Отключаем V-Sync
        QualitySettings.vSyncCount = 0;
        // Ограничиваем до 60 FPS
        Application.targetFrameRate = 60;

        audioSource = GetComponent<AudioSource>();
    }

    public void EndGame(string reason)
    {
        if (gameEnded) return;

        gameEnded = true;

        Time.timeScale = 0f;

        PlaySound(endGameSound);

        if (endGamePanel != null)
            endGamePanel.SetActive(true);
    }

    public void EndGameCord()
    {
        if (gameEnded) return;

        gameEnded = true;

        Time.timeScale = 0f;

        PlaySound(hiddenEndSound);

        if (hiddenEndPanel != null)
            hiddenEndPanel.SetActive(true);
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;

        Time.timeScale = 0f;

        PlaySound(gameOverSound);

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clip, 0.8f);
        }
    }

    public IEnumerator FadeToBlack(float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime;
            c.a = Mathf.Lerp(0, 1, t);
            fadeImage.color = c;
            yield return null;
        }
    }
}