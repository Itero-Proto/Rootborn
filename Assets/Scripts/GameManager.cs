using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerController player;
    public static System.Action OnGameEnded;
    public static bool GameEnded;
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

        GameEnded = false;
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
        GameEnded = true;

        OnGameEnded?.Invoke(); // 🔥 ВАЖНО

        if (player != null)
            player.Die();

        StartCoroutine(GameOverRoutine());
    }
    IEnumerator GameOverRoutine()
    {
        PlaySound(gameOverSound);

        // 🔴 ЖДЁМ анимацию смерти
        yield return new WaitForSecondsRealtime(2f);

        yield return FadeToBlack(3f);

        Time.timeScale = 0f;

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