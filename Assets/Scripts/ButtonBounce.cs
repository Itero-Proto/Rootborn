using UnityEngine;

public class ButtonBounce : MonoBehaviour
{
    public float bounceSpeed = 2f;
    public float bounceHeight = 30f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Используем абсолютное значение синуса для эффекта отскока
        float bounce = Mathf.Abs(Mathf.Sin(Time.time * bounceSpeed)) * bounceHeight;
        rectTransform.anchoredPosition = originalPosition + new Vector3(0, bounce, 0);
    }
}