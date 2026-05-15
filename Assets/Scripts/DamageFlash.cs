using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageFlash : MonoBehaviour
{
    public static DamageFlash Instance;

    [Header("UI")]
    public Image flashImage;

    [Header("Settings")]
    public float flashDuration = 0.15f;

    private Coroutine flashRoutine;

    void Awake()
    {
        Instance = this;

        if (flashImage != null)
        {
            Color c = flashImage.color;
            c.a = 0f;
            flashImage.color = c;
        }
    }

    public void Flash(Color color)
    {
        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashRoutine(color));
    }

    IEnumerator FlashRoutine(Color color)
    {
        // показать
        flashImage.color = color;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.unscaledDeltaTime / flashDuration;

            Color c = color;
            c.a = Mathf.Lerp(color.a, 0f, t);

            flashImage.color = c;

            yield return null;
        }

        Color end = flashImage.color;
        end.a = 0f;
        flashImage.color = end;
    }

}