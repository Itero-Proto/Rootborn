using System.Collections;
using TMPro;
using UnityEngine;

public class HintPopup : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public CanvasGroup canvasGroup;

    public float fadeSpeed = 2f;
    public float showTime = 3f;

    private Coroutine currentRoutine;

    void Start()
    {
        canvasGroup.alpha = 0;
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
    }
    public void ShowHint(string text)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(HintRoutine(text));
    }

    IEnumerator HintRoutine(string text)
    {
        hintText.text = text;

        // Fade In
        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(showTime);

        // Fade Out
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}