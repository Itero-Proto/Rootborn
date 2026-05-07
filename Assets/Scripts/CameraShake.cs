using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Camera cam;
    private float originalSize;

    private Coroutine currentShake;

    void Awake()
    {
        Instance = this;
        cam = GetComponent<Camera>();
        originalSize = cam.orthographicSize;
    }

    public void Shake(float duration, float magnitude)
    {
        if (currentShake != null)
            StopCoroutine(currentShake);

        currentShake = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float time = 0f;

        while (time < duration)
        {
            // 🔥 случайное изменение размера
            float offset = Random.Range(-1f, 1f) * magnitude;

            cam.orthographicSize = originalSize + offset;

            time += Time.deltaTime;
            yield return null;
        }

        // возврат к норме
        cam.orthographicSize = originalSize;
    }
}