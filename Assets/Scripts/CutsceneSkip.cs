using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutsceneSkip : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameScene";

    [Header("Audio")]
    public AudioClip fallSound;

    private AudioSource audioSource;
    [Header("Fruit")]
    public Rigidbody fruitRb;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 2f;

    private bool started = false;
    private bool loading = false;
    private void Start()
    {

        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (started)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            started = true;

            // Плод начинает падать
            fruitRb.isKinematic = false;
            fruitRb.useGravity = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (loading)
            return;

        if (collision.gameObject.CompareTag("Ground"))
        {
            loading = true;
            StartCoroutine(FadeAndLoad());
        }

        // 🔊 звук
        if (fallSound != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(fallSound, 0.5f);
        }
    }

    IEnumerator FadeAndLoad()
    {
        float time = 0f;

        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            fadeImage.color = color;

            yield return null;
        }

        SceneManager.LoadScene(gameSceneName);
    }
}